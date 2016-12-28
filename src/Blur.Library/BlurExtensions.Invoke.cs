using System;
using System.Reflection;
using Mono.Cecil;
using System.Reflection.Emit;
using SR = System.Reflection;
using System.Collections.Generic;

namespace Blur
{
    partial class BlurExtensions
    {
        private static readonly Dictionary<string, DynamicMethod> dynamicMethods = new Dictionary<string, DynamicMethod>();

        /// <inheritdoc cref="MethodBase.Invoke(object, object[])"/>
        public static object Invoke(this MethodDefinition method, object obj, params object[] parameters)
        {
            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (!method.IsStatic && obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));

            MethodBase matching = method.AsInfo();

            if (matching != null)
                return matching.Invoke(obj, parameters);

            DynamicMethod dynMethod;

            if (!dynamicMethods.TryGetValue(method.FullName, out dynMethod))
                dynamicMethods.Add(method.FullName, (dynMethod = CreateMethod(method)));

            return dynMethod.Invoke(obj, parameters);
        }

        private static DynamicMethod CreateMethod(MethodDefinition definition)
        {
            // Create method.
            DynamicMethod method = new DynamicMethod(definition.Name, (SR.MethodAttributes)(int)definition.Attributes,
                (CallingConventions)(int)definition.CallingConvention, definition.ReturnType.AsType(),
                definition.Parameters.Convert(param => param.ParameterType.AsType()), Processor.TargetModule, true);

            // MethodBody => DynamicMethod.
            definition.Write().CopyTo(method.GetILGenerator());

            // Return.
            return method;
        }
    }
}
