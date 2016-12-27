using System;
using System.Runtime.CompilerServices;

namespace Blur
{
    /// <summary>
    /// Provides access to local members of a rewritten
    /// method.
    /// <para>
    /// Methods in this class should only be called in a <see langword="delegate"/>
    /// passed to <see cref="ILWriter.Body(Delegate)"/>.
    /// </para>
    /// </summary>
    public static class Context
    {
        private static readonly string ErrorString = "This method should only be called in a delegate passed to"
                                                     + nameof(ILWriter) + '.' + nameof(ILWriter.Body);

        private static T Throw<T>()
        {
            throw new NotSupportedException(ErrorString);
        }

        #region This
        /// <inheritdoc cref="This"/>
        /// <typeparam name="T">The declaring type of the method to rewrite.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T This<T>() => Throw<T>();

        /// <summary>
        /// In an instance method, returns <see langword="this"/>.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object This() => Throw<object>();
        #endregion

        #region Argument
        /// <inheritdoc cref="Argument(int)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Argument<T>(int position) => Throw<T>();

        /// <inheritdoc cref="Argument(string)"/>
        /// <typeparam name="T">The type of the argument.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Argument<T>(string argName) => Throw<T>();

        /// <summary>
        /// Gets the argument at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the argument.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object Argument(int position) => Throw<object>();

        /// <summary>
        /// Gets the argument whose name is <paramref name="argName"/>.
        /// </summary>
        /// <param name="argName">The name of the argument.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object Argument(string argName) => Throw<object>();
        #endregion

        #region Variable
        /// <inheritdoc cref="Variable(int)"/>
        /// <typeparam name="T">The type of the variable.</typeparam>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Variable<T>(int position) => Throw<T>();

        /// <inheritdoc cref="Variable(string)"/>
        /// <param name="varName">The name of the variable.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static T Variable<T>(string varName) => Throw<T>();

        /// <summary>
        /// Gets the variable at the given <paramref name="position"/>.
        /// </summary>
        /// <param name="position">The 0-based index of the variable.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object Variable(int position) => Throw<object>();

        /// <summary>
        /// Gets the variable whose name is <paramref name="varName"/>.
        /// </summary>
        /// <param name="varName">The name of the variable.</param>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static object Variable(string varName) => Throw<object>();
        #endregion
    }
}
