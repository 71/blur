using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    /// <summary>
    /// Provides access to local members of a rewritten method.
    /// <para>
    /// Methods in this class should only be called in a <see langword="delegate"/>
    /// passed to <see cref="ILWriter.Delegate(Action)"/> and other overloads.
    /// </para>
    /// </summary>
    public static class Context
    {
        #region Classes
        public abstract class Any
        {
            public virtual Type ReturnType => typeof(void);
            public abstract Instruction[] GetInstructions(ILWriter il);
        }

        public abstract class Any<T> : Any
        {
            public sealed override Type ReturnType => typeof(T);

            public static implicit operator Any<T>(T val) => new Constant<T>(val);
        }

        private sealed class Constant<T> : Any<T>
        {
            private readonly T Value;

            internal Constant(T val)
            {
                if (!ILWriter.IsConstant<T>())
                    throw new InvalidOperationException($"Cannot convert {typeof(T)} to a constant.");

                Value = val;
            }

            public override Instruction[] GetInstructions(ILWriter il)
            {
                return ILWriter.InstructionsForConstant(Value);
            }
        }

        private sealed class Arg<T> : Any<T>
        {
            private readonly int NthArgument;
            private readonly ParameterDefinition Parameter;

            internal Arg(int nth)
            {
                NthArgument = nth;
            }

            internal Arg(ParameterDefinition parameter)
            {
                Parameter = parameter;
            }

            public override Instruction[] GetInstructions(ILWriter il)
            {
                return new[] { Instruction.Create(OpCodes.Ldarg, Parameter ?? il.parameters[NthArgument]) };
            }
        }

        private sealed class Var<T> : Any<T>
        {
            private readonly int NthVariable;
            private readonly VariableDefinition Variable;

            internal Var(int nth)
            {
                NthVariable = nth;
            }

            internal Var(VariableDefinition variable)
            {
                Variable = variable;
            }

            public override Instruction[] GetInstructions(ILWriter il)
            {
                return new[] { Instruction.Create(OpCodes.Ldarg, Variable ?? il.variables[NthVariable]) };
            }
        }

        private sealed class Arg0<T> : Any<T>
        {
            public override Instruction[] GetInstructions(ILWriter il)
            {
                return new[] { Instruction.Create(OpCodes.Ldarg_0) };
            }
        }

        private sealed class Instr<T> : Any<T>
        {
            private readonly Instruction[] Instructions;

            internal Instr(Instruction[] toPrint)
            {
                Instructions = toPrint;
            }

            public override Instruction[] GetInstructions(ILWriter il)
            {
                return Instructions;
            }
        }

        private sealed class Instr : Any
        {
            private readonly Instruction[] Instructions;

            internal Instr(Instruction[] toPrint)
            {
                Instructions = toPrint;
            }

            public override Instruction[] GetInstructions(ILWriter il)
            {
                return Instructions;
            }
        }
        #endregion

        #region This
        /// <inheritdoc cref="This"/>
        /// <typeparam name="T">The declaring type of the method to rewrite.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<T> This<T>() => new Arg0<T>();

        /// <summary>
        /// In an instance method, returns <see langword="this"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<object> This() => new Arg0<object>();
        #endregion

        #region Argument
        /// <inheritdoc cref="Argument(int)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<T> Argument<T>(int position) => new Arg<T>(position);

        /// <summary>
        /// Gets the argument at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the argument.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<object> Argument(int position) => new Arg<object>(position);

        /// <inheritdoc cref="Argument(ParameterDefinition)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<T> Argument<T>(ParameterDefinition parameter) => new Arg<T>(parameter);

        /// <summary>
        /// Gets the argument representend by the given <paramref name="parameter"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<object> Argument(ParameterDefinition parameter) => new Arg<object>(parameter);
        #endregion

        #region Variable
        /// <inheritdoc cref="Variable(int)"/>
        /// <typeparam name="T">The type of the variable.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<T> Variable<T>(int position) => new Var<T>(position);

        /// <summary>
        /// Gets the variable at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the variable.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<object> Variable(int position) => new Var<object>(position);

        /// <inheritdoc cref="Variable(VariableDefinition)"/>
        /// <typeparam name="T">The type of the variable.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<T> Variable<T>(VariableDefinition variable) => new Var<T>(variable);

        /// <summary>
        /// Gets the variable represented by the given <paramref name="variable"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static Any<object> Variable(VariableDefinition variable) => new Var<object>(variable);
        #endregion

        #region Instructions
        /// <summary>
        /// Prints the given instructions.
        /// </summary>
        public static Any<T> Instructions<T>(params Instruction[] instructions) => new Instr<T>(instructions);

        /// <summary>
        /// Prints the given instructions.
        /// </summary>
        public static Any Instructions(params Instruction[] instructions) => new Instr(instructions);
        #endregion
    }
}
