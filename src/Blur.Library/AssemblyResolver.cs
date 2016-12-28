using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace Blur
{
    internal class AssemblyResolver : IAssemblyResolver
    {
        private readonly Dictionary<string, AssemblyDefinition> assembliesResolved = new Dictionary<string, AssemblyDefinition>();

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(string fullName)
        {
            return Resolve(fullName, new ReaderParameters());
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name, new ReaderParameters());
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            AssemblyDefinition assembly;
            if (assembliesResolved.TryGetValue(fullName, out assembly))
                return assembly;

            Stream targetStream = Processor.GetAssemblyStream(fullName);

            if (targetStream == null)
                return null;

            try
            {
                assembly = AssemblyDefinition.ReadAssembly(targetStream, new ReaderParameters
                {
                    InMemory = true,
                    ReadWrite = false,
                    ReadingMode = ReadingMode.Immediate
                });

                this.assembliesResolved.Add(fullName, assembly);
                return assembly;
            }
            catch
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            return Resolve(name.FullName, parameters);
        }
        
        public void Dispose()
        {
            // Nothing to dispose.
        }
    }
}
