using System;
using System.Linq;
using System.Reflection.Emit;
using Mono.Cecil.Cil;
using OpCode = System.Reflection.Emit.OpCode;
using OpCodes = System.Reflection.Emit.OpCodes;
using System.Reflection;
using Mono.Cecil;

namespace Blur
{
    partial class BlurExtensions
    {
        private static readonly ConstructorInfo LabelCtor
            = typeof(Label).GetTypeInfo().DeclaredConstructors.First(x => x.GetParameters().Length == 1);

        private static Label CreateLabel(int label) => (Label)LabelCtor.Invoke(new object[] { label });

        /// <summary>
        /// Fix the offset of an <see cref="Instruction"/>.
        /// </summary>
        internal static int FixOffset(this Instruction ins)
        {
            Instruction prev = ins.Previous;
            // ReSharper disable once MergeConditionalExpression
            return ins.Offset = prev == null ? 0 : prev.Offset + prev.GetSize();
        }

        /// <summary>
        /// Emits the given instruction to an <see cref="ILGenerator"/>.
        /// </summary>
        public static void Emit(this Instruction ins, ILGenerator il)
        {
            string opname = ins.OpCode.Name == "tail" ? "Tailcall" : ins.OpCode.Name.Replace('.', '_');

            OpCode opcode = (OpCode)typeof(OpCodes).GetTypeInfo()
                .DeclaredFields.First(x => x.Name.Equals(opname, StringComparison.OrdinalIgnoreCase))
                .GetValue(null);

            object operand = ins.Operand;

            if (operand == null)
                il.Emit(opcode);

            else if (operand.GetType().Namespace.StartsWith("Mono.Cecil"))
            {
                if (operand is ParameterReference)
                    il.Emit(OpCodes.Ldarga_S, (byte)((ParameterReference)operand).Resolve().Sequence);
                else if (operand is TypeReference)
                    il.Emit(opcode, ((TypeReference)operand).AsType());
                else if (operand is FieldReference)
                    il.Emit(opcode, ((FieldReference)operand).Resolve().AsInfo());
                else if (operand is MethodReference)
                {
                    MethodDefinition method = ((MethodReference)operand).Resolve();

                    if (method.IsConstructor)
                        il.Emit(opcode, (ConstructorInfo)method.GetMethod());
                    else
                        il.Emit(opcode, (MethodInfo)method.GetMethod());
                }
                else if (operand is Instruction)
                    il.Emit(opcode, ((Instruction)operand).FixOffset());
                else if (operand is Instruction[])
                    il.Emit(opcode, ((Instruction[])operand).Convert(x => CreateLabel(x.FixOffset())).ToArray());
                else if (operand is VariableReference)
                    il.Emit(opcode, ((VariableDefinition)operand).Index);
                else if (operand is CallSite)
                {
                    CallSite callSite = (CallSite)operand;
                    il.EmitCalli(opcode, (CallingConventions)(int)callSite.CallingConvention,
                        callSite.ReturnType.AsType(),
                        callSite.Parameters.Convert(x => x.ParameterType.AsType()), new Type[0]);
                }
                else
                    // If we arrive here, somebody's been messing with Reflection...
                    throw new NotSupportedException();
            }

            else if (operand is byte)
                il.Emit(opcode, (byte)operand);
            else if (operand is sbyte)
                il.Emit(opcode, (sbyte)operand);

            else if (operand is float)
                il.Emit(opcode, (float)operand);
            else if (operand is double)
                il.Emit(opcode, (double)operand);

            else if (operand is short)
                il.Emit(opcode, (short)operand);
            else if (operand is int)
                il.Emit(opcode, (int)operand);
            else if (operand is long)
                il.Emit(opcode, (long)operand);

            else if (operand is string)
                il.Emit(opcode, (string)operand);

            else
                // If we arrive here, somebody's been messing with Reflection...
                throw new NotSupportedException();
        }
    }
}
