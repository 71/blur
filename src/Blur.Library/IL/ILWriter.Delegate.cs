using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        #region *ToInteger
        private static int LdargToInteger(Instruction ins)
        {
            switch (ins.OpCode.Code)
            {
                case Code.Ldarg_0:
                    return 0;
                case Code.Ldarg_1:
                    return 1;
                case Code.Ldarg_2:
                    return 2;
                case Code.Ldarg_3:
                    return 3;
                case Code.Ldarg:
                case Code.Ldarg_S:
                    return (ins.Operand as ParameterDefinition)?.Sequence ?? (int)ins.Operand;
                default:
                    return -1;
            }
        }

        private static int LocToInteger(Instruction ins)
        {
            switch (ins.OpCode.Code)
            {
                case Code.Ldloc_0:
                case Code.Stloc_0:
                    return 0;
                case Code.Ldloc_1:
                case Code.Stloc_1:
                    return 1;
                case Code.Ldloc_2:
                case Code.Stloc_2:
                    return 2;
                case Code.Ldloc_3:
                case Code.Stloc_3:
                    return 3;
                case Code.Ldloc_S:
                case Code.Stloc_S:
                case Code.Ldloc:
                case Code.Stloc:
                    return (ins.Operand as VariableDefinition)?.Index ?? (int)ins.Operand;
                default:
                    return -1;
            }
        }

        private static int LdlocToInteger(Instruction ins)
        {
            switch (ins.OpCode.Code)
            {
                case Code.Ldloc_0:
                    return 0;
                case Code.Ldloc_1:
                    return 1;
                case Code.Ldloc_2:
                    return 2;
                case Code.Ldloc_3:
                    return 3;
                case Code.Ldloc:
                case Code.Ldloc_S:
                    return (ins.Operand as VariableDefinition)?.Index ?? (int)ins.Operand;
                default:
                    return -1;
            }
        }

        private static int StlocToInteger(Instruction ins)
        {
            switch (ins.OpCode.Code)
            {
                case Code.Stloc_0:
                    return 0;
                case Code.Stloc_1:
                    return 1;
                case Code.Stloc_2:
                    return 2;
                case Code.Stloc_3:
                    return 3;
                case Code.Stloc:
                case Code.Stloc_S:
                    return (ins.Operand as VariableDefinition)?.Index ?? (int)ins.Operand;
                default:
                    return -1;
            }
        }
        #endregion

        #region Fix
        internal static void FixLdargFor(ParameterDefinition param, Instruction ins)
        {
            ins.Operand = null;

            switch (param.Sequence)
            {
                case 0:
                    ins.OpCode = OpCodes.Ldarg_0; break;
                case 1:
                    ins.OpCode = OpCodes.Ldarg_1; break;
                case 2:
                    ins.OpCode = OpCodes.Ldarg_2; break;
                case 3:
                    ins.OpCode = OpCodes.Ldarg_3; break;
                default:
                    ins.OpCode = param.Sequence > sbyte.MaxValue ? OpCodes.Ldarg : OpCodes.Ldarg_S;
                    ins.Operand = param;
                    break;
            }
        }
        #endregion

        private void InsertDelegate(object[] arguments, MethodBody body, bool inlined)
        {
            // When we insert a delegate, the first n parameters have to be replaced
            // with the objects in arguments. Also, all ldarg.* calls must be replaced.
            int argLength  = arguments.Length,
                bodyLength = body.Instructions.Count,
                initialPos = position,
                initialOffset      = CurrentOffset,
                initialParamsCount = parameters.Count,
                initialVarsCount   = variables.Count;

            var bodyReturnType = body.Method.ReturnType;
            if (bodyReturnType.DeclaringType?.FullName == "Blur.Context")
                bodyReturnType = ((GenericInstanceType)bodyReturnType).GenericArguments[0];
            var returnsVoid    = bodyReturnType.FullName == "System.Void";

            if (!returnsVoid && bodyReturnType != Method.ReturnType)
                throw new InvalidOperationException("Invalid return type.");

            var bodyInstructions = body.Instructions;
            var bodyParameters   = body.Method.Parameters;
            var bodyVariables    = body.Variables;

            bool toStatic = Method.IsStatic,
                 checkArg = toStatic != body.Method.IsStatic;

            // Import variables and parameters
            for (int i = argLength; i < bodyParameters.Count; i++)
            {
                ParameterDefinition parameter = bodyParameters[i];

                if (parameter.ParameterType.DeclaringType?.FullName == "Blur.Context")
                    parameter.ParameterType = ((GenericInstanceType)parameter.ParameterType).GenericArguments[0];

                parameters.Add(bodyParameters[i]);
            }
            for (int i = 0; i < bodyVariables.Count; i++)
            {
                VariableDefinition variable = bodyVariables[i];

                if (variable.VariableType.DeclaringType?.FullName == "Blur.Context")
                    variable.VariableType = ((GenericInstanceType)variable.VariableType).GenericArguments[0];

                variables.Add(bodyVariables[i]);
            }

            // Copy the body of the method simply, optionally fixing accesses to variables.
            for (int i = 0; i < bodyLength; i++)
            {
                Instruction ins = bodyInstructions[i].Clone();

                // If we're getting the Value field of a Context object, Nop' it.
                // If we're calling op_Implicit, Nop' it as well.
                if (ins.OpCode.Code == Code.Ldfld)
                {
                    FieldReference target = (FieldReference)ins.Operand;

                    if (target.DeclaringType?.DeclaringType?.FullName == "Blur.Context")
                        ins = Instruction.Create(OpCodes.Nop);
                }
                else if (ins.OpCode.Code == Code.Call)
                {
                    MethodReference target = (MethodReference)ins.Operand;

                    if (target.DeclaringType?.DeclaringType?.FullName == "Blur.Context")
                        ins = Instruction.Create(OpCodes.Nop);
                }

                // Fix the instruction if it accesses a variable
                int accessIndex = LocToInteger(ins);

                if (accessIndex != -1)
                {
                    // Access to a variable
                    if (ins.OpCode.StackBehaviourPop == StackBehaviour.Pop1)
                        FixStlocFor(accessIndex + initialVarsCount, ins);
                    else
                        FixLdlocFor(accessIndex + initialVarsCount, ins);
                }

                if (position != 0)
                {
                    ins.Previous = instructions[position - 1];
                    ins.FixOffset();
                }

                instructions.Insert(position++, ins);
            }

            // If it's inlined, we don't want a ret call at the end of it.
            if (inlined)
            {
                Instruction last = instructions[position - 1];
                if (last.OpCode.Code == Code.Ret)
                    last.OpCode = returnsVoid ? OpCodes.Nop : OpCodes.Pop;
            }

            // Fix branches within the copied instructions.
            for (int i = initialPos; i < position; i++)
            {
                object operand = instructions[i].Operand;

                if (operand is Instruction)
                {
                    Instruction ins = (Instruction)operand;

                    instructions[i].Operand = instructions[bodyInstructions.IndexOf(ins) + initialPos];
                }
                else if (operand is Instruction[])
                {
                    Instruction[] inss = (Instruction[])operand;

                    for (int o = 0; o < inss.Length; o++)
                        inss[o] = instructions.First(x => x.Offset >= inss[o].Offset + initialOffset);
                }
            }

            // Fix accesses to Context and arguments.
            for (int i = initialPos; i < position; i++)
            {
                Instruction ins = instructions[i];
                int argIndex = LdargToInteger(ins);

                if (argIndex != -1)
                {
                    if (checkArg)
                        argIndex--;

                    // Access to an argument
                    if (argIndex <= argLength)
                    {
                        // Access to an argument from the outside
                        object arg = arguments[argIndex];

                        if (arg is Context.IContextObj)
                        {
                            // Context call, change it
                            var ctx = (Context.IContextObj)arguments[argIndex];

                            Instruction replacement = ctx.GetInstruction(this);

                            ins.OpCode  = replacement.OpCode;
                            ins.Operand = replacement.Operand;

                            continue;
                        }

                        Instruction[] newInstructions = InstructionsFor(arg);

                        if (newInstructions.Length == 2)
                            instructions.Insert(i + 1, newInstructions[1]);
                        instructions[i] = newInstructions[0];
                    }
                    else
                    {
                        // Access to an actual parameter
                        FixLdargFor(parameters[argIndex - argLength + initialParamsCount], ins);
                    }
                }

                ins.FixOffset();
            }
        }

        private ILWriter Delegate(object[] arguments, Delegate del)
        {
            for (int i = 0; i < arguments.Length; i++)
            {
                object arg = arguments[i];

                if (arg != null && !IsConstant(arg.GetType()) && !(arg is Context.IContextObj))
                    throw new ArgumentException("The given argument must be constant.", ((char)(97 + i)).ToString());
            }

            MethodInfo info = del.GetMethodInfo();
            MethodBody body = info.GetDefinition().Body;

            this.InsertDelegate(arguments, body, del.GetType().Name.Contains("Action"));
            return this;
        }
    }
}
