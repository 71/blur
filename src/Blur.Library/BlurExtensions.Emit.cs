using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Mono.Cecil.Cil;
using OpCode = System.Reflection.Emit.OpCode;
using OpCodes = System.Reflection.Emit.OpCodes;
using CecilOpCode = Mono.Cecil.Cil.OpCode;
using CecilOpCodes = Mono.Cecil.Cil.OpCodes;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
        /// <summary>
        /// Emits the given instruction to an <see cref="ILGenerator"/>.
        /// </summary>
        public static void Emit(this Instruction ins, ILGenerator il)
        {
            OpCode opcode = (OpCode)typeof(OpCodes).GetTypeInfo()
                .GetDeclaredField(ins.OpCode.Name == "Tail" ? "Tailcall" : ins.OpCode.Name)
                .GetValue(null);

            object operand = ins.Operand;

            if (operand == null)
                il.Emit(opcode);

            else if (operand.GetType().Namespace == nameof(Blur))
            {
                if (operand is ParameterDefinition)
                    il.Emit(OpCodes.Ldarga_S, (byte)((ParameterDefinition)operand).Sequence);
                if (operand is TypeDefinition)
                    il.Emit(opcode, ((TypeDefinition)operand).AsType());
                if (operand is FieldDefinition)
                    il.Emit(opcode, ((FieldDefinition)operand).AsInfo());

                if (operand is MethodDefinition)
                {
                    MethodDefinition method = (MethodDefinition)operand;

                    if (method.IsConstructor)
                        il.Emit(opcode, (ConstructorInfo)method.AsInfo());
                    else
                        il.Emit(opcode, (MethodInfo)method.AsInfo());
                }
            }

            else if (operand is byte)
                il.Emit(opcode, (byte)operand);
            else if (operand is sbyte)
                il.Emit(opcode, (sbyte)operand);

            else if (operand is float)
                il.Emit(opcode, (float)operand);
            else if (operand is double)
                il.Emit(opcode, (double)operand);

            else if (operand is int)
                il.Emit(opcode, (int)operand);
            else if (operand is long)
                il.Emit(opcode, (long)operand);

            else if (operand is string)
                il.Emit(opcode, (string)operand);

            else if (operand is Instruction)
                il.Emit(opcode, ((Instruction)operand).Offset);
            else if (operand is VariableDefinition)
                il.Emit(opcode, ((VariableDefinition)operand).Index);

            // If we arrive here, somebody's been messing with Reflection...
            throw new NotSupportedException();
        }
    }
}
