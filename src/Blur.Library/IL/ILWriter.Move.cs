using System;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Position the builder before the given <paramref name="instruction"/>,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter Before(Instruction instruction)
        {
            int index = instructions.IndexOf(instruction);
            if (index == -1)
                throw new ArgumentException("The given instruction does not exist in this builder.", nameof(instruction));

            return this.To(index);
        }

        /// <summary>
        /// Position the builder after the given <paramref name="instruction"/>,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter After(Instruction instruction)
        {
            int index = instructions.IndexOf(instruction);
            if (index == -1)
                throw new ArgumentException("The given instruction does not exist in this builder.", nameof(instruction));

            return this.To(index + 1);
        }
    }
}
