using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
        internal static bool CompareSequences<T, U, V>(this IEnumerable<T> first, IEnumerable<U> second,
            Func<T, V> firstGetter, Func<U, V> secondGetter)
        {
            return first.Select(firstGetter).SequenceEqual(second.Select(secondGetter));
        }

        /// <summary>
        /// Find a method, given its declaring <paramref name="type"/>
        /// <paramref name="name"/>, and parameters.
        /// <para>
        /// If <paramref name="parameterTypes"/> is <see langword="null"/>, the first method whose
        /// <paramref name="name"/> matches will be returned.
        /// </para>
        /// </summary>
        public static MethodDefinition FindMethod(this TypeDefinition type, string name, params Type[] parameterTypes)
        {
            if (parameterTypes == null)
                return type.Methods.FirstOrDefault(x => x.Name == name);

            return type.Methods.FirstOrDefault(x => x.Name == name
                                          && x.Parameters.CompareSequences(parameterTypes,
                                                y => y.ParameterType.FullName, y => y.FullName));
        }

        ///// <inheritdoc cref="FindMethod(TypeDefinition, string, Type[])"/>
        //public static MethodDefinition FindMethod(this TypeDefinition type, string name, params TypeReference[] parameterTypes)
        //{
        //    if (parameterTypes == null)
        //        return type.Methods.FirstOrDefault(x => x.Name == name);

        //    return type.Methods.FirstOrDefault(x => x.Name == name
        //                                  && x.Parameters.CompareSequences(parameterTypes,
        //                                        y => y.ParameterType.FullName, y => y.FullName));
        //}

        /// <summary>
        /// Find a field, given its declaring <paramref name="type"/>
        /// and <paramref name="name"/>.
        /// </summary>
        public static FieldDefinition FindField(this TypeDefinition type, string name)
        {
            return type.Fields.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Find a property, given its declaring <paramref name="type"/>
        /// and <paramref name="name"/>.
        /// </summary>
        public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
        {
            return type.Properties.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// Find an event, given its declaring <paramref name="type"/>
        /// and <paramref name="name"/>.
        /// </summary>
        public static EventDefinition FindEvent(this TypeDefinition type, string name)
        {
            return type.Events.FirstOrDefault(x => x.Name == name);
        }
    }
}

#region Additional Extensions
namespace Blur.Extensions
{
    /// <summary>
    /// Provides extension methods meant to easily access
    /// a type's members.
    /// </summary>
    public static class FindExtensions
    {
        /// <summary>
        /// Find the <see cref="MethodDefinition"/> corresponding to the
        /// given <paramref name="name"/>.
        /// </summary>
        /// <param name="obj">Object whose method will be returned.</param>
        /// <param name="name">Name of the method to return.</param>
        /// <param name="parameterTypes">Type of all the parameters. If <see langword="null"/>, the first matching method will be returned.</param>
        public static MethodDefinition FindMethod<T>(this T obj, string name, params Type[] parameterTypes)
        {
            TypeDefinition type = typeof(T).GetDefinition();

            if (parameterTypes == null)
                return type.Methods.FirstOrDefault(x => x.Name == name);

            return type.Methods.FirstOrDefault(x => x.Name == name
                                          && x.Parameters.CompareSequences(parameterTypes,
                                                y => y.ParameterType.FullName, y => y.FullName));
        }

        /// <summary>
        /// Find the <see cref="FieldDefinition"/> corresponding to the
        /// given <paramref name="expr"/>.
        /// </summary>
        /// <param name="obj">Object whose field will be returned.</param>
        /// <param name="expr"><see cref="MemberExpression"/> to the field.</param>
        /// <exception cref="ArgumentException"><paramref name="expr"/>'s body is not a <see cref="MemberExpression"/>.</exception>
        public static FieldDefinition FindField<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is FieldInfo))
                throw new ArgumentException("Invalid expression given.", nameof(expr));

            return typeof(T).GetDefinition().FindField(ex.Member.Name);
        }

        /// <summary>
        /// Find the <see cref="PropertyDefinition"/> corresponding to the
        /// given <paramref name="expr"/>.
        /// </summary>
        /// <param name="obj">Object whose property will be returned.</param>
        /// <param name="expr"><see cref="MemberExpression"/> to the property.</param>
        /// <exception cref="ArgumentException"><paramref name="expr"/>'s body is not a <see cref="MemberExpression"/>.</exception>
        public static PropertyDefinition FindProperty<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is PropertyInfo))
                throw new ArgumentException("Invalid expression given.", nameof(expr));

            return typeof(T).GetDefinition().FindProperty(ex.Member.Name);
        }

        /// <summary>
        /// Find the <see cref="EventDefinition"/> corresponding to the
        /// given <paramref name="expr"/>.
        /// </summary>
        /// <param name="obj">Object whose event will be returned.</param>
        /// <param name="expr"><see cref="MemberExpression"/> to the event.</param>
        /// <exception cref="ArgumentException"><paramref name="expr"/>'s body is not a <see cref="MemberExpression"/>.</exception>
        public static EventDefinition FindEvent<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is EventInfo))
                throw new ArgumentException("Invalid expression given.", nameof(expr));

            return typeof(T).GetDefinition().FindEvent(ex.Member.Name);
        }
    }
}
#endregion