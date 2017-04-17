using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is <see langword="true"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter If()
        {
            return this.PushBlock(block =>
            {
                OpCode opcode = position < byte.MaxValue ? OpCodes.Brfalse_S : OpCodes.Brfalse;

                block.Start.OpCode = opcode;
                block.Start.Operand = instructions[position++];
            });
        }

        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is <see langword="false"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter Unless()
        {
            return this.PushBlock(block =>
            {
                OpCode opcode = position < byte.MaxValue ? OpCodes.Brtrue_S : OpCodes.Brtrue;

                block.Start.OpCode = opcode;
                block.Start.Operand = instructions[position++];
            });
        }

        /// <summary>
        /// Push <see langword="true"/> if the value at the top of the stack is <see langword="false"/>.
        /// Otherwise, push <see langword="true"/>.
        /// </summary>
        public ILWriter Not() => this.Emit(OpCodes.Not);

        #region Shortcuts
        /// <summary>
        /// Execute the following instructions if the comparison
        /// made by the given <paramref name="opcode"/> returns <see langword="true"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter If(OpCode opcode) => this.Emit(opcode).If();

        /// <summary>
        /// Execute the following instructions if the comparison
        /// made by the given <paramref name="opcode"/> returns <see langword="false"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter Unless(OpCode opcode) => this.Emit(opcode).Unless();

        /// <summary>
        /// Execute the following instructions if the values
        /// at the top of the stack are equal.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter IfEqual() => this.If(OpCodes.Ceq);

        /// <summary>
        /// Execute the following instructions if the values
        /// at the top of the stack are not equal.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter UnlessEqual() => this.Unless(OpCodes.Ceq);

        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is <see langword="null"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter IfNull() => this.Null().If(OpCodes.Ceq);

        /// <summary>
        /// Execute the following instructions unless the value
        /// at the top of the stack is <see langword="null"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter UnlessNull() => this.Null().Unless(OpCodes.Ceq);

        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is greater than its
        /// preceeding value.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter IfGreaterThan() => this.If(OpCodes.Cgt);

        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is lower than its
        /// preceeding value.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter IfLowerThan() => this.If(OpCodes.Clt);
        #endregion
    }
}
