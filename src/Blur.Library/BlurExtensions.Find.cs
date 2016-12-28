using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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
        /// 
        /// </summary>
        public static MethodDefinition FindMethod(this TypeDefinition type, string name, params Type[] parameterTypes)
        {
            return type.Methods.FirstOrDefault(x => x.Name == name
                                          && x.Parameters.CompareSequences(parameterTypes,
                                                y => y.ParameterType.FullName, y => y.FullName));
        }

        /// <summary>
        /// 
        /// </summary>
        public static MethodDefinition FindMethod(this TypeDefinition type, string name, params TypeReference[] parameterTypes)
        {
            return type.Methods.FirstOrDefault(x => x.Name == name
                                          && x.Parameters.CompareSequences(parameterTypes,
                                                y => y.ParameterType.FullName, y => y.FullName));
        }

        /// <summary>
        /// 
        /// </summary>
        public static FieldDefinition FindField(this TypeDefinition type, string name)
        {
            return type.Fields.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static PropertyDefinition FindProperty(this TypeDefinition type, string name)
        {
            return type.Properties.FirstOrDefault(x => x.Name == name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static EventDefinition FindEvent(this TypeDefinition type, string name)
        {
            return type.Events.FirstOrDefault(x => x.Name == name);
        }

        #region Expressions
        /// <summary>
        /// 
        /// </summary>
        public static FieldDefinition FindField<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is FieldInfo))
                throw new ArgumentException("", nameof(expr));

            return typeof(T).GetDefinition().FindField(ex.Member.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static PropertyDefinition FindProperty<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is PropertyInfo))
                throw new ArgumentException("", nameof(expr));

            return typeof(T).GetDefinition().FindProperty(ex.Member.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        public static EventDefinition FindEvent<T>(this T obj, Expression<Func<T, object>> expr)
        {
            MemberExpression ex = expr.Body as MemberExpression;
            if (!(ex?.Member is EventInfo))
                throw new ArgumentException("", nameof(expr));

            return typeof(T).GetDefinition().FindEvent(ex.Member.Name);
        }
        #endregion
    }
}
