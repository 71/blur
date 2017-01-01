using System.ComponentModel;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    /// <summary>
    /// Defines a set of extension methods used to
    /// work with Blur.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static partial class BlurExtensions
    {
        /// <summary>
        /// Return a new <see cref="ILWriter"/> for the given <paramref name="method"/>.
        /// </summary>
        public static ILWriter Write(this MethodDefinition method) => new ILWriter(method, false);

        /// <summary>
        /// Return a new <see cref="ILWriter"/> for the given <paramref name="method"/>.
        /// </summary>
        public static ILWriter Rewrite(this MethodDefinition method) => new ILWriter(method, true);

        /// <summary>
        /// Return a new <see cref="ILWriter"/> for the given <paramref name="body"/>.
        /// </summary>
        public static ILWriter Write(this MethodBody body) => new ILWriter(body.Method, false);

        /// <summary>
        /// Return a new <see cref="ILWriter"/> for the given <paramref name="body"/>.
        /// </summary>
        public static ILWriter Rewrite(this MethodBody body) => new ILWriter(body.Method, true);

        /// <summary>
        /// Returns whether or not the given <paramref name="property"/> is static.
        /// </summary>
        public static bool IsStatic(this PropertyDefinition property)
            => property.GetMethod?.IsStatic ?? property.SetMethod?.IsStatic ?? false;
    }
}
