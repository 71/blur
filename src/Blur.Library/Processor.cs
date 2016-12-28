using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using System.Linq;

// This file defines the internal Blur processor.
// If the signature of the declarations below changes,
// please update src\Blur\Processor.cs.

namespace Blur
{

    /// <summary>
    /// Defines the inner Blur processor. This class will
    /// edit an <see cref="Assembly"/> in which it is loaded.
    /// </summary>
    internal static class Processor
    {
        public static Assembly Target                     { get; private set; }
        public static AssemblyDefinition TargetDefinition { get; private set; }
        public static BlurAttribute Settings              { get; private set; }

        public static Module TargetModule { get; private set; }
        public static ModuleDefinition TargetModuleDefinition { get; private set; }

#pragma warning disable CS0649
        public static Func<Assembly[]> GetAssemblies;
        public static Func<string, Assembly> GetAssembly;
        public static Func<string, Stream> GetAssemblyStream;
#pragma warning restore CS0649

        /// <summary>
        /// Process the given assembly.
        /// </summary>
        /// <param name="targetPath">The path of a shadow copy of the <see cref="Assembly"/> to process.</param>
        /// <param name="weavingAssembly">The <see cref="Assembly"/> to process.</param>
        /// <param name="settings">The <see cref="BlurAttribute"/> set on the <see cref="Assembly"/> to process.</param>
        /// <param name="targetStream"><see cref="Stream"/> to the assembly to weave.</param>
        public static void Process(string targetPath, Assembly weavingAssembly, BlurAttribute settings, Stream targetStream)
        {
            Settings = settings;
            Target = weavingAssembly;

            // Open a stream to the assembly.
            // Currently, Mono.Cecil crashes if it owns the stream, so we do it ourselves.
            using (targetStream)
            {
                TargetDefinition = AssemblyDefinition.ReadAssembly(targetStream, new ReaderParameters
                {
                    InMemory = true,
                    ReadWrite = true,
                    AssemblyResolver = new AssemblyResolver()
                });
                TargetModuleDefinition = TargetDefinition.MainModule;
                TargetModule = Target.Modules.First(x => x.Name == TargetModuleDefinition.Name);

                // Load all referenced assemblies.
                // Some assemblies are lazily loaded. We don't want that.
                foreach (AssemblyNameReference anRef in TargetModuleDefinition.AssemblyReferences)
                {
                    try
                    {
                        Assembly.Load(new AssemblyName(anRef.FullName));
                    }
                    catch (Exception)
                    {
                        // If it happens, it shouldn't matter much.
                    }
                }

                // Find all visitors in this assembly, and referenced assemblies.
                List<BlurVisitor> visitors = FindVisitors();

                // Execute all visitors BEFORE.
                if (visitors.Count > 0)
                    ExecuteVisitors(visitors, ProcessingState.Before);

                // Execute the internal visitor.
                AttributesVisitor internalVisitor = new AttributesVisitor();
                ExecuteVisitors(new[] { internalVisitor }, ProcessingState.Before);

                // Execute all visitors AFTER.
                if (visitors.Count > 0)
                    ExecuteVisitors(visitors, ProcessingState.After);

                // Optionally clean up.
                if (Settings.CleanUp)
                    CleanUp();

                // Save assembly.
                using (MemoryStream ms = new MemoryStream())
                {
                    TargetDefinition.Write(ms);

                    targetStream.SetLength(0);
                    ms.Position = 0;
                    ms.CopyTo(targetStream);
                }
            }
        }

        /// <summary>
        /// Returns a list containing all the visitors that
        /// could be found in this assembly, and referenced assemblies.
        /// </summary>
        private static List<BlurVisitor> FindVisitors()
        {
            List<BlurVisitor> visitors = new List<BlurVisitor>();

            // Search this assembly.
            visitors.AddRange(FindVisitors(Target));
            
            // Search referenced assemblies.
            foreach (Assembly assembly in GetAssemblies())
                visitors.AddRange(FindVisitors(assembly));

            return visitors;
        }

        /// <summary>
        /// Returns a list containing all the visitors that
        /// could be found in the given assembly.
        /// </summary>
        private static IEnumerable<BlurVisitor> FindVisitors(Assembly assembly)
        {
            TypeInfo visitorType = typeof(BlurVisitor).GetTypeInfo();
            string[] allowedVisitors = Settings.Visitors;

            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (!visitorType.IsAssignableFrom(type))
                    continue;
                if (type.IsGenericType)
                    throw new NotSupportedException($"A {nameof(BlurVisitor)} cannot be generic.");
                if (type.Name == nameof(AttributesVisitor)
                 || type.Name == nameof(BlurVisitor)
                 || (allowedVisitors != null && !allowedVisitors.Contains(type.Name)))
                    continue;

                ConstructorInfo visitorCtor = type.DeclaredConstructors.FirstOrDefault();

                if (visitorCtor == null)
                    throw new NotSupportedException($"A {nameof(BlurVisitor)} must declare a parameterless constructor.");

                yield return (BlurVisitor)visitorCtor.Invoke(new object[0]);
            }
        }

        /// <summary>
        /// Execute the given visitors on the target assembly.
        /// </summary>
        private static void ExecuteVisitors(IReadOnlyList<BlurVisitor> visitors, ProcessingState state)
        {
            var targetTypes = TargetDefinition.MainModule.Types;

            if (state == ProcessingState.Before)
            {
                for (int i = 0; i < visitors.Count; i++)
                    visitors[i].Visit(TargetDefinition, ProcessingState.Before);
            }

            for (int i = 0; i < targetTypes.Count; i++)
                ExecuteVisitors(visitors, state, targetTypes[i]);

            if (state == ProcessingState.After)
            {
                for (int i = 0; i < visitors.Count; i++)
                    visitors[i].Visit(TargetDefinition, ProcessingState.After);
            }
        }

        /// <summary>
        /// Execute the given visitors on a type.
        /// </summary>
        private static void ExecuteVisitors(IReadOnlyList<BlurVisitor> visitors, ProcessingState state,
            TypeDefinition targetType)
        {
            // Fields
            ExecuteVisitors(visitors, state, targetType.Fields);

            // Events
            ExecuteVisitors(visitors, state, targetType.Events);

            // Properties
            ExecuteVisitors(visitors, state, targetType.Properties);

            // Methods
            ExecuteVisitors(visitors, state, targetType.Methods);

            // Nested types
            foreach (TypeDefinition type in targetType.NestedTypes)
                ExecuteVisitors(visitors, state, type);

            // Type
            for (int x = 0; x < visitors.Count; x++)
                visitors[x].Visit(targetType, state);
        }

        /// <summary>
        /// Execute the given visitors on a list of members.
        /// </summary>
        private static void ExecuteVisitors<T>(IReadOnlyList<BlurVisitor> visitors, ProcessingState state,
            IList<T> toVisit) where T : IMemberDefinition
        {
            int count = toVisit.Count;

            for (int i = 0; i < count; i++)
            {
                for (int x = 0; x < visitors.Count; x++)
                {
                    visitors[x].Visit(toVisit[i], state);

                    if (toVisit.Count != count)
                    {
                        i += toVisit.Count - count;
                        count = toVisit.Count;
                        continue;
                    }

                    if (toVisit[i].DeclaringType != null)
                        continue;

                    // The visitor removed this member...
                    i--;
                    goto EndMemberLoop;
                }

                EndMemberLoop: ;
            }
        }

        /// <summary>
        /// Clean up declarations, references and visitors in an assembly.
        /// </summary>
        /// <remarks>
        /// We don't clean up custom attributes here, because it was
        /// already done by the <see cref="AttributesVisitor"/>.
        /// </remarks>
        private static void CleanUp()
        {
            // Remove visitors / weavers.
            var types = TargetDefinition.MainModule.Types;

            for (int i = 0; i < types.Count; i++)
            {
                TypeDefinition type = types[i];

                // Does this type inherit "IWeaver" or "BlurVisitor"?
                if (type.Interfaces.Any(x => x.InterfaceType.FullName == $"{nameof(Blur)}.{nameof(IWeaver)}") ||
                    type.IsMatch(x => x.FullName == $"{nameof(Blur)}.{nameof(BlurVisitor)}"))
                    types.RemoveAt(i--);
            }

            // Remove blur attribute.
            var customAttributesOnAssembly = TargetDefinition.CustomAttributes;

            for (int i = 0; i < customAttributesOnAssembly.Count; i++)
            {
                CustomAttribute attribute = customAttributesOnAssembly[i];

                if (attribute.AttributeType.FullName == $"{nameof(Blur)}.{nameof(BlurAttribute)}")
                {
                    customAttributesOnAssembly.RemoveAt(i);
                    break;
                }
            }

            // Remove reference to Blur.
            string blurFullname = typeof(BlurVisitor).GetTypeInfo().Assembly.GetName().FullName;
            string cecilFullname = typeof(TypeDefinition).GetTypeInfo().Assembly.GetName().FullName;
            var references = TargetDefinition.MainModule.AssemblyReferences;

            for (int i = 0; i < references.Count; i++)
            {
                if (references[i].FullName == blurFullname
                 || references[i].FullName == cecilFullname)
                {
                    references.RemoveAt(i--);
                }
            }
        }
    }
}
