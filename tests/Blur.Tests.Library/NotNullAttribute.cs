using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Tests.Library
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
        /// TODO: That's what I want to do, but right now "return null" is compiled with branches. Right now, only runtime checks are made.
        /// </remarks>
        public void Apply(MethodReturnType returnType, MethodDefinition method)
        {
            if (!method.HasBody)
                throw new Exception("The NotNull attribute can only be set on return values of a method that declares a body.");
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
                    if (ins.Previous.OpCode.Code == Code.Ldnull)
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
            if (!method.HasBody)
                throw new Exception("The NotNull attribute can only be set on parameters of a method that declares a body.");
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
