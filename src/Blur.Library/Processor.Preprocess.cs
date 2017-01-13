using System;
using System.ComponentModel;
using System.IO;
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
            AssemblyDefinition assembly = ReadAssembly(targetStream);

            FixExternals(assembly);

            SaveAssembly(assembly, targetStream);
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
                    if (method.RVA == 0)
                    {
                        method.Body.Instructions.Add(Instruction.Create(OpCodes.Newobj, emeCtor));
                        method.Body.Instructions.Add(Instruction.Create(OpCodes.Throw));
                    }
                    else
                    {
                        // Not really the point of the method, but putting this here avoids
                        // making another loop.
                        CustomAttribute debugAttrData = method.GetAttribute<DebugAttribute>();

                        if (debugAttrData == null)
                            continue;

                        // Should the feature be needed, I could make this feature available
                        // to all weavers. But right now, only the Debug attribute needs it.
                        DebugAttribute debugAttr = debugAttrData.CreateInstance<DebugAttribute>();
                        debugAttr.Apply(method);
                    }
                }
            }
        }
    }
}
