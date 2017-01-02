using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    /// <summary>
    /// Exception thrown when an <see langword="extern"/> method is called.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public sealed class ExternalMethodException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="ExternalMethodException"/>.
        /// </summary>
        public ExternalMethodException() : base("This external method declares no body.")
        {
        }
    }

    partial class Processor
    {
        /// <summary>
        /// Prepares an assembly for processing.
        /// </summary>
        public static void Preprocess(Stream targetStream)
        {
            targetStream.Position = 0;

            AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(targetStream, new ReaderParameters
            {
                InMemory = true,
                ReadWrite = true
            });

            FixExternals(assembly);

            using (MemoryStream ms = new MemoryStream())
            {
                assembly.Write(ms);
                targetStream.SetLength(0);

                ms.Position = 0;
                ms.CopyTo(targetStream);
            }
        }

        /// <summary>
        /// Gives a body to all <see langword="extern"/> methods in an assembly.
        /// </summary>
        private static void FixExternals(AssemblyDefinition assembly)
        {
            TypeReference emeType = assembly.MainModule.GetType("Blur.ExternalMethodException", true);
            MethodReference emeCtor = new MethodReference(".ctor", assembly.MainModule.TypeSystem.Void, emeType);

            emeCtor = assembly.MainModule.ImportReference(emeCtor);

            foreach (TypeDefinition type in assembly.MainModule.GetTypes())
            {
                foreach (MethodDefinition method in type.Methods)
                {
                    if (method.RVA != 0)
                        continue;

                    method.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, emeCtor));
                    method.Body.Instructions.Add(Instruction.Create(OpCodes.Throw));
                }
            }
        }
    }
}
