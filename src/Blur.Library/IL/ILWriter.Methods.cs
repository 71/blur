using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Returns the integer corresponding to the given instruction.
        /// </summary>
        private int NbrOf(Instruction ins)
        {
            switch (ins.OpCode.Code)
            {
                case Code.Ldc_I4_0:
                    return 0;
                case Code.Ldc_I4_1:
                    return 1;
                case Code.Ldc_I4_2:
                    return 2;
                case Code.Ldc_I4_3:
                    return 3;
                case Code.Ldc_I4_4:
                    return 4;
                case Code.Ldc_I4_5:
                    return 5;
                case Code.Ldc_I4_6:
                    return 6;
                case Code.Ldc_I4_7:
                    return 7;
                case Code.Ldc_I4_8:
                    return 8;
                case Code.Ldc_I4_M1:
                    return -1;

                default:
                    if (ins.Operand is int)
                        return (int)ins.Operand;
                    throw new InvalidOperationException("The given number must be a compile-time constant.");
            }
        }

        #region Fix / From
        private static void FixLdargFor(ParameterDefinition param, Instruction ins)
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

        private static void FixLdlocFor(int i, Instruction ins)
        {
            ins.Operand = null;

            switch (i)
            {
                case 0:
                    ins.OpCode = OpCodes.Ldloc_0; break;
                case 1:
                    ins.OpCode = OpCodes.Ldloc_1; break;
                case 2:
                    ins.OpCode = OpCodes.Ldloc_2; break;
                case 3:
                    ins.OpCode = OpCodes.Ldloc_3; break;
                default:
                    ins.OpCode = i > sbyte.MaxValue ? OpCodes.Ldloc : OpCodes.Ldloc_S;
                    ins.Operand = i;
                    break;
            }
        }

        private static void FixStlocFor(int i, Instruction ins)
        {
            ins.Operand = null;

            switch (i)
            {
                case 0:
                    ins.OpCode = OpCodes.Stloc_0; break;
                case 1:
                    ins.OpCode = OpCodes.Stloc_1; break;
                case 2:
                    ins.OpCode = OpCodes.Stloc_2; break;
                case 3:
                    ins.OpCode = OpCodes.Stloc_3; break;
                default:
                    ins.OpCode = i > sbyte.MaxValue ? OpCodes.Stloc : OpCodes.Stloc_S;
                    ins.Operand = i;
                    break;
            }
        }

        private static int FromStloc(Instruction ins)
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
                    return (int)ins.Operand;
                default:
                    return -1;
            }
        }

        private static int FromLdloc(Instruction ins)
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
                    return (int)ins.Operand;
                default:
                    return -1;
            }
        } 
        #endregion

        /// <summary>
        /// Update an instruction that may be accessing <see cref="Context"/>.
        /// </summary>
        private Instruction UpdateInstructionContext(Instruction ins, List<Instruction> toRemove)
        {
            MethodReference method = (MethodReference)ins.Operand;

            if (method.DeclaringType.FullName == "Blur.Context")
            {
                TypeReference targetType = null;
                bool isGeneric = false;

                if (method is GenericInstanceMethod)
                {
                    isGeneric = true;
                    targetType = ((GenericInstanceMethod)method).GenericArguments[0];
                }

                string targetName = ins.Previous.Operand as string;
                int targetIndex = targetName == null ? NbrOf(ins.Previous) : -1;
                
                switch (method.Name)
                {
                    case nameof(Context.Argument):
                        ParameterDefinition parameter = targetName == null
                            ? Method.Parameters[Method.HasThis ? targetIndex + 1 : targetIndex]
                            : Method.Parameters.First(x => x.Name == targetName);

                        toRemove.Add(ins.Previous);
                        //instructions.RemoveAt(--position);

                        FixLdargFor(parameter, ins);

                        if (!isGeneric)
                            return Instruction.Create(OpCodes.Box);
                        if (targetType != parameter.ParameterType)
                            throw new InvalidOperationException("Invalid generic argument given.");
                        return null;

                    case nameof(Context.Variable):
                        if (targetIndex < 0)
                            throw new InvalidOperationException("The given index must be positive.");
                        ins.OpCode = OpCodes.Ldloc_S;
                        ins.Operand = variables.First(x => x.Index == targetIndex);

                        toRemove.Add(ins.Previous);
                        //instructions.RemoveAt(--position);

                        if (!isGeneric)
                            return Instruction.Create(OpCodes.Box);
                        if (targetType != ((VariableDefinition)ins.Operand).VariableType)
                            throw new InvalidOperationException("Invalid generic argument given.");
                        return null;

                    case nameof(Context.This):
                        if (!Method.HasThis)
                            throw new InvalidOperationException("Cannot access this from a static method.");
                        ins.OpCode = OpCodes.Ldarg_0;

                        if (!isGeneric)
                            return Instruction.Create(OpCodes.Box);
                        if (targetType != Method.DeclaringType)
                            throw new InvalidOperationException("Invalid generic argument given.");
                        return null;
                }

                throw new NotSupportedException($"Context.{method.Name} is not supported.");
            }

            return null;
        }

        /// <summary>
        /// Insert the given <see cref="MethodBody"/> to this writer.
        /// </summary>
        /// <param name="body">The <see cref="MethodBody"/> to copy.</param>
        public ILWriter Body(MethodBody body)
        {
            // Save current position to later fix offsets.
            int initialVarCount = variables.Count;
            int initialPosition = position;

            // Copy variables...
            // Mono.Cecil already takes care of fixing variable indexes.
            foreach (var variable in body.Variables)
                variables.Add(variable);

            // Save variables locally for quick access.
            bool isTargetStatic = Method.IsStatic;
            bool mustChange = isTargetStatic != body.Method.IsStatic;
            List<Tuple<int, Instruction>> insToPush = new List<Tuple<int, Instruction>>();
            List<Instruction> toRemove = new List<Instruction>();

            // Copy body, making sure that all offsets are good.
            for (int i = 0; i < body.Instructions.Count; i++)
            {
                Instruction ins = body.Instructions[i].Clone();

                if (position != 0)
                    ins.Previous = instructions[position - 1];

                // If a ldloc.* instruction has been modified, no change will be made below.
                if (mustChange && UpdateInstruction(ins, isTargetStatic))
                {
                    this.instructions.Insert(position++, ins);

                    continue;
                }

                if (ins.OpCode.Code == Code.Call)
                {
                    // If UpdateInstruction returns false, it hasn't been updated.
                    // In this case, it might be a call to Context.

                    Instruction toPush = UpdateInstructionContext(ins, toRemove);

                    // It might have been updated; if so, it may return an instruction
                    // used to box the value returned by ldarg.* or ldloc.*.

                    this.instructions.Insert(position++, ins);

                    if (toPush != null)
                        insToPush.Add(Tuple.Create(position, toPush));

                    continue;
                }

                this.instructions.Insert(position++, ins);

                // Save position for fixups.
                Instruction operand = ins.Operand as Instruction;

                if (operand != null)
                    ins.Operand = new InstructionOffset(body.Instructions.IndexOf(operand));
                else
                {
                    // Fix variable indexes.
                    int loc;

                    if ((loc = FromLdloc(ins)) != -1)
                        FixLdlocFor(loc + initialVarCount, ins);
                    else if ((loc = FromStloc(ins)) != -1)
                        FixStlocFor(loc + initialVarCount, ins);
                }
            }

            // Fix branches.
            for (int i = initialPosition; i < position; i++)
            {
                Instruction ins = instructions[i];

                if (ins.Operand is InstructionOffset)
                {
                    int offset = ((InstructionOffset)ins.Operand).Offset;
                    Instruction next = instructions[offset + initialPosition];

                    if (toRemove.Contains(next))
                        next = next.Next;

                    ins.Operand = next;
                }
            }

            // Fix contexts.
            // I know, so many different loops that iterate over the same data...
            // I tried to only use one or two loops, but it makes everything much harder.
            for (int i = 0; i < insToPush.Count; i++)
            {
                Tuple<int, Instruction> toPush = insToPush[i];
                this.instructions.Insert(toPush.Item1 + i, toPush.Item2);
            }

            for (int i = 0; i < toRemove.Count; i++)
                this.instructions.Remove(toRemove[i]);

            position += insToPush.Count - toRemove.Count;

            for (int i = initialPosition; i < instructions.Count; i++)
                instructions[i].FixOffset();

            return this;
        }

        /// <summary>
        /// Insert the IL body of an <see cref="Action"/> to this writer.
        /// </summary>
        /// <param name="action">The action whose body will be copied.</param>
        /// <param name="removeRet">If <see langword="true"/>, all instructions with the <see cref="OpCodes.Ret"/> opcode will be removed.</param>
        public ILWriter Body(Action action, bool removeRet = true)
        {
            int posBefore = position;

            this.Body(action.GetMethodInfo().GetDefinition().Body);

            if (removeRet)
            {
                for (int i = posBefore; i < position; i++)
                {
                    if (instructions[i].OpCode == OpCodes.Ret)
                        this.instructions[i].OpCode = OpCodes.Nop;
                }
            }

            return this;
        }

        /// <summary>
        /// Insert the IL body of a <paramref name="method"/> to this writer.
        /// </summary>
        /// <param name="method">The method whose body will be copied.</param>
        public ILWriter Body(MethodInfo method) => this.Body(method.GetDefinition().Body);

        /// <summary>
        /// Insert the IL body of a <see langword="delegate"/> to this writer.
        /// </summary>
        /// <param name="del">The <see langword="delegate"/> whose body will be copied.</param>
        public ILWriter Body(Delegate del) => this.Body(del.GetMethodInfo());

        /// <summary>
        /// Insert the IL body of a <see langword="delegate"/> of
        /// type <typeparamref name="TDelegate"/> to this writer.
        /// </summary>
        /// <param name="del">The <see langword="delegate"/> whose body will be copied.</param>
        public ILWriter Body<TDelegate>(TDelegate del)
        {
            Delegate deleg = del as Delegate;
            if (deleg != null)
                return this.Body(deleg);
            throw new ArgumentException("Given object is not a delegate.", nameof(del));
        }
    }
}
