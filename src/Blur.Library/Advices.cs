using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Advices
{
    /// <summary>
    /// Static class used to "advise" methods, changing their
    /// runtime behavior.
    /// </summary>
    public static class Advice
    {
        private static readonly MethodReference GetMethodFromHandleReference
            = typeof(MethodBase)
                .GetRuntimeMethod(nameof(MethodBase.GetMethodFromHandle), new[] { typeof(RuntimeMethodHandle) })
                .GetReference();

        private static readonly TypeReference AdviceReference
            = typeof(Advice).GetReference();

        private enum AdviceMember
        {
            None = 0, Method, Target, Arguments, Invoke, Return
        }

        private static AdviceMember AccessedMethod(MethodReference method)
        {
            if (method.DeclaringType != AdviceReference)
                return AdviceMember.None;

            switch (method.Name)
            {
                case nameof(Invoke):
                    return AdviceMember.Invoke;
                case nameof(Return):
                    return AdviceMember.Return;

                case "get_" + nameof(Arguments):
                    return AdviceMember.Arguments;
                case "get_" + nameof(Target):
                    return AdviceMember.Target;
                case "get_" + nameof(Method):
                    return AdviceMember.Method;

                default:
                    return AdviceMember.None;
            }
        }

        /// <summary>
        /// Change the content of the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="method">The method whose body will be replaced.</param>
        /// <param name="modifier">The new body of the method. In this body, calls to all <see cref="Advice"/> members can be made.</param>
        public static void Advise(this MethodDefinition method, Action modifier)
        {
            MethodDefinition modifierDefinition = modifier?.GetMethodInfo().GetDefinition();

            if (modifierDefinition == null)
                throw new Exception("Invalid modifier");

            Processor.MarkForDeletion(modifierDefinition);

            // Copy the method we're supposed to invoke to an hidden method.
            MethodDefinition old = new MethodDefinition($"$underlying_{method.Name}",
                method.Attributes,
                method.ReturnType) { IsPrivate = true };

            foreach (GenericParameter genParameter in method.GenericParameters)
                old.GenericParameters.Add(new GenericParameter(genParameter.Name, genParameter.Owner));

            foreach (ParameterDefinition parameter in method.Parameters)
                old.Parameters.Add(new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType));

            method.DeclaringType.Methods.Add(old);
            method.Write().CopyTo(old.Body);

            // Rewrite the original method to invoke the context instead.
            // For this, we copy the content of the original method with the given modifier.
            ILWriter writer = method.Rewrite();

            modifierDefinition.Write().CopyTo(method.Body);

            // First, create the array which stores all arguments.
            TypeSystem typeSystem = Bridge.TargetAssemblyDefinition.MainModule.TypeSystem;
            VariableDefinition argsVar = new VariableDefinition(typeof(object[]).GetReference());

            method.Body.Variables.Add(argsVar);

            writer.Array(typeSystem.Object, method.Parameters.Count);

            foreach (ParameterDefinition parameter in method.Parameters)
            {
                writer.Dup()
                      .Int(parameter.Sequence)
                      .LoadArg(parameter)
                      .Box(parameter.ParameterType)
                      .SaveToIndex();
            }

            writer.SaveTo(argsVar);

            // Then, replace each call to Advice by something else.
            writer.ForEach(ins =>
            {
                MethodReference target = ins.Operand as MethodReference;

                if (target == null)
                    return;

                switch (AccessedMethod(target))
                {
                    case AdviceMember.Method:
                        // ldtoken method [method]
                        // call MethodBase.GetMethodFromHandle(RuntimeMethodHandle)
                        ins.OpCode = OpCodes.Ldtoken;
                        ins.Operand = old;
                        writer.After(ins)
                              .Call(GetMethodFromHandleReference);
                        break;
                    case AdviceMember.Target:
                        // ldarg_0
                        ins.OpCode = method.HasThis ? OpCodes.Ldarg_0 : OpCodes.Ldnull;
                        ins.Operand = null;
                        break;
                    case AdviceMember.Arguments:
                        // ldloc argsVar
                        ins.OpCode = OpCodes.Ldloc;
                        ins.Operand = argsVar;
                        break;
                    case AdviceMember.Return:
                        // ret
                        ins.OpCode = OpCodes.Ret;
                        ins.Operand = null;
                        break;
                    case AdviceMember.Invoke:
                    {
                        // Put all objects in the array at the top of the stack
                        // in the stack themselves.
                        bool isStatic = method.IsStatic;

                        writer.Before(ins)
                              .Load(argsVar);

                        if (!isStatic)
                            writer.This();

                        foreach (ParameterDefinition parameter in method.Parameters)
                        {
                            writer.Int(parameter.Index)
                                  .LoadIndex()
                                  .Cast(parameter.ParameterType);
                        }

                        ins.OpCode = isStatic ? OpCodes.Call : OpCodes.Callvirt;
                        ins.Operand = old;

                        break;
                    }
                }
            }, false);
        }

        /// <summary>
        /// Gets the method that will be invoked by <see cref="Invoke(object[])"/>.
        /// </summary>
        public static MethodBase Method => null;

        /// <summary>
        /// Gets the target of the call, or <see langword="null"/> if the method
        /// is static.
        /// </summary>
        public static object Target => null;

        /// <summary>
        /// Gets the arguments of the call.
        /// </summary>
        public static object[] Arguments => null;

        /// <summary>
        /// Invokes the original method with the given arguments,
        /// and returns its result.
        /// </summary>
        public static object Invoke(params object[] args) => null;

        /// <summary>
        /// Returns the given value.
        /// </summary>
        public static void Return(object obj) { }
    }
}
