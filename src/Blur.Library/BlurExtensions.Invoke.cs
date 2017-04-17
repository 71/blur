using System;
using System.Reflection;
using Mono.Cecil;
using System.Reflection.Emit;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Blur
{
    partial class BlurExtensions
    {
        private static readonly Dictionary<string, Tuple<int, DynamicMethod>> dynamicMethods = new Dictionary<string, Tuple<int, DynamicMethod>>();

        /// <inheritdoc cref="MethodBase.Invoke(object, object[])"/>
        internal static object InvokeDefinition(this MethodDefinition method, object obj, params object[] parameters)
        {
            // TODO: Fix this.
            //
            // Right now, I can get a body FULLY identical to
            // working compiled IL (for what I've tried, which is rather simple).
            // However, calling Invoke() will throw an exception anyway (invalid program).
            //
            // I suspect the problem comes from ILGenerator.BakeByteArray()
            // or another internal method. I'm going to try to step into .NET sources,
            // and see what's up.
            // 

            if (method == null)
                throw new ArgumentNullException(nameof(method));
            if (!method.IsStatic && obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            if (!method.HasBody)
                throw new ArgumentException("The given method must have a body.", nameof(method));

            return method.GetMethod().Invoke(obj, parameters);
        }

        /// <summary>
        /// Returns the method associated with a method definition.
        /// <para>
        /// <see cref="AsInfo(MethodDefinition)"/> only attempts to get a matching
        /// <see cref="MethodBase"/>, whereas <see cref="GetMethod(MethodDefinition)"/>
        /// will generate a new method if needed.
        /// </para>
        /// </summary>
        internal static MethodBase GetMethod(this MethodDefinition method)
        {
            // If the method hasn't changed, run it directly.
            if (!ILWriter.ChangedMethods.Contains(method.MetadataToken.RID))
            {
                MethodBase matching = method.AsInfo();

                if (matching != null)
                    return matching;
            }

            if (!method.HasBody)
                throw new ArgumentException("Cannot create dynamic method from virtual or external method.");

            Tuple<int, DynamicMethod> dynMethod;

            if (!dynamicMethods.TryGetValue(method.FullName, out dynMethod))
                // No method has been found, create it.
                dynamicMethods.Add(method.FullName, dynMethod = CreateMethod(method));
            else if (dynMethod.Item1 != method.Body.CodeSize)
                // A matching method has been found, but it needs to be updated.
                dynamicMethods[method.FullName] = dynMethod = UpdateMethod(method, dynMethod.Item2);
            
            return dynMethod.Item2;
        }

        private static Tuple<int, DynamicMethod> CreateMethod(MethodDefinition definition)
        {
            // Create method.
            DynamicMethod method = new DynamicMethod(definition.Name, definition.ReturnType.AsType(),
                definition.Parameters.Convert(param => param.ParameterType.AsType()), Processor.TargetModule, true);

            // MethodBody => DynamicMethod.
            definition.Write().CopyTo(method.GetILGenerator());

            // Return.
            return Tuple.Create(definition.Body.CodeSize, method);
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Hopefully it will be useful one day...")]
        private static Tuple<int, DynamicMethod> UpdateMethod(MethodDefinition definition, DynamicMethod old)
        {
            // It would be nice to do something about the old method in the future.
            // Right now, it can't be disposed or removed, so we just ignore it.

            return CreateMethod(definition);
        }
    }
}
