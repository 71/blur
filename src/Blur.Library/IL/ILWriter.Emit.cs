using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// This list contains all methods that have been changed
        /// by an <see cref="ILWriter"/>.
        /// </summary>
        internal static readonly List<uint> ChangedMethods = new List<uint>();

        private bool hasChanged;

        private ILWriter Emit(Instruction instruction)
        {
            if (!hasChanged)
            {
                hasChanged = true;
                ChangedMethods.Add(Method.MetadataToken.RID);
            }

            this.instructions.Insert(position++, instruction);

            if (instruction.Offset == 0 && instruction.Previous != null)
            {
                instruction.FixOffset();
                for (int i = position; i < instructions.Count; i++)
                    instructions[i].FixOffset();
            }

            return this;
        }

        private ILWriter Emit(params Instruction[] ins)
        {
            for (int i = 0; i < ins.Length; i++)
                this.Emit(ins[i]);
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/>.
        /// </summary>
        public ILWriter Emit(OpCode opcode) => this.Emit(Instruction.Create(opcode));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, string str) => this.Emit(Instruction.Create(opcode, str));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, TypeReference operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, MethodReference operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, FieldReference operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ParameterDefinition operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, VariableDefinition operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, Instruction target) => this.Emit(Instruction.Create(opcode, target));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, CallSite callSite) => this.Emit(Instruction.Create(opcode, callSite));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, params Instruction[] targets) => this.Emit(Instruction.Create(opcode, targets));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, byte operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, sbyte operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, short operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ushort operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, int operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, uint operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, long operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ulong operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, float operand) => this.Emit(Instruction.Create(opcode, operand));

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, double operand) => this.Emit(Instruction.Create(opcode, operand));

        #region Converted
        /// <inheritdoc cref="Emit(OpCode, TypeReference)"/>
        public ILWriter Emit(OpCode opcode, Type operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, MethodReference)"/>
        public ILWriter Emit(OpCode opcode, MethodInfo operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, FieldReference)"/>
        public ILWriter Emit(OpCode opcode, FieldInfo operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, ParameterDefinition)"/>
        public ILWriter Emit(OpCode opcode, ParameterInfo operand) => this.Emit(opcode, operand.GetDefinition());
        #endregion
    }
}
