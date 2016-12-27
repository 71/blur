using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// Represents the default <see cref="BlurVisitor"/>, that visits
    /// all members marked with a <see cref="IWeaver"/> <see cref="Attribute"/> in an assembly.
    /// </summary>
    internal sealed class InternalBlurVisitor : BlurVisitor
    {
        private static bool shouldCleanUp;

        internal InternalBlurVisitor() : base(ProcessingState.Before)
        {
            shouldCleanUp = Processor.Settings.CleanUp;
        }

        #region Utils
        /// <summary>
        /// Returns whether or not the specified <paramref name="type"/>
        /// implements the given interface <typeparamref name="T"/>.
        /// </summary>
        private static bool IsWeaver<T>(TypeReference type)
        {
            TypeDefinition typeDef = type.Resolve();
            return typeDef != null && typeDef.Interfaces.Any(x => x.InterfaceType.FullName == typeof(T).FullName);
        }

        /// <summary>
        /// Creates an attribute of type <typeparamref name="T"/>, given
        /// its <see cref="CustomAttribute"/> data.
        /// </summary>
        private static T Create<T>(CustomAttribute data)
        {
            TypeInfo type = data.AttributeType.AsTypeInfo();

            T weaver = (T)Activator.CreateInstance(type.AsType(), data.ConstructorArguments.Convert(x => x.Value));

            foreach (var field in data.Fields)
                type.GetDeclaredField(field.Name)
                    .SetValue(weaver, field.Argument.Value);

            foreach (var property in data.Properties)
                type.GetDeclaredProperty(property.Name)
                    .SetValue(weaver, property.Argument.Value);

            return weaver;
        }

        /// <summary>
        /// Visits all the given <paramref name="attributes"/>.
        /// If an attribute is a <typeparamref name="TWeaver"/>, <paramref name="callback"/>
        /// will be called with it.
        /// Additionally, if <see cref="Processor.Settings"/>.<c>CleanUp</c>
        /// is <see langword="true"/>, the attribute will be removed.
        /// </summary>
        private static void Visit<TWeaver>(IList<CustomAttribute> attributes, Action<TWeaver> callback) where TWeaver : IWeaver
        {
            for (int i = 0; i < attributes.Count; i++)
            {
                CustomAttribute attr = attributes[i];

                if (!IsWeaver<TWeaver>(attr.AttributeType))
                    continue;

                callback(Create<TWeaver>(attr));

                if (shouldCleanUp)
                    attributes.RemoveAt(i--);
            }
        }
#endregion

        /// <inheritdoc/>
        protected override void Visit(TypeDefinition type)
        {
            foreach (GenericParameter genParam in type.GenericParameters)
                if (genParam.HasCustomAttributes)
                    Visit<IGenericParameterWeaver>(genParam.CustomAttributes, weaver => weaver.Apply(genParam, type));

            if (type.HasCustomAttributes)
                Visit<ITypeWeaver>(type.CustomAttributes, weaver => weaver.Apply(type));
        }

        /// <inheritdoc/>
        protected override void Visit(FieldDefinition field)
        {
            if (field.HasCustomAttributes)
                Visit<IFieldWeaver>(field.CustomAttributes, weaver => weaver.Apply(field));
        }

        /// <inheritdoc/>
        protected override void Visit(EventDefinition @event)
        {
            if (@event.HasCustomAttributes)
                Visit<IEventWeaver>(@event.CustomAttributes, weaver => weaver.Apply(@event));
        }

        /// <inheritdoc/>
        protected override void Visit(PropertyDefinition property)
        {
            if (property.HasCustomAttributes)
                Visit<IPropertyWeaver>(property.CustomAttributes, weaver => weaver.Apply(property));
        }

        /// <inheritdoc/>
        protected override void Visit(MethodDefinition method)
        {
            {
                // Put this in its own scope to avoid creating putting returnType
                // in lambdas.
                MethodReturnType returnType = method.MethodReturnType;
                if (returnType.HasCustomAttributes)
                    Visit<IReturnValueWeaver>(returnType.CustomAttributes, weaver => weaver.Apply(returnType, method));
            }

            foreach (GenericParameter genParam in method.GenericParameters)
                if (genParam.HasCustomAttributes)
                    Visit<IGenericParameterWeaver>(genParam.CustomAttributes, weaver => weaver.Apply(genParam, method));

            if (method.HasCustomAttributes)
                Visit<IMethodWeaver>(method.CustomAttributes, weaver => weaver.Apply(method));
        }

        /// <inheritdoc/>
        protected override void Visit(ParameterDefinition parameter, MethodDefinition method)
        {
            if (parameter.HasCustomAttributes)
                Visit<IParameterWeaver>(parameter.CustomAttributes, weaver => weaver.Apply(parameter, method));
        }
    }
}
