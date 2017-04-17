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
            // Fix bad formatting of the fullname:
            // Sometimes the "Version=, Culture=, PublicKeyToken=" part of the assembly
            // appears more than once in the full name; keep only the first part.
            for (int i = 0, commaCount = 0; i < fullName.Length; i++)
            {
                if (fullName[i] != ',')
                    continue;

                if (++commaCount != 4)
                    continue;

                fullName = fullName.Substring(0, i);
                break;
            }

            if (assembliesResolved.TryGetValue(fullName, out AssemblyDefinition assembly))
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
