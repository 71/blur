using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Tests
{
    /// <summary>
    /// Forces the marked method to block its execution before
    /// returning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    internal class BlockMethodAttribute : Attribute, IMethodWeaver
    {
        /// <summary>
        /// Prepends <see cref="Console.ReadLine"/> before all
        /// <see langword="ret"/> op codes.
        /// </summary>
        public void Apply(MethodDefinition method)
        {
            ILWriter writer = method.Write();

            writer.ForEach(ins =>
            {
                if (ins.OpCode == OpCodes.Ret)
                    writer.Before(ins).Call(typeof(Console).GetMethod(nameof(Console.ReadLine))).Pop();
            });
        }
    }
}
