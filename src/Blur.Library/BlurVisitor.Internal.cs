using System;
using System.Collections.Generic;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// Represents the default <see cref="BlurVisitor"/>, that visits
    /// all members marked with a <see cref="IWeaver"/> <see cref="Attribute"/> in an assembly.
    /// </summary>
    internal sealed class AttributesVisitor : BlurVisitor
    {
        private static bool shouldCleanUp;

        /// <inheritdoc/>
        public override int Priority => 100;

        internal AttributesVisitor()
        {
            shouldCleanUp = Processor.Settings.CleanUp;
        }

        #region Utils
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

                if (!attr.AttributeType.Resolve().Implements<IWeaver>())
                    continue;

                callback(attr.CreateInstance<TWeaver>());

                if (shouldCleanUp)
                    attributes.RemoveAt(i--);
            }
        }
        #endregion

        /// <inheritdoc/>
        protected override void Visit(AssemblyDefinition assembly)
        {
            if (assembly.HasCustomAttributes)
                Visit<IAssemblyWeaver>(assembly.CustomAttributes, weaver => weaver.Apply(assembly));
        }

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
                // Put this in its own scope to avoid having returnType in the following call sites.
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
