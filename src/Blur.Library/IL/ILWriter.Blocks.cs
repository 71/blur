using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil.Cil;

namespace Blur
{
    [SuppressMessage("ReSharper", "ParameterHidesMember", Justification = "Both the field and the parameter have clear names.")]
    partial class ILWriter
    {
        private readonly Stack<Block> blocks = new Stack<Block>();

        /// <summary>
        /// Class that stores the current block (or scope)
        /// in which we are.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
        private struct Block
        {
            public readonly Instruction Start;
            public readonly Action<Block> Ended;

            public Block(Instruction start, Action<Block> ended)
            {
                this.Start = start;
                this.Ended = ended;
            }

            public void End()
            {
                Ended(this);
            }
        }

        /// <summary>
        /// Pushes a new <see cref="Block"/>, given the delegate
        /// that'll be invoked once it ends.
        /// </summary>
        private ILWriter PushBlock(Action<Block> ended)
        {
            Instruction nop = Instruction.Create(OpCodes.Nop);
            blocks.Push(new Block(nop, ended));
            return this.Emit(nop);
        }

        /// <summary>
        /// Sets the writer's position to the specified <paramref name="instruction"/>'s,
        /// and begins a new block.
        /// <para>
        /// You can then go back to the previous position by calling <see cref="End"/>.
        /// </para>
        /// </summary>
        public ILWriter Go(Instruction instruction) => this.Go(this.IndexOf(instruction));

        /// <summary>
        /// Sets the writer's position to the specified <paramref name="position"/>,
        /// and begins a new block.
        /// <para>
        /// You can then go back to the previous position by calling <see cref="End"/>.
        /// </para>
        /// </summary>
        public ILWriter Go(int position) => this.PushBlock(block =>
        {
            this.Move(block.Start);
        }).To(position);

        /// <summary>
        /// End the block of a <see cref="If()"/> or <see cref="Unless()"/> call.
        /// </summary>
        public ILWriter End()
        {
            if (blocks.Count == 0)
                throw new Exception("There is no block to end.");

            blocks.Pop().End();
            return this;
        }
    }
}
