using System;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        private int IndexOf(Instruction instruction)
        {
            int index = instructions.IndexOf(instruction);

            if (index == -1)
                throw new ArgumentException("The given instruction is not a part of this ILWriter's body.", nameof(instruction));

            return index;
        }

        /// <summary>
        /// Updates an ldarg.* instruction to account for
        /// <see langword="static"/> methods.
        /// </summary>
        private static bool UpdateInstruction(Instruction ins, bool toStatic)
        {
            if (toStatic)
            {
                switch (ins.OpCode.Code)
                {
                    case Code.Ldarg_0:
                        throw new InvalidOperationException("Cannot access this from a static method.");
                    case Code.Ldarg_1:
                        ins.OpCode = OpCodes.Ldarg_0; break;
                    case Code.Ldarg_2:
                        ins.OpCode = OpCodes.Ldarg_1; break;
                    case Code.Ldarg_3:
                        ins.OpCode = OpCodes.Ldarg_2; break;
                    case Code.Ldarg_S:
                        if ((int)ins.Operand == 4)
                        {
                            ins.OpCode = OpCodes.Ldarg_3;
                            ins.Operand = null;
                        }
                        else
                            ins.Operand = (int)ins.Operand - 1;
                        break;
                    case Code.Ldarg:
                        if ((int)ins.Operand == sbyte.MaxValue)
                            ins.OpCode = OpCodes.Ldarg_S;
                        ins.Operand = (int)ins.Operand - 1;
                        break;
                    default:
                        return false;
                }

                return true;
            }

            switch (ins.OpCode.Code)
            {
                case Code.Ldarg_0:
                    ins.OpCode = OpCodes.Ldarg_1; break;
                case Code.Ldarg_1:
                    ins.OpCode = OpCodes.Ldarg_2; break;
                case Code.Ldarg_2:
                    ins.OpCode = OpCodes.Ldarg_3; break;
                case Code.Ldarg_3:
                    ins.OpCode = OpCodes.Ldarg_S;
                    ins.Operand = 4;
                    break;
                case Code.Ldarg_S:
                    if ((int)ins.Operand == sbyte.MaxValue)
                        ins.OpCode = OpCodes.Ldarg;
                    ins.Operand = (int)ins.Operand + 1;
                    break;
                case Code.Ldarg:
                    ins.Operand = (int)ins.Operand + 1;
                    break;
                default:
                    return false;
            }

            return true;
        }

        private static Instruction InstructionFor(int i)
        {
            switch (i)
            {
                case -1:
                    return Instruction.Create(OpCodes.Ldc_I4_M1);
                case 0:
                    return Instruction.Create(OpCodes.Ldc_I4_0);
                case 1:
                    return Instruction.Create(OpCodes.Ldc_I4_1);
                case 2:
                    return Instruction.Create(OpCodes.Ldc_I4_2);
                case 3:
                    return Instruction.Create(OpCodes.Ldc_I4_3);
                case 4:
                    return Instruction.Create(OpCodes.Ldc_I4_4);
                case 5:
                    return Instruction.Create(OpCodes.Ldc_I4_5);
                case 6:
                    return Instruction.Create(OpCodes.Ldc_I4_6);
                case 7:
                    return Instruction.Create(OpCodes.Ldc_I4_7);
                case 8:
                    return Instruction.Create(OpCodes.Ldc_I4_8);
                default:
                    return Instruction.Create(i > sbyte.MaxValue ? OpCodes.Ldc_I4 : OpCodes.Ldc_I4_S, i);
            }
        }

        internal static Instruction[] InstructionsForConstant(object obj)
        {
            if (obj == null)
                return new[] { Instruction.Create(OpCodes.Ldnull) };

            Type type = obj.GetType();
            string ns = type.Namespace;

            if (ns == "System")
            {
                // Constant
                switch (type.Name)
                {
                    case nameof(Boolean):
                        return new[] { Instruction.Create((bool)obj ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0) };
                    case nameof(String):
                        return new[] { Instruction.Create(OpCodes.Ldstr, (string)obj) };

                    case nameof(Int16):
                        return new[] { InstructionFor((int)obj), Instruction.Create(OpCodes.Conv_I2) };
                    case nameof(Int32):
                        return new[] { InstructionFor((int)obj) };
                    case nameof(Int64):
                        return new[] { Instruction.Create(OpCodes.Ldc_I8, (long)obj) };

                    case nameof(UInt16):
                        return new[] { InstructionFor((int)obj), Instruction.Create(OpCodes.Conv_U2) };
                    case nameof(UInt32):
                        return new[] { InstructionFor((int)obj), Instruction.Create(OpCodes.Conv_U4) };
                    case nameof(UInt64):
                        return new[] { Instruction.Create(OpCodes.Ldc_I8, (long)obj), Instruction.Create(OpCodes.Conv_U8) };
                        
                    case nameof(SByte):
                        return new[] { InstructionFor((int)obj), Instruction.Create(OpCodes.Conv_I1) };
                    case nameof(Byte):
                        return new[] { InstructionFor((int)obj), Instruction.Create(OpCodes.Conv_U1) };

                    case nameof(Double):
                        return new[] { Instruction.Create(OpCodes.Ldc_R8, (double)obj) };
                    case nameof(Single):
                        return new[] { Instruction.Create(OpCodes.Ldc_R4, (float)obj) };
                }
            }

            throw new NotSupportedException();
        }
    }
}
