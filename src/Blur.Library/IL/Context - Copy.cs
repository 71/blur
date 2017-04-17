using System;
using System.Runtime.CompilerServices;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Linq;

namespace Blur
{
    /// <summary>
    /// Provides access to local members of a rewritten method.
    /// <para>
    /// Methods in this class should only be called in a <see langword="delegate"/>
    /// passed to <see cref="ILWriter.Delegate(Action)"/> and other overloads.
    /// </para>
    /// </summary>
    internal static class __Context
    {
        #region Classes
        internal interface IContextObj
        {
            /// <summary>
            /// Gets the type of the value that'll be returned.
            /// </summary>
            Type ObjectType { get; }

            /// <summary>
            /// 
            /// </summary>
            Instruction GetInstruction(ILWriter il);
        }

        /// <summary>
        /// Represents a local argument (or parameter).
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        public struct ContextArgument<T> : IContextObj
        {
            internal int NthArgument;
            internal string ArgumentName;

            /// <summary>
            /// Returns the value of the argument.
            /// </summary>
            public T Value;

            /// <inheritdoc/> 
            public Type ObjectType => typeof(T);

            /// <inheritdoc cref="Value"/> 
            public static implicit operator T(ContextArgument<T> obj) => obj.Value;

            internal ContextArgument(int nth)
            {
                NthArgument = nth;
                ArgumentName = null;
                Value = default(T);
            }

            internal ContextArgument(string name)
            {
                NthArgument = -1;
                ArgumentName = name;
                Value = default(T);
            }

            internal ContextArgument<T> Verify()
            {
                if (!ILWriter.IsConstant<T>())
                    throw new InvalidOperationException($"Cannot convert {typeof(T)} to a constant.");
                return this;
            }

            /// <inheritdoc/> 
            public Instruction GetInstruction(ILWriter il)
            {
                string name = ArgumentName;
                ParameterDefinition parameter = ArgumentName == null
                    ? il.parameters[NthArgument]
                    : il.parameters.First(x => x.Name == name);

                Instruction ins = Instruction.Create(OpCodes.Ldarg, parameter);
                ILWriter.FixLdargFor(parameter, ins);

                return ins;
            }
        }

        /// <summary>
        /// Represents a local variable.
        /// </summary>
        /// <typeparam name="T">The type of the variable.</typeparam>
        public struct ContextVariable<T> : IContextObj
        {
            internal int NthVariable;

            /// <summary>
            /// Returns the value of the local variable.
            /// </summary>
            public T Value;

            /// <inheritdoc/> 
            public Type ObjectType => typeof(T);

            /// <inheritdoc cref="Value"/>
            public static implicit operator T(ContextVariable<T> obj) => obj.Value;

            internal ContextVariable(int nth)
            {
                NthVariable = nth;
                Value = default(T);
            }

            internal ContextVariable<T> Verify()
            {
                if (!ILWriter.IsConstant<T>())
                    throw new InvalidOperationException($"Cannot convert {typeof(T)} to a constant.");
                return this;
            }

            /// <inheritdoc/> 
            public Instruction GetInstruction(ILWriter il)
            {
                switch (NthVariable)
                {
                    case 0:
                        return Instruction.Create(OpCodes.Ldloc_0);
                    case 1:
                        return Instruction.Create(OpCodes.Ldloc_1);
                    case 2:
                        return Instruction.Create(OpCodes.Ldloc_2);
                    case 3:
                        return Instruction.Create(OpCodes.Ldloc_3);
                    default:
                        return Instruction.Create(NthVariable > sbyte.MaxValue ? OpCodes.Ldloc : OpCodes.Ldloc_S, il.variables[NthVariable]);
                }
            }
        }

        /// <summary>
        /// Represents <see langword="this"/>.
        /// </summary>
        /// <typeparam name="T">The declaring type of the method in which the context is retrieved.</typeparam>
        public struct ContextThis<T> : IContextObj
        {
            /// <summary>
            /// Returns the value of the current object.
            /// </summary>
            public T Value;

            /// <inheritdoc/>
            public Type ObjectType => typeof(T);

            /// <inheritdoc cref="Value"/>
            public static implicit operator T(ContextThis<T> obj) => obj.Value;

            internal ContextThis<T> Verify()
            {
                if (!ILWriter.IsConstant<T>())
                    throw new InvalidOperationException($"Cannot convert {typeof(T)} to a constant.");
                return this;
            }

            /// <inheritdoc/>
            public Instruction GetInstruction(ILWriter il)
            {
                return Instruction.Create(OpCodes.Ldarg_0);
            }
        }
        #endregion

        #region This
        /// <inheritdoc cref="This"/>
        /// <typeparam name="T">The declaring type of the method to rewrite.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextThis<T> This<T>() => new ContextThis<T>().Verify();

        /// <summary>
        /// In an instance method, returns <see langword="this"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextThis<object> This() => new ContextThis<object>();
        #endregion

        #region Argument
        /// <inheritdoc cref="Argument(int)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextArgument<T> Argument<T>(int position) => new ContextArgument<T>(position).Verify();

        /// <inheritdoc cref="Argument(string)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextArgument<T> Argument<T>(string argName) => new ContextArgument<T>(argName).Verify();

        /// <summary>
        /// Gets the argument at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the argument.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextArgument<object> Argument(int position) => new ContextArgument<object>(position);

        /// <summary>
        /// Gets the argument whose name is <paramref name="argName"/>.
        /// </summary>
        /// <param name="argName">The name of the argument.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextArgument<object> Argument(string argName) => new ContextArgument<object>(argName);
        #endregion

        #region Variable
        /// <inheritdoc cref="Variable(int)"/>
        /// <typeparam name="T">The type of the variable.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextVariable<T> Variable<T>(int position) => new ContextVariable<T>(position).Verify();

        /// <summary>
        /// Gets the variable at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the variable.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ContextVariable<object> Variable(int position) => new ContextVariable<object>(position);
        #endregion
    }
}
