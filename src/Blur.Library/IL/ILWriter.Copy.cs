using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

using Arr = System.Array;

namespace Blur
{
    partial class ILWriter
    {
        /// <summary>
        /// Copy the content of a <see cref="MethodBody"/> to another <see cref="MethodBody"/>,
        /// optionally converting <see cref="Instruction"/>s.
        /// </summary>
        public static void Copy(MethodBody src, MethodBody target, int index = 0, Func<Instruction, Instruction[]> modify = null)
        {
            var srcInstr = src.Instructions;
            var tgtInstr = target.Instructions;

            var tgtParams = target.Method.Parameters;

            int originalVarCount = src.Variables.Count;

            // Adapt target method
            if (src.MaxStackSize > target.MaxStackSize)
                target.MaxStackSize = src.MaxStackSize;

            // Copy variables
            for (int i = 0; i < src.Variables.Count; i++)
                target.Variables.Add(src.Variables[i].Clone());

            // Copy instructions
            Instruction[] tmpInstr = new Instruction[srcInstr.Count];

            for (int i = 0; i < tmpInstr.Length; i++)
            {
                Instruction ins = tmpInstr[i] = srcInstr[i].Clone();

                // Fix variable?
                if (ins.Operand is VariableDefinition variable)
                {
                    ins.Operand = target.Variables[variable.Index + originalVarCount];
                }

                // Fix parameter?
                else if (ins.Operand is ParameterDefinition parameter)
                {
                    if (tgtParams.Count < parameter.Index)
                        throw new ArgumentException("Target method does not have matching parameters.", nameof(target));

                    ins.Operand = tgtParams[parameter.Index];
                }

                // Mark instruction?
                else if (ins.Operand is Instruction instruction)
                {
                    ins.Operand = new InstructionOffset(srcInstr.IndexOf(instruction));
                }
                else if (ins.Operand is Instruction[] instructions)
                {
                    ins.Operand = instructions.Convert(x => new InstructionOffset(srcInstr.IndexOf(x)));
                }
            }

            // Copy exception handlers
            for (int i = 0; i < src.ExceptionHandlers.Count; i++)
            {
                ExceptionHandler srcHandler = src.ExceptionHandlers[i];
                ExceptionHandler handler = new ExceptionHandler(srcHandler.HandlerType)
                {
                    CatchType = srcHandler.CatchType,
                    TryStart  = Arr.Find(tmpInstr, x => x.Offset == srcHandler.TryStart.Offset),
                    TryEnd    = Arr.Find(tmpInstr, x => x.Offset == srcHandler.TryEnd.Offset),

                    HandlerStart = Arr.Find(tmpInstr, x => x.Offset == srcHandler.HandlerStart.Offset),
                    HandlerEnd   = Arr.Find(tmpInstr, x => x.Offset == srcHandler.HandlerEnd.Offset),
                    FilterStart  = Arr.Find(tmpInstr, x => x.Offset == srcHandler.FilterStart.Offset)
                };

                target.ExceptionHandlers.Add(handler);
            }

            // Fix copied instructions
            for (int i = 0; i < tmpInstr.Length; i++)
            {
                Instruction ins = tmpInstr[i];

                // If the instruction was marked as an instruction offset,
                // replace its operand by an actual offset.
                if (ins.Operand is InstructionOffset insOffset)
                {
                    ins.Operand = tmpInstr[insOffset.Offset];
                }
                else if (ins.Operand is InstructionOffset[] insOffsets)
                {
                    ins.Operand = insOffsets.Convert(offset => tmpInstr[offset.Offset]);
                }

                // Either write the instruction directly, or convert
                // it using the given modifier.
                Instruction[] newInstr = modify?.Invoke(ins);

                // If null is returned, simply print things normally.
                if (newInstr == null)
                    tgtInstr.Insert(index++, ins);
                else
                    for (int o = 0; o < newInstr.Length; o++)
                        tgtInstr.Insert(index++, newInstr[o]);
            }

            // Copy debug infos
            //MethodDebugInformation srcDebugInfo = src.Method.DebugInformation;
            //MethodDebugInformation tgtDebugInfo = target.Method.DebugInformation;

            //if (srcDebugInfo.StateMachineKickOffMethod != null)
            //    tgtDebugInfo.StateMachineKickOffMethod = srcDebugInfo.StateMachineKickOffMethod;

            //if (srcDebugInfo.Scope != null)
            //    tgtDebugInfo.Scope = srcDebugInfo.Scope;

            //if (srcDebugInfo.HasSequencePoints)
            //{
            //    for (int i = 0; i < srcDebugInfo.SequencePoints.Count; i++)
            //        tgtDebugInfo.SequencePoints.Add(srcDebugInfo.SequencePoints[i]);
            //}
        }
    }
}
