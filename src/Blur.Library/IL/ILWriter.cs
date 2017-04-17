using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.ObjectModel;

namespace Blur
{
    /// <summary>
    /// Class used to fluently write IL code that is executable and compilable.
    /// <para>
    /// This class can also convert LINQ expressions and existing <see langword="methods"/>
    /// or <see langword="delegates"/> to IL.
    /// </para>
    /// </summary>
    [SuppressMessage("ReSharper", "ParameterHidesMember", Justification = "Both the field and the parameter have clear names.")]
    public sealed partial class ILWriter
    {
        private readonly Lazy<ReadOnlyCollection<Instruction>> readOnlyInstructions;

        internal readonly IList<Instruction> instructions;
        internal readonly IList<VariableDefinition> variables;
        internal readonly IList<ParameterDefinition> parameters;
        internal int position;

        /// <summary>
        /// Returns whether or not this <see cref="ILWriter"/>
        /// is at the end of the body being written.
        /// </summary>
        public bool AtEnd => position >= instructions.Count - 1;

        /// <summary>
        /// Returns the count of all instructions
        /// to write.
        /// </summary>
        public int Count => instructions.Count;

        /// <summary>
        /// Returns the offset at the current position.
        /// </summary>
        public int CurrentOffset => Count == 0 ? 0 : instructions[position].Offset;

        /// <summary>
        /// Enumerates all <see cref="Instruction"/>s printed
        /// by this <see cref="ILWriter"/>.
        /// </summary>
        public IReadOnlyList<Instruction> Instructions => readOnlyInstructions.Value;

        /// <summary>
        /// Gets the <see cref="Instruction"/> at the current <see cref="Position"/>.
        /// </summary>
        public Instruction Current => instructions[position];

        /// <summary>
        /// Gets or sets the position of this <see cref="ILWriter"/>
        /// in the body being created.
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Method whose body is being modified.
        /// </summary>
        public MethodDefinition Method { get; private set; }

        internal ILWriter(MethodDefinition method, bool clean)
        {
            this.Method = method;
            this.instructions = method.Body.Instructions;
            this.variables = method.Body.Variables;
            this.parameters = method.Parameters;

            this.readOnlyInstructions =
                new Lazy<ReadOnlyCollection<Instruction>>(() => new ReadOnlyCollection<Instruction>(instructions));

            if (clean || method.IsExtern())
                this.instructions.Clear();
        }

        #region To
        /// <summary>
        /// Go to the given <paramref name="position"/>,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter To(int position)
        {
            if (position > instructions.Count)
                throw new IndexOutOfRangeException();

            this.position = position;
            return this;
        }

        /// <summary>
        /// Go to the given <paramref name="instruction"/>,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter To(Instruction instruction) => this.To(this.IndexOf(instruction));

        /// <inheritdoc cref="To(int)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ILWriter Move(int position) => this.To(position);

        /// <inheritdoc cref="To(Instruction)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ILWriter Move(Instruction instruction) => this.To(instruction);

        /// <summary>
        /// Go to the end of the body being written,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter ToEnd() => this.To(instructions.Count);

        /// <summary>
        /// Go to the start of the body being written,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter ToStart() => this.To(0);

        /// <summary>
        /// Go to the last instruction of the body being written,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter ToLast() => this.To(instructions.Count - 1);
        #endregion

        #region Remove / Replace
        /// <summary>
        /// Removes the <see cref="Current"/> instruction,
        /// and returns <see langword="this"/>.
        /// </summary>
        public ILWriter Remove()
        {
            instructions.RemoveAt(position);

            if (position == instructions.Count)
                position--;

            return this;
        }

        /// <summary>
        /// Removes the given <paramref name="instruction"/>,
        /// and returns <see langword="this"/>.
        /// </summary>
        public ILWriter Remove(Instruction instruction)
        {
            instructions.Remove(instruction);

            if (position == instructions.Count)
                position--;

            return this;
        }

        /// <summary>
        /// Removes every instruction that matches <paramref name="predicate"/>,
        /// and returns <see langword="this"/>.
        /// </summary>
        public ILWriter Remove(Predicate<Instruction> predicate)
        {
            for (int i = 0; i < instructions.Count; i++)
            {
                if (predicate(instructions[i]))
                    instructions.RemoveAt(i--);
            }

            if (position >= instructions.Count)
                position = instructions.Count - 1;

            return this;
        }
        #endregion

        /// <summary>
        /// Copy the content of this <see cref="ILWriter"/> to
        /// an <see cref="ILGenerator"/>.
        /// </summary>
        internal void CopyTo(ILGenerator il)
        {
            for (int i = 0; i < instructions.Count; i++)
                instructions[i].Emit(il);
        }

        /// <summary>
        /// Copy the content of this <see cref="ILWriter"/> to
        /// a <see cref="MethodBody"/>.
        /// </summary>
        internal void CopyTo(MethodBody body)
        {
            Copy(this.Method.Body, body, body.Instructions.Count, null);
        }
    }
}
