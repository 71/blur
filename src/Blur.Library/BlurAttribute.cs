using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Blur
{
    /// <summary>
    /// Attribute that defines settings for Blur, and allows it to
    /// be modified by the Blur processor.
    /// <para>
    /// An <see cref="Assembly"/> MUST be marked with this attribute to be modified by Blur.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class BlurAttribute : Attribute
    {
        /// <inheritdoc cref="BlurAttribute"/>
        [SuppressMessage("ReSharper", "EmptyConstructor", Justification = "Inheriting the class' documentation.")]
        public BlurAttribute()
        {
        }

        /// <summary>
        /// Gets or sets whether or not the weaved <see cref="Assembly"/>
        /// should be cleaned up.
        /// <para>
        /// If <see langword="true"/>, every class and method related to Blur
        /// will be removed.
        /// </para>
        /// <para>
        /// Default: <see langword="true"/>.
        /// </para>
        /// </summary>
        public bool CleanUp { get; set; } = true;

        /// <summary>
        /// Gets or sets the list of all visitors that will be used, by name.
        /// <para>
        /// Hint: use <see langword="nameof"/> to reference those visitors.
        /// </para>
        /// <para>
        /// If this array is <see langword="null"/>, every visitor that can be
        /// found will be used.
        /// </para>
        /// <para>
        /// Default: <see langword="null"/>.
        /// </para>
        /// </summary>
        public string[] Visitors { get; set; } = null;

        /// <summary>
        /// Gets or sets the list of all assemblies that will be unreferenced
        /// if <see cref="CleanUp"/> is <see langword="true"/>, by name.
        /// <para>
        /// Default: <c>{ "Mono.Cecil" }</c>
        /// </para>
        /// </summary>
        /// <remarks>
        /// Blur.Library is unreferenced by default.
        /// </remarks>
        public string[] AdditionalUnreferencedAssemblies { get; set; } = { "Mono.Cecil" };
    }
}
