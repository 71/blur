using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        private ILWriter Emit(Instruction instruction)
        {
            instructions.Insert(position++, instruction);
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/>.
        /// </summary>
        public ILWriter Emit(OpCode opcode)
        {
            instructions.Insert(position++, Instruction.Create(opcode));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, string str)
        {
            instructions.Insert(position++, Instruction.Create(opcode, str));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, TypeReference operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, MethodReference operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, FieldReference operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ParameterDefinition operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, VariableDefinition operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, Instruction target)
        {
            instructions.Insert(position++, Instruction.Create(opcode, target));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, CallSite callSite)
        {
            instructions.Insert(position++, Instruction.Create(opcode, callSite));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, params Instruction[] targets)
        {
            instructions.Insert(position++, Instruction.Create(opcode, targets));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, byte operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, sbyte operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, short operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ushort operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, int operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, uint operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, long operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, ulong operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, float operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        /// <summary>
        /// Print an <see cref="OpCode"/> with its operand.
        /// </summary>
        public ILWriter Emit(OpCode opcode, double operand)
        {
            instructions.Insert(position++, Instruction.Create(opcode, operand));
            return this;
        }

        #region Converted
        /// <inheritdoc cref="Emit(OpCode, TypeDefinition)"/>
        public ILWriter Emit(OpCode opcode, Type operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, MethodDefinition)"/>
        public ILWriter Emit(OpCode opcode, MethodInfo operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, FieldDefinition)"/>
        public ILWriter Emit(OpCode opcode, FieldInfo operand) => this.Emit(opcode, operand.GetReference());

        /// <inheritdoc cref="Emit(OpCode, ParameterDefinition)"/>
        public ILWriter Emit(OpCode opcode, ParameterInfo operand) => this.Emit(opcode, operand.GetDefinition());
        #endregion
    }
}
