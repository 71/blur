using System;
using Blur.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Annotations
{
    /// <summary>
    /// Adds a <see langword="null"/>-check for this parameter
    /// in the method body.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public sealed class NotNullAttribute : Attribute, IParameterWeaver, IReturnValueWeaver
    {
        /// <inheritdoc />
        /// <remarks>
        /// This method only checks "return <see langword="null"/>" statements
        /// during compile-time and will throw if a <see langword="null"/> value is returned
        /// during runtime.
        /// </remarks>
        public void Apply(MethodReturnType returnType, MethodDefinition method)
        {
            ContractUtils.CheckExternal(method);

            if (returnType.ReturnType.IsValueType)
                throw new Exception("The NotNull attribute can only be set on reference values.");
            if (returnType.ReturnType.FullName == "System.Void")
                throw new Exception("The NotNull attribute cannot be set on void return values.");

            ILWriter writer = method.Write();

            writer.ForEach(ins =>
            {
                if (ins.OpCode == OpCodes.Ret)
                {
                    // Check if we're returning null during compilation.
                    if (ins.Previous?.OpCode.Code == Code.Ldnull)
                        throw new Exception("Cannot return null from a [NotNull] method.");

                    // Add runtime check.
                    writer.Before(ins)
                          .Dup()
                          .IfNull()
                              .Pop()
                              .Throw<ArgumentNullException>("return value")
                          .End();
                }
            });
        }

        /// <inheritdoc />
        public void Apply(ParameterDefinition parameter, MethodDefinition method)
        {
            ContractUtils.CheckExternal(method);

            if (parameter.ParameterType.IsValueType)
                throw new Exception("The NotNull attribute can only be set on reference parameters.");

            method.Write().ToStart()
                  .LoadArg(parameter.Sequence)
                  .IfNull()
                      .Throw<ArgumentNullException>(parameter.Name)
                  .End();
        }
    }
}
