using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Blur.Extensions;

namespace Blur
{
    partial class ILWriter
    {
        #region Variables
        /// <summary>
        /// Define a local variable.
        /// </summary>
        public VariableDefinition Define(TypeReference type)
        {
            VariableDefinition variable = new VariableDefinition(type);
            variables.Add(variable);
            return variable;
        }

        /// <summary>
        /// Define a local variable.
        /// </summary>
        public VariableDefinition Define(Type type) => Define(type.GetDefinition());

        /// <summary>
        /// Define a local variable.
        /// </summary>
        public VariableDefinition Define<T>() => this.Define(typeof(T));
        #endregion

        #region ForEach
        /// <summary>
        /// Executes the given <paramref name="action"/> on every
        /// <see cref="Instruction"/> in this <see cref="ILWriter"/>,
        /// then returns <see langword="this"/>.
        /// </summary>
        /// <param name="action">The action to execute on every <see cref="Instruction"/>.</param>
        /// <param name="keepPositionUpdated">If <see langword="true"/>, <see cref="Position"/> will have its value set to this loop's position.</param>
        /// <remarks>
        /// This method handles changes made to the method body
        /// this writer represents, unlike a traditional <see langword="foreach"/> loop.
        /// </remarks>
        public ILWriter ForEach(Action<Instruction> action, bool keepPositionUpdated = true)
        {
            if (keepPositionUpdated)
            {
                position = 0;
                this.ForEachInternal(action, ref position);
            }
            else
            {
                int i = 0;
                this.ForEachInternal(action, ref i);
            }

            return this;
        }

        private void ForEachInternal(Action<Instruction> action, ref int pos)
        {
            for (; pos < instructions.Count; pos++)
            {
                int countBefore = instructions.Count;
                action(instructions[pos]);
                pos += instructions.Count - countBefore;
            }
        }
        #endregion

        #region Cast
        /// <summary>
        /// Casts the value at the top of the stack to <paramref name="type"/>.
        /// </summary>
        public ILWriter Cast(TypeReference type)
        {
            return type.IsValueType ? this.Unbox_Any(type) : this.Castclass(type);
        }

        /// <summary>
        /// Casts the value at the top of the stack to <typeparamref name="T"/>.
        /// </summary>
        public ILWriter Cast<T>() => this.Cast(typeof(T).GetReference());

        /// <summary>
        /// Casts the value at the top of the stack to <paramref name="type"/>.
        /// </summary>
        public ILWriter Cast(Type type) => this.Cast(type.GetReference());
        #endregion
    }
}
