using System;
using System.Collections.Generic;

namespace Blur
{
    #region Enums
    /// <summary>
    /// The type of reposition that <see cref="ILWriter.Reposition(RepositionType)"/>
    /// will execute.
    /// </summary>
    public enum RepositionType
    {
        /// <summary>
        /// Reposition the <see cref="ILWriter"/> back where
        /// the stack was empty.
        /// </summary>
        EmptyStack
    }

    /// <summary>
    /// The type of reposition that <see cref="ILWriter.Reposition(CountedRepositionType,int)"/>
    /// will execute.
    /// </summary>
    public enum CountedRepositionType
    {
        /// <summary>
        /// Reposition the <see cref="ILWriter"/> where n
        /// more values are on top of the stack.
        /// </summary>
        MoreValues,

        /// <summary>
        /// Reposition the <see cref="ILWriter"/> where n
        /// less values are on top of the stack.
        /// </summary>
        LessValues
    }
    #endregion

    partial class ILWriter
    {
        #region RepositionState
        private struct RepositionState
        {
            /// <summary>Count of <see cref="Instructions"/> before repositioning.</summary>
            public readonly int Length;
            /// <summary><see cref="ILWriter.Position"/> before repositioning.</summary>
            public readonly int Position;

            public RepositionState(ILWriter il)
            {
                this.Length = il.instructions.Count;
                this.Position = il.position;
            }
        }

        private readonly Stack<RepositionState> repositions = new Stack<RepositionState>();

        /// <summary>
        /// Reposition the <see cref="ILWriter"/> back to its previous position.
        /// <para>
        /// Call this method after calling <see cref="Reposition(RepositionType)"/>
        /// or <see cref="Reposition(CountedRepositionType, int)"/> to go back to your previous position.
        /// </para>
        /// <para>
        /// This method will take care of the changes you may have made before calling it.
        /// </para>
        /// </summary>
        public ILWriter Reposition()
        {
            if (repositions.Count == 0)
                throw new Exception("Cannot reposition now.");

            RepositionState data = repositions.Pop();
            position = data.Position + (instructions.Count - data.Length);
            return this;
        }
        #endregion

        /// <summary>
        /// Reposition the <see cref="ILWriter"/> to a given state.
        /// </summary>
        public ILWriter Reposition(RepositionType type)
        {
            repositions.Push(new RepositionState(this));

            int currentValuesOnStack = this.instructions.CountValuesOnStack(0, position);

            switch (type)
            {
                case RepositionType.EmptyStack:
                    while (currentValuesOnStack > 0)
                    {
                        position--;
                        currentValuesOnStack += instructions[position].AddedToStack();
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return this;
        }

        /// <summary>
        /// Reposition the <see cref="ILWriter"/> to a given state.
        /// </summary>
        /// <exception cref="InvalidOperationException">Cannot execute reposition operation.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Invalid <see cref="CountedRepositionType"/>.</exception>
        public ILWriter Reposition(CountedRepositionType type, int n)
        {
            repositions.Push(new RepositionState(this));
            
            int initialValuesOnStack = this.instructions.CountValuesOnStack(0, position);
            int currentValuesOnStack = initialValuesOnStack;

            if (type == CountedRepositionType.LessValues)
                n = -n;

            switch (type)
            {
                case CountedRepositionType.LessValues:
                case CountedRepositionType.MoreValues:
                    int change = type == CountedRepositionType.LessValues ? -1 : 1;

                    while (currentValuesOnStack != initialValuesOnStack + n)
                    {
                        position += change;

                        if (position < 0 || position > instructions.Count)
                        {
                            Reposition();
                            throw new InvalidOperationException("Cannot reposition: not enough / too many elements on stack.");
                        }

                        currentValuesOnStack += instructions[position].AddedToStack();
                    }

                    break;
                default:
                    Reposition();
                    throw new ArgumentOutOfRangeException(nameof(type));
            }

            return this;
        }
    }
}
