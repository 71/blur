using System;
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
        #region Fields & Constructor
        private const string ATTRIBUTE_FULLNAME = "Blur.BlurAttribute";
        private const string PROCESSOR_FULLNAME = "Blur.Processor";
        private const string PROCESSOR_METHOD = "Process";
        private const string PROCESSOR_GETASSEMBLIES = "GetAssemblies";
        private const string PROCESSOR_GETASSEMBLY = "GetAssembly";
        private const string PROCESSOR_LOGMESSAGE = "LogMessage";
        private const string PROCESSOR_PREPROCESS = "Preprocess";
        private const string PROCESSOR_GETASSEMBLYSTREAM = "GetAssemblyStream";

        private static bool WillSave = true;
        private static bool HasBeenInitialized;
        private static string AssemblyDirectory;
        private static string AssemblyPath;

        private static Stream TargetStream;
        private static Stream SymbolStream;
        private static string TargetPath;

        internal static Assembly Assembly;
        internal static Assembly BlurLibrary;
        private static Type ProcessorType;

        public static event Action<string> MessageLogged;
        public static event Action<string> WarningLogged;

        static Processor()
        {
#if NET_CORE
            // Handle case where Blur.Library can't be found.
            AssemblyLoadContext.Default.Resolving += ResolvingAssembly;
#else
            // Handle case where Blur.Library can't be found.
            AppDomain.CurrentDomain.AssemblyResolve += ResolvingAssembly;
#endif
        }
        #endregion

        #region Public methods
        /// <summary>
        /// Initialize the <see cref="Processor"/>, given the file that it'll modify.
        /// </summary>
        /// <param name="assemblyPath">Path of the assembly to process.</param>
        /// <param name="copyPath">Path of the shadow copy of the assembly to process. If <see langword="null"/>, a temporary file will be used instead.</param>
        public static void Initialize(string assemblyPath, string copyPath = null)
        {
            HasBeenInitialized = true;

            TargetPath = copyPath;
            AssemblyPath = assemblyPath;
            AssemblyDirectory = Path.GetDirectoryName(assemblyPath);

            // Copy file to modify it later.
            if (copyPath == null)
                copyPath = Path.GetTempFileName();

            File.Copy(assemblyPath, copyPath, true);

            // Load Blur.Library first.
            if (AssemblyResolver.References != null)
            {
                string blurLibraryPath = AssemblyResolver.References
                    .FirstOrDefault(x => x.EndsWith("Blur.Library.dll", StringComparison.OrdinalIgnoreCase));

                if (blurLibraryPath != null)
                    BlurLibrary = Assembly.LoadFrom(blurLibraryPath);
            }

            if (BlurLibrary == null)
            {
                try
                {
                    BlurLibrary = Assembly.Load(new AssemblyName("Blur.Library"));
                }
                catch
                {
                    string currentDir = Path.GetDirectoryName(typeof(Processor).Assembly.Location);
                    BlurLibrary = Assembly.LoadFrom(Path.Combine(currentDir, "Blur.Library.dll"));
                }
            }

            // Load the assembly to modify.
            TargetStream = File.Open(copyPath, FileMode.Open, FileAccess.ReadWrite);

            // If possible, load the symbols.
            string symbolsPath;

            if (File.Exists(symbolsPath = Path.ChangeExtension(assemblyPath, ".pdb")))
                SymbolStream = File.Open(symbolsPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);

            else if (File.Exists(symbolsPath = Path.ChangeExtension(symbolsPath, ".mdb")))
                SymbolStream = File.Open(symbolsPath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        }

        /// <summary>
        /// Dispose the Processor, saving its content.
        /// </summary>
        public static void Dispose()
        {
            if (!WillSave)
                return;

            TargetStream.Dispose();
        }

        /// <summary>
        /// Cancel the operation, discarding changes.
        /// </summary>
        public static void Cancel()
        {
            WillSave = false;

            Dispose();
        }

        /// <summary>
        /// Process the <see cref="Assembly"/> at the given path.
        /// </summary>
        /// <inheritdoc cref="FileStream(string, FileMode, FileAccess)" select="exception"/>
        public static void Process()
        {
            if (!HasBeenInitialized)
                throw new Exception("Please initialize the processor before calling Process.");

            // Load the assembly.
#if NET_CORE
            Assembly = AssemblyLoadContext.Default.LoadFromStream(TargetStream);
#else
            byte[] assemblyBytes = new byte[TargetStream.Length];

            TargetStream.Position = 0;
            TargetStream.Read(assemblyBytes, 0, assemblyBytes.Length);

            Assembly = Assembly.Load(assemblyBytes);
#endif
            // Initialize the remote processor.
            if (ProcessorType == null)
                ProcessorType = GetRemoteProcessor();

            // Process the assembly.
            ProcessAssembly();
        }

        /// <summary>
        /// Preprocess the target assembly.
        /// </summary>
        public static void Preprocess()
        {
            if (!HasBeenInitialized)
                throw new Exception("Please initialize the processor before calling Preprocess.");

            TargetStream.Position = 0;

            // Initialize the remote processor.
            if (ProcessorType == null)
                ProcessorType = GetRemoteProcessor();

            ProcessorType.GetMethod(PROCESSOR_PREPROCESS, BindingFlags.Static | BindingFlags.Public)
                         .Invoke(null, new object[] { TargetStream, SymbolStream });
        }
        #endregion

        #region Assembly Resolution
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
            string assemblyPath = Path.Combine(AssemblyDirectory, e.Name.Substring(0, e.Name.IndexOf(',')) + ".dll");

            if (File.Exists(assemblyPath))
                return Assembly.LoadFrom(assemblyPath);

            return AssemblyResolver.Resolve(e.Name);
        }
#endif
        #endregion

        #region Utils
        /// <summary>
        /// Returns the type of the remote processor, after initializing it.
        /// </summary>
        private static Type GetRemoteProcessor()
        {
            //foreach (var t in BlurLibrary.GetTypes())
            //    Console.WriteLine($"type: {t}");

            Type processor = BlurLibrary.GetType(PROCESSOR_FULLNAME, true);

            // Set the processor's "tunnel" functions.
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

            Console.WriteLine($"Enumerating fields from {processor}");
            foreach (var fld in processor.GetRuntimeFields())
                Console.WriteLine($"field: {fld}");

            processor.GetField(PROCESSOR_GETASSEMBLIES).SetValue(null, getAssemblies);
            processor.GetField(PROCESSOR_GETASSEMBLY).SetValue(null, getAssembly);
            processor.GetField(PROCESSOR_GETASSEMBLYSTREAM).SetValue(null, getAssemblyStream);
            processor.GetField(PROCESSOR_LOGMESSAGE).SetValue(null, new Action<string, bool>((msg, isWarning) =>
                     {
                         if (isWarning)
                             WarningLogged?.Invoke(msg);
                         else
                             MessageLogged?.Invoke(msg);
                     }));

            return processor;
        }

		/// <summary>
        /// Create a BlurAttribute, given its custom attribute data.
        /// </summary>
        private static object CreateAttribute(CustomAttributeData blurAttribute)
        {
            object[] args = new object[blurAttribute.ConstructorArguments.Count];
            for (int i = 0; i < args.Length; i++)
                args[i] = blurAttribute.ConstructorArguments[i].Value;

            Type attrType = blurAttribute.AttributeType;
            object attrObj = blurAttribute.Constructor.Invoke(args);

            if (blurAttribute.NamedArguments != null)
            {
                foreach (CustomAttributeNamedArgument namedArg in blurAttribute.NamedArguments)
                {
                    object value = namedArg.TypedValue.Value;

                    IReadOnlyList<CustomAttributeTypedArgument> typedArgs = value as IReadOnlyList<CustomAttributeTypedArgument>;
                    if (typedArgs != null)
                    {
                        Array array = (Array)Activator.CreateInstance(namedArg.TypedValue.ArgumentType, typedArgs.Count);

                        for (int i = 0; i < array.Length; i++)
                            array.SetValue(typedArgs[i].Value, i);

                        value = array;
                    }

                    if (namedArg.IsField)
                        attrType.GetField(namedArg.MemberName).SetValue(attrObj, value);
                    else
                        attrType.GetProperty(namedArg.MemberName).SetValue(attrObj, value);
                }
            }

            return attrObj;
        }

        /// <summary>
        /// Process the target assembly.
        /// </summary>
        private static void ProcessAssembly()
        {
            // Get the BlurAttribute set on the assembly.
            CustomAttributeData blurAttribute =
                Assembly.CustomAttributes.FirstOrDefault(x => x.AttributeType.FullName == ATTRIBUTE_FULLNAME);

            if (blurAttribute == null)
                throw new ArgumentException("The given assembly must be marked with the Blur attribute.");

            // Make the Blur attribute out of its data.
            object attrObj = CreateAttribute(blurAttribute);

            // Retrieve the BlurAttribute's TypeInfo, to access its declaring Assembly.
            if (BlurLibrary == null)
                BlurLibrary = blurAttribute.AttributeType.GetTypeInfo().Assembly;

            TargetStream.Position = 0;

            // Call Processor.Process();
            ProcessorType.GetMethod(PROCESSOR_METHOD, new[] { typeof(string), typeof(Assembly), attrObj.GetType(), typeof(Stream), typeof(Stream) })
                         .Invoke(null, new[] { TargetPath, Assembly, attrObj, TargetStream, SymbolStream });
        }
        #endregion
    }
}
