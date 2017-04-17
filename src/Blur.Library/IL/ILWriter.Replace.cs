using System;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Replaces the given <paramref name="instruction"/> by other
        /// instructions.
        /// </summary>
        public ILWriter Replace(Instruction instruction, params Instruction[] newInstructions)
            => this.Replace(this.IndexOf(instruction), newInstructions);

        /// <summary>
        /// Replaces the instruction at the given position by other instructions.
        /// </summary>
        public ILWriter Replace(int instructionPos, params Instruction[] newInstructions)
        {
            instructions.RemoveAt(instructionPos);

            // Insert every new instruction to its new place.
            for (int i = 0; i < newInstructions.Length; i++)
                instructions.Insert(instructionPos + i, newInstructions[i]);

            // If the ILWriter's position is after the changes, the position should change.
            if (position > instructionPos)
                position += newInstructions.Length;

            return this;
        }
    }
}
