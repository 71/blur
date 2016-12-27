﻿using System;
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
    public sealed partial class ILWriter
    {
        private readonly Lazy<ReadOnlyCollection<Instruction>> readOnlyInstructions;

        internal readonly IList<Instruction> instructions;
        internal readonly List<VariableDefinition> variables;
        internal int position;

        /// <summary>
        /// Returns whether or not this <see cref="ILWriter"/>
        /// is at the end of the body being written.
        /// </summary>
        public bool AtEnd => position == instructions.Count - 1;

        /// <summary>
        /// Returns the count of all instructions
        /// to write.
        /// </summary>
        public int Count => instructions.Count;

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
        public int Position
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set;
        }

        /// <summary>
        /// Method whose body is being modified.
        /// </summary>
        public MethodDefinition Method { get; private set; }

        internal ILWriter(MethodDefinition method, bool clean)
        {
            this.instructions = method.Body.Instructions;
            this.readOnlyInstructions =
                new Lazy<ReadOnlyCollection<Instruction>>(() => new ReadOnlyCollection<Instruction>(instructions));
            this.variables = new List<VariableDefinition>();
            this.Method = method;

            if (clean)
                this.instructions.Clear();
        }

        /// <summary>
        /// Go to the given <paramref name="position"/>,
        /// and return <see langword="this"/>.
        /// </summary>
        [SuppressMessage("ReSharper", "ParameterHidesMember", Justification = "Both the field and the parameter have clear names.")]
        public ILWriter To(int position)
        {
            if (position >= instructions.Count)
                throw new IndexOutOfRangeException();

            this.position = position;
            return this;
        }

        /// <summary>
        /// Go to the end of the body being written,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter ToEnd() => this.To(instructions.Count - 1);

        /// <summary>
        /// Go to the start of the body being written,
        /// and return <see langword="this"/>.
        /// </summary>
        public ILWriter ToStart() => this.To(0);

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
            for (int i = 0; i < instructions.Count; i++)
                body.Instructions.Add(instructions[i]);
        }
    }
}
