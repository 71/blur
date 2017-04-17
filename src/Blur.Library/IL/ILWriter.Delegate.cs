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
            var bodyReturnType = body.Method.ReturnType;
            bool returnsVoid = bodyReturnType.Is(typeof(void), false),
                 toStatic = Method.IsStatic,
                 checkArg = toStatic != body.Method.IsStatic;

            int argLength = arguments.Length;

            Copy(body, Method.Body, position, ins =>
            {
                switch (ins.OpCode.Code)
                {
                    case Code.Ldfld:
                    {
                        // Maybe it gets a Context field?
                        FieldReference target = (FieldReference)ins.Operand;

                        if (target.DeclaringType?.DeclaringType?.FullName == "Blur.Context")
                            return new[] { Instruction.Create(OpCodes.Nop) };

                        break;
                    }

                    case Code.Call:
                    {
                        // Maybe it calls Context.smth?
                        MethodReference target = (MethodReference)ins.Operand;

                        if (target.DeclaringType?.DeclaringType?.FullName == "Blur.Context")
                            return new[] { Instruction.Create(OpCodes.Nop) };

                        break;
                    }
                }

                int argIndex = LdargToInteger(ins);

                if (argIndex != -1)
                {
                    // Accessing an argument.
                    // In case it's a new one, inject it.
                    if (checkArg)
                        argIndex--;

                    if (argIndex <= argLength)
                    {
                        // Access to an argument from the outside
                        var arg = arguments[argIndex] as Context.Any;

                        if (arg != null)
                            return arg.GetInstructions(this);
                    }
                }

                return null;
            });

            //// When we insert a delegate, the first n parameters have to be replaced
            //// with the objects in arguments. Also, all ldarg.* calls must be replaced.
            //int argLength  = arguments.Length,
            //    bodyLength = body.Instructions.Count,
            //    initialPos = position,
            //    initialOffset      = CurrentOffset,
            //    initialParamsCount = parameters.Count,
            //    initialVarsCount   = variables.Count;

            //if (bodyReturnType.DeclaringType?.FullName == "Blur.Context")
            //    bodyReturnType = ((GenericInstanceType)bodyReturnType).GenericArguments[0];

            //if (!returnsVoid && bodyReturnType != Method.ReturnType)
            //    throw new InvalidOperationException("Invalid return type.");

            //var bodyInstructions = body.Instructions;
            //var bodyParameters   = body.Method.Parameters;
            //var bodyVariables    = body.Variables;

            // If it's inlined, we don't want a ret call at the end of it.
            if (inlined)
            {
                Instruction last = instructions[position + body.Instructions.Count - 1];

                if (last.OpCode.Code == Code.Ret)
                    last.OpCode = returnsVoid ? OpCodes.Nop : OpCodes.Pop;
            }

            //// Fix accesses to Context and arguments.
            //for (int i = initialPos; i < position; i++)
            //{
            //    Instruction ins = instructions[i];
            //    int argIndex = LdargToInteger(ins);

            //    if (argIndex != -1)
            //    {
            //        if (checkArg)
            //            argIndex--;

            //        // Access to an argument
            //        if (argIndex <= argLength)
            //        {
            //            // Access to an argument from the outside
            //            object arg = arguments[argIndex];

            //            if (arg is Context.IContextObj)
            //            {
            //                // Context call, change it
            //                var ctx = (Context.IContextObj)arguments[argIndex];

            //                Instruction replacement = ctx.GetInstruction(this);

            //                ins.OpCode  = replacement.OpCode;
            //                ins.Operand = replacement.Operand;

            //                continue;
            //            }

            //            Instruction[] newInstructions = InstructionsForConstant(arg);

            //            if (newInstructions.Length == 2)
            //                instructions.Insert(i + 1, newInstructions[1]);
            //            instructions[i] = newInstructions[0];
            //        }
            //        else
            //        {
            //            // Access to an actual parameter
            //            FixLdargFor(parameters[argIndex - argLength + initialParamsCount], ins);
            //        }
            //    }

            //    ins.FixOffset();
            //}
        }

        private ILWriter Delegate(object[] arguments, Delegate del)
        {
            MethodInfo info = del.GetMethodInfo();
            MethodBody body = info.GetDefinition().Body;

            Processor.MarkForDeletion(body.Method);

            this.InsertDelegate(arguments, body, info.ReturnType == typeof(void));
            return this;
        }
    }
}
