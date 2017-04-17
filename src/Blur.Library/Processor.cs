using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using System.Linq;
using System.Text;
using Mono.Cecil.Cil;

// This file defines the internal Blur processor.
// If the signature of the declarations below changes,
// please update src\Blur\Processor.cs.

namespace Blur
{
    /// <summary>
    /// Defines the inner Blur processor. This class will
    /// edit an <see cref="Assembly"/> in which it is loaded.
    /// </summary>
    internal static partial class Processor
    {
        public static Assembly Target { get; private set; }
        public static AssemblyDefinition TargetDefinition { get; private set; }
        public static BlurAttribute Settings { get; private set; }

        public static Module TargetModule { get; private set; }
        public static ModuleDefinition TargetModuleDefinition { get; private set; }

        public static Stream SymbolsStream { get; private set; }

#pragma warning disable CS0649
        public static Func<Assembly[]> GetAssemblies;
        public static Func<string, Assembly> GetAssembly;
        public static Func<string, Stream> GetAssemblyStream;

        public static Action<string, bool> LogMessage;
#pragma warning restore CS0649

        public static readonly ISymbolWriterProvider WriterProvider = new PortablePdbWriterProvider();
        public static readonly ISymbolReaderProvider ReaderProvider = new PortablePdbReaderProvider();
        public static readonly AssemblyResolver AssemblyResolver    = new AssemblyResolver();

        /// <summary>
        /// Process the given assembly.
        /// </summary>
        /// <param name="targetPath">The path of a shadow copy of the <see cref="Assembly"/> to process.</param>
        /// <param name="weavingAssembly">The <see cref="Assembly"/> to process.</param>
        /// <param name="settings">The <see cref="BlurAttribute"/> set on the <see cref="Assembly"/> to process.</param>
        /// <param name="targetStream"><see cref="Stream"/> to the assembly to weave.</param>
        /// <param name="symbolStream"><see cref="Stream"/> to the symbols of the assembly to weave, or <see langword="null"/>.</param>
        public static void Process(string targetPath, Assembly weavingAssembly, BlurAttribute settings, Stream targetStream, Stream symbolStream)
        {
            Settings = settings;
            Target = weavingAssembly;
            SymbolsStream = symbolStream;

            // Open a stream to the assembly.
            // Currently, Mono.Cecil crashes if it owns the stream, so we do it ourselves.
            using (targetStream)
            {
                TargetDefinition = ReadAssembly(targetStream);
                TargetModuleDefinition = TargetDefinition.MainModule;
                TargetModule = Target.Modules.FirstOrDefault(x => x.Name == TargetModuleDefinition.Name)
                               ?? Target.Modules.First();

                // Load all referenced assemblies.
                // Some assemblies are lazily loaded. We don't want that.
                foreach (AssemblyNameReference anRef in TargetModuleDefinition.AssemblyReferences)
                {
                    try
                    {
                        Assembly.Load(new AssemblyName(anRef.FullName));
                    }
                    catch
                    {
                        // If it happens, it shouldn't matter much.
                    }
                }

                // Find all visitors in this assembly, and referenced assemblies.
                List<BlurVisitor> visitors = FindVisitors();

                visitors.Add(new AttributesVisitor());
                visitors.Sort((a, b) => a.Priority - b.Priority);

                // Execute all visitors.
                IList<TypeDefinition> targetTypes = TargetDefinition.MainModule.Types;

                for (int i = 0; i < visitors.Count; i++)
                {
                    visitors[i].Visit(TargetDefinition);

                    for (int o = 0; o < targetTypes.Count; o++)
                    {
                        TypeDefinition targetType = targetTypes[o];

                        ExecuteVisitor(visitors[i], targetType);
                    }
                }

                // Optionally clean up.
                if (Settings.CleanUp)
                    CleanUp();

                // Save assembly.
                SaveAssembly(targetStream);
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
                    throw new NotSupportedException(
                        $"A {nameof(BlurVisitor)} must declare a parameterless constructor.");

                yield return (BlurVisitor)visitorCtor.Invoke(new object[0]);
            }
        }

        /// <summary>
        /// Execute the given visitors on a type.
        /// </summary>
        private static void ExecuteVisitor(BlurVisitor visitor, TypeDefinition targetType)
        {
            // Fields
            ExecuteVisitor(visitor, targetType.Fields);

            // Events
            ExecuteVisitor(visitor, targetType.Events);

            // Properties
            ExecuteVisitor(visitor, targetType.Properties);

            // Methods
            ExecuteVisitor(visitor, targetType.Methods);

            // Nested types
            foreach (TypeDefinition type in targetType.NestedTypes)
                ExecuteVisitor(visitor, type);

            // Type
            visitor.Visit(targetType);
        }

        /// <summary>
        /// Execute the given visitors on a list of members.
        /// </summary>
        private static void ExecuteVisitor<T>(BlurVisitor visitor, IList<T> toVisit) where T : IMemberDefinition
        {
            int count = toVisit.Count;

            for (int i = 0; i < count; i++)
            {
                visitor.Visit(toVisit[i]);

                if (toVisit.Count == count)
                    continue;

                i += toVisit.Count - count;
                count = toVisit.Count;
            }
        }

        /// <summary>
        /// Field used to store methods that will be deleted during clean-up.
        /// </summary>
        private static readonly Stack<MethodDefinition> MarkedForDeletion = new Stack<MethodDefinition>();

        /// <summary>
        /// Mark a method for deletion.
        /// When cleaning up, this method will be removed.
        /// If its declaring type no longer declares a method, it
        /// will be removed as well.
        /// </summary>
        public static void MarkForDeletion(MethodDefinition method)
        {
            if (!MarkedForDeletion.Contains(method))
                MarkedForDeletion.Push(method);
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
            // Get a list of all assemblies that shall be unreferenced.
            string[] toUnreference = new string[Settings.AdditionalUnreferencedAssemblies.Length + 1];

            toUnreference[0] = "Blur.Library";
            Settings.AdditionalUnreferencedAssemblies.CopyTo(toUnreference, 1);

            // Remove visitors / weavers.
            var types = TargetDefinition.MainModule.Types;

            for (int i = 0; i < types.Count; i++)
            {
                TypeDefinition type = types[i];

                // Does this type inherit "IWeaver" or "BlurVisitor"?
                if (type.Interfaces.Any(x => x.InterfaceType.Is<IWeaver>() || type.Is<BlurVisitor>()))
                    types.RemoveAt(i--);
            }

            // Remove blur attribute.
            var customAttributesOnAssembly = TargetDefinition.CustomAttributes;

            for (int i = 0; i < customAttributesOnAssembly.Count; i++)
            {
                CustomAttribute attribute = customAttributesOnAssembly[i];

                if (Array.IndexOf(toUnreference, attribute.AttributeType.Scope.Name) != -1)
                {
                    customAttributesOnAssembly.RemoveAt(i);
                    break;
                }
            }

            // Remove reference to Blur.
            var references = TargetDefinition.MainModule.AssemblyReferences;

            for (int i = 0; i < references.Count; i++)
            {
                if (Array.IndexOf(toUnreference, references[i].Name) != -1)
                {
                    references.RemoveAt(i--);
                }
            }

            // Remove methods marked for deletion.
            foreach (MethodDefinition method in MarkedForDeletion)
            {
                TypeDefinition declaringType = method.DeclaringType;

                declaringType.Methods.Remove(method);

                if (declaringType.Methods.Count == 0)
                {
                    // If the type no longer stores methods,
                    // it can be safely deleted.
                    if (declaringType.DeclaringType != null)
                        declaringType.DeclaringType.NestedTypes.Remove(declaringType);
                    else
                        declaringType.Module.Types.Remove(declaringType);
                }
            }
        }


        #region Utils
        private static AssemblyDefinition ReadAssembly(Stream assemblyStream)
        {
            assemblyStream.Position = 0;

            if (SymbolsStream != null)
            {
                try
                {
                    return AssemblyDefinition.ReadAssembly(assemblyStream, new ReaderParameters
                    {
                        InMemory = true,
                        ReadWrite = true,
                        AssemblyResolver = AssemblyResolver,
                        SymbolStream = SymbolsStream,
                        SymbolReaderProvider = ReaderProvider,
                        MetadataResolver = new MetadataResolver(AssemblyResolver)
                    });
                }
                catch
                {
                    // Fallback to loading no symbols.
                    assemblyStream.Position = 0;
                }
            }

            return AssemblyDefinition.ReadAssembly(assemblyStream, new ReaderParameters
            {
                InMemory = true,
                ReadWrite = true,
                AssemblyResolver = AssemblyResolver,
                MetadataResolver = new MetadataResolver(AssemblyResolver)
            });
        }

        private static void SaveAssembly(Stream stream)
        {
            using (MemoryStream ms = new MemoryStream((int)stream.Length))
            {
                stream.Position = 0;

                if (SymbolsStream != null && TargetDefinition.MainModule.HasSymbols)
                {
                    SymbolsStream.Position = 0;

                    TargetDefinition.Write(ms, new WriterParameters
                    {
                        SymbolStream = SymbolsStream,
                        SymbolWriterProvider = WriterProvider
                    });
                }
                else
                {
                    TargetDefinition.Write(ms, new WriterParameters());
                }

                if (stream.Length > ms.Length)
                    stream.SetLength(ms.Length);

                ms.Position = 0;
                ms.CopyTo(stream);
            }
        } 
        #endregion
    }
}
