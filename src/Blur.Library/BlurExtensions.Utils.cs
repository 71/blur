using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
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
    }
}
