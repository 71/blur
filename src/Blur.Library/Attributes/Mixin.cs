using System;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// Indicates that this method will be called during
    /// compilation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class MixinAttribute : Attribute, IMethodWeaver
    {
        private static readonly TypeReference MixinType = typeof(MixinAttribute).GetReference();

        /// <summary>
        /// Makes sure that every <see langword="static"/> method
        /// marked with the <see cref="MixinAttribute"/> are executed
        /// during compilation.
        /// </summary>
        public void Apply(MethodDefinition method)
        {
            // Make sure we've got a valid method.
            if (!method.HasCustomAttributes)
                return;
            if (!method.IsStatic)
                return;
            if (!method.HasBody)
                return;
            if (method.HasGenericParameters)
                return;

            // Find the Mixin attribute.
            foreach (CustomAttribute attribute in method.CustomAttributes)
            {
                if (attribute.AttributeType == MixinType)
                {
                    // We've got a match! Call the method.
                    object[] parameters;

                    if (method.Parameters.Count == 0)
                        parameters = new object[0];
                    else if (method.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == typeof(TypeDefinition).FullName)
                        parameters = new object[] { method.DeclaringType };
                    else if (method.Parameters.Count == 1 && method.Parameters[0].ParameterType.FullName == typeof(MethodDefinition).FullName)
                        parameters = new object[] { method };
                    else
                        throw new Exception("Invalid mixin signature.");

                    try
                    {
                        method.InvokeDefinition(null, parameters);
                    }
                    catch
                    {
                        method.DeclaringType.Methods.Remove(method);
                        throw;
                    }

                    // Remove the method.
                    method.DeclaringType.Methods.Remove(method);
                    break;
                }
            }
        }
    }
}
