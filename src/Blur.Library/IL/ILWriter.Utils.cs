using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
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
    }
}
