using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        private Block currentBlock;

        private class Block
        {
            public Block Parent { get; private set; }
            public int Depth => IsRoot ? 0 : Parent.Depth + 1;
            public bool IsRoot => Parent == null;

            public int Start { get; private set; }
            public bool IsInversed { get; private set; }

            public Block(Block parent, int position, bool inversed)
            {
                this.Parent = parent;
                this.Start = position;
                this.IsInversed = inversed;
            }
        }

        /// <summary>
        /// Execute the following instructions if the value
        /// at the top of the stack is <see langword="true"/>.
        /// <para>
        /// The instructions are executed until <see cref="End"/> is called.
        /// </para>
        /// </summary>
        public ILWriter If()
        {
            currentBlock = new Block(currentBlock, position, false);
            return this;
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
            currentBlock = new Block(currentBlock, position, true);
            return this;
        }

        /// <summary>
        /// End the block of a <see cref="If()"/> or <see cref="Unless()"/> call.
        /// </summary>
        public ILWriter End()
        {
            if (currentBlock == null)
                throw new System.Exception("End can only be called following If or Unless.");

            OpCode opcode = !currentBlock.IsInversed
                ? (position < byte.MaxValue ? OpCodes.Brfalse_S : OpCodes.Brfalse)
                : (position < byte.MaxValue ? OpCodes.Brtrue_S : OpCodes.Brtrue);

            instructions.Insert(currentBlock.Start, Instruction.Create(opcode, instructions[position++]));
            currentBlock = currentBlock.Parent;
            return this;
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
