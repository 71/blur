using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// Base interface for all weavers that
    /// can modify an <see cref="Assembly"/>.
    /// </summary>
    public interface IWeaver
    {
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave methods.
    /// </summary>
    public interface IMethodWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="MethodDefinition"/>.
        /// </summary>
        void Apply(MethodDefinition method);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave properties.
    /// </summary>
    public interface IPropertyWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="PropertyDefinition"/>.
        /// </summary>
        void Apply(PropertyDefinition property);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave types (classes and interfaces).
    /// </summary>
    public interface ITypeWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="TypeDefinition"/>.
        /// </summary>
        void Apply(TypeDefinition type);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave events.
    /// </summary>
    public interface IEventWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving an <see cref="EventInfo"/>.
        /// </summary>
        void Apply(EventDefinition @event);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave fields.
    /// </summary>
    public interface IFieldWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="FieldInfo"/>.
        /// </summary>
        void Apply(FieldDefinition field);
    }

    /// <summary>
    /// Indicates that this <see cref="IReturnValueWeaver"/> can weave return values.
    /// </summary>
    public interface IReturnValueWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="MethodReturnType"/>, given the
        /// <see cref="MethodDefinition"/> in which it is declared.
        /// </summary>
        void Apply(MethodReturnType returnType, MethodDefinition method);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave parameters.
    /// </summary>
    public interface IParameterWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="ParameterDefinition"/>, given the
        /// <see cref="MethodDefinition"/> in which it is declared.
        /// </summary>
        void Apply(ParameterDefinition parameter, MethodDefinition method);
    }

    /// <summary>
    /// Indicates that this <see cref="IWeaver"/> can weave generic parameters.
    /// </summary>
    public interface IGenericParameterWeaver : IWeaver
    {
        /// <summary>
        /// Begin weaving a <see cref="GenericParameter"/>, given the
        /// <see cref="MethodDefinition"/> in which it is declared.
        /// </summary>
        void Apply(GenericParameter parameter, MethodDefinition provider);

        /// <summary>
        /// Begin weaving a <see cref="GenericParameter"/>, given the
        /// <see cref="TypeDefinition"/> in which it is declared.
        /// </summary>
        void Apply(GenericParameter parameter, TypeDefinition provider);
    }
}
