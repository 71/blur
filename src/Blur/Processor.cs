﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

#if NET_CORE
using System.Runtime.Loader;
#endif

namespace Blur.Processing
{
    /// <summary>
    /// Static class used to run, edit and save assemblies
    /// using Blur.
    /// </summary>
    public static class Processor
    {
        private const string ATTRIBUTE_FULLNAME      = "Blur.BlurAttribute";
        private const string PROCESSOR_FULLNAME      = "Blur.Processor";
        private const string PROCESSOR_METHOD        = "Process";
        private const string PROCESSOR_GETASSEMBLIES = "GetAssemblies";
        private const string PROCESSOR_GETASSEMBLY   = "GetAssembly";
        private const string PROCESSOR_GETASSEMBLYSTREAM = "GetAssemblyStream";

        private static Dictionary<string, byte[]> assembliesBytes = new Dictionary<string, byte[]>();
        private static string AssemblyPath;
        private static Assembly Assembly;

        /// <summary>
        /// Process the <see cref="Assembly"/> at the given path.
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly to process.</param>
        /// <param name="copyPath">Path of the shadow copy of the assembly to process. If <see langword="null"/>, a temporary file will be used instead.</param>
        /// <inheritdoc cref="FileStream(string, FileMode, FileAccess)" select="exception"/>
        public static void Process(string assemblyPath, string copyPath = null)
        {
            AssemblyPath = assemblyPath;

            // Copy file to modify it later.
            if (copyPath == null)
                copyPath = Path.GetTempFileName();

            File.Copy(assemblyPath, copyPath, true);

#if NET_CORE
            // Open assembly.
            using (FileStream assemblyStream = new FileStream(assemblyPath, FileMode.Open, FileAccess.ReadWrite))
            {
                // Load the assembly.
                Assembly = AssemblyLoadContext.Default.LoadFromStream(assemblyStream);

                // Handle case where Blur.Library can't be found.
                AssemblyLoadContext.Default.Resolving += ResolvingAssembly;
            }
#else
            // Load the assembly.
            Assembly = Assembly.LoadFrom(assemblyPath);

            // Handle case where Blur.Library can't be found.
            AppDomain.CurrentDomain.AssemblyResolve += ResolvingAssembly;
#endif

            // Finally, process the assembly.
            ProcessAssembly(copyPath);
        }

        /// <summary>
        /// Attempts to resolve an assembly.
        /// </summary>
#if NET_CORE
        private static Assembly ResolvingAssembly(AssemblyLoadContext loadContext, AssemblyName name)
        {
            string assemblyPath = Path.Combine(Path.GetDirectoryName(AssemblyPath), name.Name + ".dll");

            if (File.Exists(assemblyPath))
                return loadContext.LoadFromAssemblyPath(assemblyPath);
        
            return AssemblyResolver.Resolve(name);
        }
#else
        private static Assembly ResolvingAssembly(object sender, ResolveEventArgs e)
        {
            string assemblyDirectory = (sender as AppDomain)?.BaseDirectory ?? Path.GetDirectoryName(AssemblyPath);

            if (assemblyDirectory == null)
                return null;

            string assemblyPath = Path.Combine(assemblyDirectory, e.Name.Substring(0, e.Name.IndexOf(',')) + ".dll");

            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);

            return AssemblyResolver.Resolve(e.Name);
        }
#endif

        /// <summary>
        /// Process <see cref="Assembly"/>.
        /// </summary>
        private static void ProcessAssembly(string copyPath)
        {
            // Get the BlurAttribute set on the assembly.
            CustomAttributeData blurAttribute =
                Assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == ATTRIBUTE_FULLNAME);

            if (blurAttribute == null)
                throw new ArgumentException("The given assembly must be marked with the Blur attribute.", nameof(Assembly));

            // Make the Blur attribute out of its data.
            object[] args = new object[blurAttribute.ConstructorArguments.Count];
            for (int i = 0; i < args.Length; i++)
                args[i] = blurAttribute.ConstructorArguments[i].Value;

            Type attrType = blurAttribute.AttributeType;
            object attrObj = blurAttribute.Constructor.Invoke(args);

            if (blurAttribute.NamedArguments != null)
            {
                foreach (CustomAttributeNamedArgument namedArg in blurAttribute.NamedArguments)
                {
                    if (namedArg.IsField)
                        attrType.GetField(namedArg.MemberName).SetValue(attrObj, namedArg.TypedValue.Value);
                    else
                        attrType.GetProperty(namedArg.MemberName).SetValue(attrObj, namedArg.TypedValue.Value);
                }
            }

            // Retrieve the BlurAttribute's TypeInfo, to access its declaring Assembly.
            TypeInfo attributeType = blurAttribute.AttributeType.GetTypeInfo();

            // Set the processor's GetAssemblies function.
            Func<Assembly[]> getAssemblies = AppDomain.CurrentDomain.GetAssemblies;
            Func<string, Assembly> getAssembly = assemblyName
                => AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == assemblyName)
                ?? AssemblyResolver.Resolve(new AssemblyName(assemblyName));
            Func<string, Stream> getAssemblyStream = assemblyName =>
            {
                string location = getAssembly(assemblyName)?.Location;

                if (location == null)
                    return null;

                try
                {
                    return File.OpenRead(location);
                }
                catch
                {
                    return null;
                }
            };

            Type processor = attributeType.Assembly.GetType(PROCESSOR_FULLNAME);

            processor.GetField(PROCESSOR_GETASSEMBLIES).SetValue(null, getAssemblies);
            processor.GetField(PROCESSOR_GETASSEMBLY).SetValue(null, getAssembly);
            processor.GetField(PROCESSOR_GETASSEMBLYSTREAM).SetValue(null, getAssemblyStream);

            // Call Processor.Process();
            processor.GetMethod(PROCESSOR_METHOD, new[] { typeof(string), typeof(Assembly), attrType, typeof(Stream) })
                     .Invoke(null, new[] { copyPath, Assembly, attrObj, File.Open(copyPath, FileMode.Open) });
        }
    }
}
