using System;
using System.Collections.Generic;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
        #region IsMatch
        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// implements the interface <typeparamref name="T"/>.
        /// </summary>
        public static bool Implements<T>(this TypeDefinition type)
        {
            if (!type.HasInterfaces)
                return false;

            string toCompare = typeof(T).FullName;
            var interfaces = type.Interfaces;

            for (int i = 0; i < interfaces.Count; i++)
                if (interfaces[i].InterfaceType.FullName == toCompare)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// inherits the type <typeparamref name="T"/>.
        /// </summary>
        public static bool Inherits<T>(this TypeDefinition type)
        {
            string toCompare = typeof(T).FullName;
            return type.FullName == toCompare || type.IsMatch(x => x.FullName == toCompare);
        }

        /// <summary>
        /// Returns whether or not <paramref name="self"/> inherits
        /// <paramref name="typeDef"/>.
        /// </summary>
        public static bool Inherits(this TypeReference self, TypeReference typeDef)
        {
            return self.Resolve().IsMatch(x => x == typeDef);
        }

        /// <summary>
        /// Returns whether or not <paramref name="self"/> inherits
        /// <paramref name="typeDef"/>.
        /// </summary>
        public static bool Inherits(this TypeDefinition self, TypeDefinition typeDef)
        {
            return self.IsMatch(x => x == typeDef);
        }

        /// <summary>
        /// Returns whether or not <paramref name="self"/>'s base types
        /// matches the given <paramref name="predicate"/>.
        /// </summary>
        public static bool IsMatch(this TypeDefinition self, Predicate<TypeReference> predicate)
        {
            return predicate(self)
                || (self.BaseType != null
                    && (predicate(self.BaseType) || self.BaseType.Resolve().IsMatch(predicate)));
        }
        #endregion

        #region CreateInstance
        /// <summary>
        /// Creates an attribute of type <typeparamref name="T"/>, given
        /// its <see cref="CustomAttribute"/> data.
        /// </summary>
        public static T CreateInstance<T>(this CustomAttribute attribute)
        {
            TypeInfo type = attribute.AttributeType.AsTypeInfo();

            T weaver = (T)Activator.CreateInstance(type.AsType(), attribute.ConstructorArguments.Convert(x => x.Value));

            foreach (var field in attribute.Fields)
                type.GetDeclaredField(field.Name)
                    .SetValue(weaver, field.Argument.Value);

            foreach (var property in attribute.Properties)
                type.GetDeclaredProperty(property.Name)
                    .SetValue(weaver, property.Argument.Value);

            return weaver;
        }

        /// <summary>
        /// Creates an object of type <paramref name="typeRef"/>, given
        /// its constructor <paramref name="arguments"/>.
        /// </summary>
        public static object CreateInstance(this TypeReference typeRef, params object[] arguments)
        {
            return Activator.CreateInstance(typeRef.AsType(), arguments);
        }
        #endregion

        #region Convert
        /// <summary>
        /// Converts a list of type <typeparamref name="T"/> to
        /// an array of type <typeparamref name="U"/>.
        /// </summary>
        internal static U[] Convert<T, U>(this IReadOnlyList<T> self, Func<T, U> converter)
        {
            U[] array = new U[self.Count];

            for (int i = 0; i < array.Length; i++)
                array[i] = converter(self[i]);

            return array;
        }

        /// <summary>
        /// Converts a list of type <typeparamref name="T"/> to
        /// an array of type <typeparamref name="U"/>.
        /// </summary>
        internal static U[] Convert<T, U>(this Mono.Collections.Generic.Collection<T> self, Func<T, U> converter)
        {
            U[] array = new U[self.Count];

            for (int i = 0; i < array.Length; i++)
                array[i] = converter(self[i]);

            return array;
        }
        #endregion
    }
}
