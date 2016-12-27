using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    /// <summary>
    /// Provides helper methods for analysis of a <see cref="MethodBody"/>.
    /// </summary>
    public static class BodyAnalysis
    {
        #region GetAccessedFields
        /// <inheritdoc cref="GetAccessedFields(ILWriter, bool, bool)"/>
        /// <param name="method">The method to check.</param>
        /// <param name="load"></param>
        /// <param name="save"></param>
        public static IEnumerable<FieldDefinition> GetAccessedFields(this MethodDefinition method, bool load, bool save)
            => GetAccessedFields(method.Write(), load, save);

        /// <summary>
        /// Load all fields accessed in the body of a method.
        /// </summary>
        /// <param name="il">The method body to check.</param>
        /// <param name="load">Whether or not <see cref="System.Reflection.Emit.OpCodes.Ldfld"/> and <see cref="System.Reflection.Emit.OpCodes.Ldsfld"/> instructions should be checked.</param>
        /// <param name="save">Whether or not <see cref="System.Reflection.Emit.OpCodes.Stfld"/> and <see cref="System.Reflection.Emit.OpCodes.Stsfld"/> instructions should be checked.</param>
        public static IEnumerable<FieldDefinition> GetAccessedFields(this ILWriter il, bool load, bool save)
        {
            if (!load && !save)
                yield break;

            OpCode[] checks = load && save
                ? new[] { OpCodes.Ldsfld, OpCodes.Ldfld, OpCodes.Stsfld, OpCodes.Stfld }
                : load
                    ? new[] { OpCodes.Ldsfld, OpCodes.Ldfld }
                    : new[] { OpCodes.Stsfld, OpCodes.Stfld };

            for (int i = 0; i < il.instructions.Count; i++)
            {
                Instruction ins = il.instructions[i];

                if (Array.IndexOf(checks, ins.OpCode) != -1)
                {
                    yield return (FieldDefinition)ins.Operand;
                }
            }
        }
        #endregion

        #region CountValuesOnStack
        /// <summary>
        /// Count the number of values available on the stack from
        /// <paramref name="start"/> to <paramref name="end"/>.
        /// </summary>
        public static int CountValuesOnStack(this IList<Instruction> instructions, int start = 0, int? end = null)
        {
            int total = 0;

            for (; start < end.GetValueOrDefault(instructions.Count); start++)
                total += instructions[start].AddedToStack();

            return total;
        }

        /// <summary>
        /// Count the number of values available on the stack
        /// at <see cref="ILWriter.Position"/>.
        /// </summary>
        public static int CountValuesOnStack(this ILWriter writer)
        {
            return CountValuesOnStack(writer.instructions, 0, writer.position);
        }

        /// <summary>
        /// Returns the number of values added to the stack by the given
        /// <see cref="OpCode"/>. This value can be negative.
        /// </summary>
        public static int AddedToStack(this Instruction ins)
        {
            OpCode opcode = ins.OpCode;

            if (opcode == OpCodes.Call || opcode == OpCodes.Calli || opcode == OpCodes.Callvirt)
            {
                MethodDefinition method = ins.Operand as MethodDefinition;
                if (method == null)
                    throw new NotSupportedException();

                return method.Parameters.Count + (method.ReturnType.Name == "Void" ? 0 : 1);
            }

            return opcode.StackBehaviourPop.AddedToStack() + opcode.StackBehaviourPush.AddedToStack();
        }

        /// <summary>
        /// Returns the number of values added to the stack by the given
        /// <see cref="OpCode"/>. This value can be negative.
        /// </summary>
        public static int AddedToStack(this OpCode opcode)
        {
            return opcode.StackBehaviourPop.AddedToStack() + opcode.StackBehaviourPush.AddedToStack();
        }

        /// <summary>
        /// Returns the number of values added to the stack by the given
        /// <see cref="System.Reflection.Emit.StackBehaviour"/>. This value can be negative.
        /// </summary>
        public static int AddedToStack(this StackBehaviour behaviour)
        {
            if (behaviour > StackBehaviour.Push0)
            {
                switch (behaviour)
                {
                    case StackBehaviour.Push1_push1:
                        return 2;
                    default:
                        return 1;
                }
            }

            switch (behaviour)
            {
                case StackBehaviour.Pop0:
                case StackBehaviour.Push0:
                    return 0;
                case StackBehaviour.Popi_pop1:
                case StackBehaviour.Popi_popi:
                case StackBehaviour.Popi_popi8:
                case StackBehaviour.Popi_popr4:
                case StackBehaviour.Popi_popr8:
                case StackBehaviour.Popref_pop1:
                case StackBehaviour.Pop1_pop1:
                case StackBehaviour.Popref_popi:
                    return -2;
                case StackBehaviour.Popi_popi_popi:
                case StackBehaviour.Popref_popi_popi:
                case StackBehaviour.Popref_popi_popi8:
                case StackBehaviour.Popref_popi_popr4:
                case StackBehaviour.Popref_popi_popr8:
                case StackBehaviour.Popref_popi_popref:
                    return -3;
                default:
                    return -1;
            }
        }
        #endregion

        #region FindMatchingMethod
        /// <summary>
        /// Find the method whose name is <paramref name="methodName"/>, and whose
        /// parameters match the values currently on the stack.
        /// </summary>
        /// <param name="il">The <see cref="ILWriter"/> whose stack shall be checked for a match.</param>
        /// <param name="declaringType">The type whose methods shall be checked.</param>
        /// <param name="methodName">The name of the method to find.</param>
        /// <param name="fast">
        /// If <see langword="false"/>, this method will attempt to find the method that matches the number of
        /// arguments on the stack the best, ie that has the most matching parameters.
        /// Otherwise, it will return the first match.
        /// </param>
        public static MethodDefinition FindMatchingMethod(this ILWriter il, TypeDefinition declaringType, string methodName, bool fast = false)
        {
            Stack<TypeDefinition> valuesTypesStack = new Stack<TypeDefinition>(0);
            TypeDefinition[] valuesTypes = new TypeDefinition[0];
            int lastCheckIndex = il.position;
            MethodDefinition found = null;

            for (int i = 0; i < declaringType.Methods.Count; i++)
            {
                MethodDefinition method = declaringType.Methods[i];
                int paramCount = method.Parameters.Count;

                // If we already have a match, only proceed if the method has more parameters.
                if (found != null && paramCount < found.Parameters.Count)
                    continue;

                // Ensure the number of types found at the top of the stack is big enough for those parameters.
                if (paramCount > valuesTypes.Length)
                {
                    lastCheckIndex = UpdateStack(valuesTypesStack, method, lastCheckIndex, il, method.Parameters.Count);
                    valuesTypes = valuesTypesStack.ToArray();
                }

                // If the number of types is still inferior to the number of parameters, the method has too many parameters.
                if (paramCount > valuesTypes.Length)
                    continue;

                // Make sure all parameters have the same type.
                for (int o = 0; o < paramCount; o++)
                {
                    if (method.Parameters[o].ParameterType != valuesTypes[o])
                        goto NoMatch;
                }

                // We have a match!
                // If we're going fast, immediately return.
                if (fast)
                    break;

                // Else, keep going for a better match.
                found = method;

                NoMatch: ;
            }

            return found;
        }

        private static int UpdateStack(Stack<TypeDefinition> stack, MethodDefinition method, int lastPosition, ILWriter il, int length)
        {
            int newArrayIndex = stack.Count;

            // Loop as long as we're not at the start of the body,
            // and we still need to find types.
            // Note that we're going from the current position to the beginning.
            for (; lastPosition != 0 && newArrayIndex < length; lastPosition--)
            {
                Instruction ins = il.instructions[lastPosition];

                int popped = ins.OpCode.StackBehaviourPop.AddedToStack();
                //while ()
                    


                stack.Push(ins.GetReturnType(method));

            }

            return lastPosition;
        }

        /// <summary>
        /// Get the return type of an <paramref name="instruction"/>.
        /// Returns <see langword="null"/> if the <paramref name="instruction"/> returns <see langword="void"/>.
        /// </summary>
        public static TypeDefinition GetReturnType(this Instruction instruction, MethodDefinition method)
        {
            OpCode opcode = instruction.OpCode;

            // TODO: Finish this method.
            //  - Check return type of pretty much all Ld* methods, plus Calls.
            throw new NotImplementedException("This method has not yet been implemented.");
        }
        #endregion
    }
}
