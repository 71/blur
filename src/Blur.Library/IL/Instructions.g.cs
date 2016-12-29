





using System.ComponentModel;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Extensions
{
    /// <summary>Provides extension methods to easily emit an <see cref="Instruction"/> to an <see cref="ILWriter"/>.</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ILWriterExtensions
    {
        // Instructions found on https://en.wikipedia.org/wiki/List_of_CIL_instructions
        // License: https://creativecommons.org/licenses/by-sa/4.0/
        // Script used:
        //
        //  var table = document.querySelector('.wikitable');
        //  var names = table.querySelectorAll('td:nth-child(2)');
        //  var dscrs = table.querySelectorAll('td:nth-child(3)');
        //  var str = "";
        //  for (var i = 0; i < names.length; i++)
        //      str += '{ "' + names[i].innerText.trim() + '", "' + dscrs[i].innerText.trim() + '" },\n';
        //  return str;
        //

        

        #region 
            

        /// <summary>
        /// Add two values, returning a new value.
        /// </summary>
        public static ILWriter Add(this ILWriter il)
            => il.Emit(OpCodes.Add);
                

        /// <summary>
        /// Add signed integer values with overflow check.
        /// </summary>
        public static ILWriter Add_Ovf(this ILWriter il)
            => il.Emit(OpCodes.Add_Ovf);
                

        /// <summary>
        /// Add unsigned integer values with overflow check.
        /// </summary>
        public static ILWriter Add_Ovf_Un(this ILWriter il)
            => il.Emit(OpCodes.Add_Ovf_Un);
                

        /// <summary>
        /// Bitwise AND of two integral values, returns an integral value.
        /// </summary>
        public static ILWriter And(this ILWriter il)
            => il.Emit(OpCodes.And);
                

        /// <summary>
        /// Return argument list handle for the current method.
        /// </summary>
        public static ILWriter Arglist(this ILWriter il)
            => il.Emit(OpCodes.Arglist);
                

        /// <summary>
        /// Inform a debugger that a breakpoint has been reached.
        /// </summary>
        public static ILWriter Break(this ILWriter il)
            => il.Emit(OpCodes.Break);
                

        /// <summary>
        /// Push 1 (of type int32) if value1 equals value2, else push 0.
        /// </summary>
        public static ILWriter Ceq(this ILWriter il)
            => il.Emit(OpCodes.Ceq);
                

        /// <summary>
        /// Push 1 (of type int32) if value1 &gt; value2, else push 0.
        /// </summary>
        public static ILWriter Cgt(this ILWriter il)
            => il.Emit(OpCodes.Cgt);
                

        /// <summary>
        /// Push 1 (of type int32) if value1 &gt; value2, unsigned or unordered, else push 0.
        /// </summary>
        public static ILWriter Cgt_Un(this ILWriter il)
            => il.Emit(OpCodes.Cgt_Un);
                

        /// <summary>
        /// Throw ArithmeticException if value is not a finite number.
        /// </summary>
        public static ILWriter Ckfinite(this ILWriter il)
            => il.Emit(OpCodes.Ckfinite);
                

        /// <summary>
        /// Push 1 (of type int32) if value1 &lt; value2, else push 0.
        /// </summary>
        public static ILWriter Clt(this ILWriter il)
            => il.Emit(OpCodes.Clt);
                

        /// <summary>
        /// Push 1 (of type int32) if value1 &lt; value2, unsigned or unordered, else push 0.
        /// </summary>
        public static ILWriter Clt_Un(this ILWriter il)
            => il.Emit(OpCodes.Clt_Un);
                

        /// <summary>
        /// Convert to native int, pushing native int on stack.
        /// </summary>
        public static ILWriter Conv_I(this ILWriter il)
            => il.Emit(OpCodes.Conv_I);
                

        /// <summary>
        /// Convert to int8, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_I1(this ILWriter il)
            => il.Emit(OpCodes.Conv_I1);
                

        /// <summary>
        /// Convert to int16, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_I2(this ILWriter il)
            => il.Emit(OpCodes.Conv_I2);
                

        /// <summary>
        /// Convert to int32, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_I4(this ILWriter il)
            => il.Emit(OpCodes.Conv_I4);
                

        /// <summary>
        /// Convert to int64, pushing int64 on stack.
        /// </summary>
        public static ILWriter Conv_I8(this ILWriter il)
            => il.Emit(OpCodes.Conv_I8);
                

        /// <summary>
        /// Convert to a native int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I);
                

        /// <summary>
        /// Convert unsigned to a native int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I_Un);
                

        /// <summary>
        /// Convert to an int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I1(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I1);
                

        /// <summary>
        /// Convert unsigned to an int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I1_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I1_Un);
                

        /// <summary>
        /// Convert to an int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I2(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I2);
                

        /// <summary>
        /// Convert unsigned to an int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I2_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I2_Un);
                

        /// <summary>
        /// Convert to an int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I4(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I4);
                

        /// <summary>
        /// Convert unsigned to an int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I4_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I4_Un);
                

        /// <summary>
        /// Convert to an int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I8(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I8);
                

        /// <summary>
        /// Convert unsigned to an int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_I8_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_I8_Un);
                

        /// <summary>
        /// Convert to a native unsigned int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U);
                

        /// <summary>
        /// Convert unsigned to a native unsigned int (on the stack as native int) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U_Un);
                

        /// <summary>
        /// Convert to an unsigned int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U1(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U1);
                

        /// <summary>
        /// Convert unsigned to an unsigned int8 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U1_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U1_Un);
                

        /// <summary>
        /// Convert to an unsigned int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U2(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U2);
                

        /// <summary>
        /// Convert unsigned to an unsigned int16 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U2_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U2_Un);
                

        /// <summary>
        /// Convert to an unsigned int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U4(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U4);
                

        /// <summary>
        /// Convert unsigned to an unsigned int32 (on the stack as int32) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U4_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U4_Un);
                

        /// <summary>
        /// Convert to an unsigned int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U8(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U8);
                

        /// <summary>
        /// Convert unsigned to an unsigned int64 (on the stack as int64) and throw an exception on overflow.
        /// </summary>
        public static ILWriter Conv_Ovf_U8_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_Ovf_U8_Un);
                

        /// <summary>
        /// Convert unsigned integer to floating-point, pushing F on stack.
        /// </summary>
        public static ILWriter Conv_R_Un(this ILWriter il)
            => il.Emit(OpCodes.Conv_R_Un);
                

        /// <summary>
        /// Convert to float32, pushing F on stack.
        /// </summary>
        public static ILWriter Conv_R4(this ILWriter il)
            => il.Emit(OpCodes.Conv_R4);
                

        /// <summary>
        /// Convert to float64, pushing F on stack.
        /// </summary>
        public static ILWriter Conv_R8(this ILWriter il)
            => il.Emit(OpCodes.Conv_R8);
                

        /// <summary>
        /// Convert to native unsigned int, pushing native int on stack.
        /// </summary>
        public static ILWriter Conv_U(this ILWriter il)
            => il.Emit(OpCodes.Conv_U);
                

        /// <summary>
        /// Convert to unsigned int8, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_U1(this ILWriter il)
            => il.Emit(OpCodes.Conv_U1);
                

        /// <summary>
        /// Convert to unsigned int16, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_U2(this ILWriter il)
            => il.Emit(OpCodes.Conv_U2);
                

        /// <summary>
        /// Convert to unsigned int32, pushing int32 on stack.
        /// </summary>
        public static ILWriter Conv_U4(this ILWriter il)
            => il.Emit(OpCodes.Conv_U4);
                

        /// <summary>
        /// Convert to unsigned int64, pushing int64 on stack.
        /// </summary>
        public static ILWriter Conv_U8(this ILWriter il)
            => il.Emit(OpCodes.Conv_U8);
                

        /// <summary>
        /// Copy data from memory to memory.
        /// </summary>
        public static ILWriter Cpblk(this ILWriter il)
            => il.Emit(OpCodes.Cpblk);
                

        /// <summary>
        /// Divide two values to return a quotient or floating-point result.
        /// </summary>
        public static ILWriter Div(this ILWriter il)
            => il.Emit(OpCodes.Div);
                

        /// <summary>
        /// Divide two values, unsigned, returning a quotient.
        /// </summary>
        public static ILWriter Div_Un(this ILWriter il)
            => il.Emit(OpCodes.Div_Un);
                

        /// <summary>
        /// Duplicate the value on the top of the stack.
        /// </summary>
        public static ILWriter Dup(this ILWriter il)
            => il.Emit(OpCodes.Dup);
                

        /// <summary>
        /// End an exception handling filter clause.
        /// </summary>
        public static ILWriter Endfilter(this ILWriter il)
            => il.Emit(OpCodes.Endfilter);
                

        /// <summary>
        /// End finally clause of an exception block.
        /// </summary>
        public static ILWriter Endfinally(this ILWriter il)
            => il.Emit(OpCodes.Endfinally);
                

        /// <summary>
        /// Set all bytes in a block of memory to a given byte value.
        /// </summary>
        public static ILWriter Initblk(this ILWriter il)
            => il.Emit(OpCodes.Initblk);
                

        /// <summary>
        /// Load argument 0 onto the stack.
        /// </summary>
        public static ILWriter Ldarg_0(this ILWriter il)
            => il.Emit(OpCodes.Ldarg_0);
                

        /// <summary>
        /// Load argument 1 onto the stack.
        /// </summary>
        public static ILWriter Ldarg_1(this ILWriter il)
            => il.Emit(OpCodes.Ldarg_1);
                

        /// <summary>
        /// Load argument 2 onto the stack.
        /// </summary>
        public static ILWriter Ldarg_2(this ILWriter il)
            => il.Emit(OpCodes.Ldarg_2);
                

        /// <summary>
        /// Load argument 3 onto the stack.
        /// </summary>
        public static ILWriter Ldarg_3(this ILWriter il)
            => il.Emit(OpCodes.Ldarg_3);
                

        /// <summary>
        /// Push 0 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_0(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_0);
                

        /// <summary>
        /// Push 1 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_1(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_1);
                

        /// <summary>
        /// Push 2 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_2(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_2);
                

        /// <summary>
        /// Push 3 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_3(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_3);
                

        /// <summary>
        /// Push 4 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_4(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_4);
                

        /// <summary>
        /// Push 5 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_5(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_5);
                

        /// <summary>
        /// Push 6 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_6(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_6);
                

        /// <summary>
        /// Push 7 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_7(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_7);
                

        /// <summary>
        /// Push 8 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_8(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_8);
                

        /// <summary>
        /// Push -1 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4_M1(this ILWriter il)
            => il.Emit(OpCodes.Ldc_I4_M1);
                

        /// <summary>
        /// Indirect load value of type native int as native int on the stack
        /// </summary>
        public static ILWriter Ldind_I(this ILWriter il)
            => il.Emit(OpCodes.Ldind_I);
                

        /// <summary>
        /// Indirect load value of type int8 as int32 on the stack.
        /// </summary>
        public static ILWriter Ldind_I1(this ILWriter il)
            => il.Emit(OpCodes.Ldind_I1);
                

        /// <summary>
        /// Indirect load value of type int16 as int32 on the stack.
        /// </summary>
        public static ILWriter Ldind_I2(this ILWriter il)
            => il.Emit(OpCodes.Ldind_I2);
                

        /// <summary>
        /// Indirect load value of type int32 as int32 on the stack.
        /// </summary>
        public static ILWriter Ldind_I4(this ILWriter il)
            => il.Emit(OpCodes.Ldind_I4);
                

        /// <summary>
        /// Indirect load value of type int64 as int64 on the stack.
        /// </summary>
        public static ILWriter Ldind_I8(this ILWriter il)
            => il.Emit(OpCodes.Ldind_I8);
                

        /// <summary>
        /// Indirect load value of type float32 as F on the stack.
        /// </summary>
        public static ILWriter Ldind_R4(this ILWriter il)
            => il.Emit(OpCodes.Ldind_R4);
                

        /// <summary>
        /// Indirect load value of type float64 as F on the stack.
        /// </summary>
        public static ILWriter Ldind_R8(this ILWriter il)
            => il.Emit(OpCodes.Ldind_R8);
                

        /// <summary>
        /// Indirect load value of type object ref as O on the stack.
        /// </summary>
        public static ILWriter Ldind_Ref(this ILWriter il)
            => il.Emit(OpCodes.Ldind_Ref);
                

        /// <summary>
        /// Indirect load value of type unsigned int8 as int32 on the stack
        /// </summary>
        public static ILWriter Ldind_U1(this ILWriter il)
            => il.Emit(OpCodes.Ldind_U1);
                

        /// <summary>
        /// Indirect load value of type unsigned int16 as int32 on the stack
        /// </summary>
        public static ILWriter Ldind_U2(this ILWriter il)
            => il.Emit(OpCodes.Ldind_U2);
                

        /// <summary>
        /// Indirect load value of type unsigned int32 as int32 on the stack
        /// </summary>
        public static ILWriter Ldind_U4(this ILWriter il)
            => il.Emit(OpCodes.Ldind_U4);
                

        /// <summary>
        /// Push the length (of type native unsigned int) of array on the stack.
        /// </summary>
        public static ILWriter Ldlen(this ILWriter il)
            => il.Emit(OpCodes.Ldlen);
                

        /// <summary>
        /// Load local variable 0 onto stack.
        /// </summary>
        public static ILWriter Ldloc_0(this ILWriter il)
            => il.Emit(OpCodes.Ldloc_0);
                

        /// <summary>
        /// Load local variable 1 onto stack.
        /// </summary>
        public static ILWriter Ldloc_1(this ILWriter il)
            => il.Emit(OpCodes.Ldloc_1);
                

        /// <summary>
        /// Load local variable 2 onto stack.
        /// </summary>
        public static ILWriter Ldloc_2(this ILWriter il)
            => il.Emit(OpCodes.Ldloc_2);
                

        /// <summary>
        /// Load local variable 3 onto stack.
        /// </summary>
        public static ILWriter Ldloc_3(this ILWriter il)
            => il.Emit(OpCodes.Ldloc_3);
                

        /// <summary>
        /// Push a null reference on the stack.
        /// </summary>
        public static ILWriter Ldnull(this ILWriter il)
            => il.Emit(OpCodes.Ldnull);
                

        /// <summary>
        /// Allocate space from the local memory pool.
        /// </summary>
        public static ILWriter Localloc(this ILWriter il)
            => il.Emit(OpCodes.Localloc);
                

        /// <summary>
        /// Multiply values.
        /// </summary>
        public static ILWriter Mul(this ILWriter il)
            => il.Emit(OpCodes.Mul);
                

        /// <summary>
        /// Multiply signed integer values. Signed result shall fit in same size
        /// </summary>
        public static ILWriter Mul_Ovf(this ILWriter il)
            => il.Emit(OpCodes.Mul_Ovf);
                

        /// <summary>
        /// Multiply unsigned integer values. Unsigned result shall fit in same size
        /// </summary>
        public static ILWriter Mul_Ovf_Un(this ILWriter il)
            => il.Emit(OpCodes.Mul_Ovf_Un);
                

        /// <summary>
        /// Negate value.
        /// </summary>
        public static ILWriter Neg(this ILWriter il)
            => il.Emit(OpCodes.Neg);
                

        /// <summary>
        /// Do nothing (No operation).
        /// </summary>
        public static ILWriter Nop(this ILWriter il)
            => il.Emit(OpCodes.Nop);
                

        /// <summary>
        /// Bitwise complement (logical not).
        /// </summary>
        public static ILWriter Not(this ILWriter il)
            => il.Emit(OpCodes.Not);
                

        /// <summary>
        /// Bitwise OR of two integer values, returns an integer.
        /// </summary>
        public static ILWriter Or(this ILWriter il)
            => il.Emit(OpCodes.Or);
                

        /// <summary>
        /// Pop value from the stack.
        /// </summary>
        public static ILWriter Pop(this ILWriter il)
            => il.Emit(OpCodes.Pop);
                

        /// <summary>
        /// Specify that the subsequent array address operation performs no type check at runtime, and that it returns a controlled-mutability managed pointer
        /// </summary>
        public static ILWriter Readonly(this ILWriter il)
            => il.Emit(OpCodes.Readonly);
                

        /// <summary>
        /// Push the type token stored in a typed reference.
        /// </summary>
        public static ILWriter Refanytype(this ILWriter il)
            => il.Emit(OpCodes.Refanytype);
                

        /// <summary>
        /// Remainder when dividing one value by another.
        /// </summary>
        public static ILWriter Rem(this ILWriter il)
            => il.Emit(OpCodes.Rem);
                

        /// <summary>
        /// Remainder when dividing one unsigned value by another.
        /// </summary>
        public static ILWriter Rem_Un(this ILWriter il)
            => il.Emit(OpCodes.Rem_Un);
                

        /// <summary>
        /// Return from method, possibly with a value.
        /// </summary>
        public static ILWriter Ret(this ILWriter il)
            => il.Emit(OpCodes.Ret);
                

        /// <summary>
        /// Rethrow the current exception.
        /// </summary>
        public static ILWriter Rethrow(this ILWriter il)
            => il.Emit(OpCodes.Rethrow);
                

        /// <summary>
        /// Shift an integer left (shifting in zeros), return an integer.
        /// </summary>
        public static ILWriter Shl(this ILWriter il)
            => il.Emit(OpCodes.Shl);
                

        /// <summary>
        /// Shift an integer right (shift in sign), return an integer.
        /// </summary>
        public static ILWriter Shr(this ILWriter il)
            => il.Emit(OpCodes.Shr);
                

        /// <summary>
        /// Shift an integer right (shift in zero), return an integer.
        /// </summary>
        public static ILWriter Shr_Un(this ILWriter il)
            => il.Emit(OpCodes.Shr_Un);
                

        /// <summary>
        /// Store value of type native int into memory at address
        /// </summary>
        public static ILWriter Stind_I(this ILWriter il)
            => il.Emit(OpCodes.Stind_I);
                

        /// <summary>
        /// Store value of type int8 into memory at address
        /// </summary>
        public static ILWriter Stind_I1(this ILWriter il)
            => il.Emit(OpCodes.Stind_I1);
                

        /// <summary>
        /// Store value of type int16 into memory at address
        /// </summary>
        public static ILWriter Stind_I2(this ILWriter il)
            => il.Emit(OpCodes.Stind_I2);
                

        /// <summary>
        /// Store value of type int32 into memory at address
        /// </summary>
        public static ILWriter Stind_I4(this ILWriter il)
            => il.Emit(OpCodes.Stind_I4);
                

        /// <summary>
        /// Store value of type int64 into memory at address
        /// </summary>
        public static ILWriter Stind_I8(this ILWriter il)
            => il.Emit(OpCodes.Stind_I8);
                

        /// <summary>
        /// Store value of type float32 into memory at address
        /// </summary>
        public static ILWriter Stind_R4(this ILWriter il)
            => il.Emit(OpCodes.Stind_R4);
                

        /// <summary>
        /// Store value of type float64 into memory at address
        /// </summary>
        public static ILWriter Stind_R8(this ILWriter il)
            => il.Emit(OpCodes.Stind_R8);
                

        /// <summary>
        /// Store value of type object ref (type O) into memory at address
        /// </summary>
        public static ILWriter Stind_Ref(this ILWriter il)
            => il.Emit(OpCodes.Stind_Ref);
                

        /// <summary>
        /// Pop a value from stack into local variable 0.
        /// </summary>
        public static ILWriter Stloc_0(this ILWriter il)
            => il.Emit(OpCodes.Stloc_0);
                

        /// <summary>
        /// Pop a value from stack into local variable 1.
        /// </summary>
        public static ILWriter Stloc_1(this ILWriter il)
            => il.Emit(OpCodes.Stloc_1);
                

        /// <summary>
        /// Pop a value from stack into local variable 2.
        /// </summary>
        public static ILWriter Stloc_2(this ILWriter il)
            => il.Emit(OpCodes.Stloc_2);
                

        /// <summary>
        /// Pop a value from stack into local variable 3.
        /// </summary>
        public static ILWriter Stloc_3(this ILWriter il)
            => il.Emit(OpCodes.Stloc_3);
                

        /// <summary>
        /// Subtract value2 from value1, returning a new value.
        /// </summary>
        public static ILWriter Sub(this ILWriter il)
            => il.Emit(OpCodes.Sub);
                

        /// <summary>
        /// Subtract native int from a native int. Signed result shall fit in same size
        /// </summary>
        public static ILWriter Sub_Ovf(this ILWriter il)
            => il.Emit(OpCodes.Sub_Ovf);
                

        /// <summary>
        /// Subtract native unsigned int from a native unsigned int. Unsigned result shall fit in same size.
        /// </summary>
        public static ILWriter Sub_Ovf_Un(this ILWriter il)
            => il.Emit(OpCodes.Sub_Ovf_Un);
                

        /// <summary>
        /// Subsequent call terminates current method
        /// </summary>
        public static ILWriter Tail(this ILWriter il)
            => il.Emit(OpCodes.Tail);
                

        /// <summary>
        /// Throw an exception.
        /// </summary>
        public static ILWriter Throw(this ILWriter il)
            => il.Emit(OpCodes.Throw);
                

        /// <summary>
        /// Subsequent pointer reference is volatile.
        /// </summary>
        public static ILWriter Volatile(this ILWriter il)
            => il.Emit(OpCodes.Volatile);
                

        /// <summary>
        /// Bitwise XOR of integer values, returns an integer.
        /// </summary>
        public static ILWriter Xor(this ILWriter il)
            => il.Emit(OpCodes.Xor);
                

        #endregion // 
        

        #region Instruction target
            

        /// <summary>
        /// Branch to target if equal.
        /// </summary>
        public static ILWriter Beq(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Beq, target);
                

        /// <summary>
        /// Branch to target if equal, short form.
        /// </summary>
        public static ILWriter Beq_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Beq_S, target);
                

        /// <summary>
        /// Branch to target if greater than or equal to.
        /// </summary>
        public static ILWriter Bge(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bge, target);
                

        /// <summary>
        /// Branch to target if greater than or equal to, short form.
        /// </summary>
        public static ILWriter Bge_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bge_S, target);
                

        /// <summary>
        /// Branch to target if greater than or equal to (unsigned or unordered).
        /// </summary>
        public static ILWriter Bge_Un(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bge_Un, target);
                

        /// <summary>
        /// Branch to target if greater than or equal to (unsigned or unordered), short form
        /// </summary>
        public static ILWriter Bge_Un_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bge_Un_S, target);
                

        /// <summary>
        /// Branch to target if greater than.
        /// </summary>
        public static ILWriter Bgt(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bgt, target);
                

        /// <summary>
        /// Branch to target if greater than, short form.
        /// </summary>
        public static ILWriter Bgt_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bgt_S, target);
                

        /// <summary>
        /// Branch to target if greater than (unsigned or unordered).
        /// </summary>
        public static ILWriter Bgt_Un(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bgt_Un, target);
                

        /// <summary>
        /// Branch to target if greater than (unsigned or unordered), short form.
        /// </summary>
        public static ILWriter Bgt_Un_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bgt_Un_S, target);
                

        /// <summary>
        /// Branch to target if less than or equal to.
        /// </summary>
        public static ILWriter Ble(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Ble, target);
                

        /// <summary>
        /// Branch to target if less than or equal to, short form.
        /// </summary>
        public static ILWriter Ble_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Ble_S, target);
                

        /// <summary>
        /// Branch to target if less than or equal to (unsigned or unordered).
        /// </summary>
        public static ILWriter Ble_Un(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Ble_Un, target);
                

        /// <summary>
        /// Branch to target if less than or equal to (unsigned or unordered), short form
        /// </summary>
        public static ILWriter Ble_Un_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Ble_Un_S, target);
                

        /// <summary>
        /// Branch to target if less than.
        /// </summary>
        public static ILWriter Blt(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Blt, target);
                

        /// <summary>
        /// Branch to target if less than, short form.
        /// </summary>
        public static ILWriter Blt_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Blt_S, target);
                

        /// <summary>
        /// Branch to target if less than (unsigned or unordered).
        /// </summary>
        public static ILWriter Blt_Un(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Blt_Un, target);
                

        /// <summary>
        /// Branch to target if less than (unsigned or unordered), short form.
        /// </summary>
        public static ILWriter Blt_Un_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Blt_Un_S, target);
                

        /// <summary>
        /// Branch to target if unequal or unordered.
        /// </summary>
        public static ILWriter Bne_Un(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bne_Un, target);
                

        /// <summary>
        /// Branch to target if unequal or unordered, short form.
        /// </summary>
        public static ILWriter Bne_Un_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Bne_Un_S, target);
                

        /// <summary>
        /// Branch to target.
        /// </summary>
        public static ILWriter Br(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Br, target);
                

        /// <summary>
        /// Branch to target, short form.
        /// </summary>
        public static ILWriter Br_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Br_S, target);
                

        /// <summary>
        /// Branch to target if value is zero (false).
        /// </summary>
        public static ILWriter Brfalse(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Brfalse, target);
                

        /// <summary>
        /// Branch to target if value is zero (false), short form.
        /// </summary>
        public static ILWriter Brfalse_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Brfalse_S, target);
                

        /// <summary>
        /// Branch to target if value is non-zero (true).
        /// </summary>
        public static ILWriter Brtrue(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Brtrue, target);
                

        /// <summary>
        /// Branch to target if value is non-zero (true), short form.
        /// </summary>
        public static ILWriter Brtrue_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Brtrue_S, target);
                

        /// <summary>
        /// Exit a protected region of code.
        /// </summary>
        public static ILWriter Leave(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Leave, target);
                

        /// <summary>
        /// Exit a protected region of code, short form.
        /// </summary>
        public static ILWriter Leave_S(this ILWriter il, Instruction target)
            => il.Emit(OpCodes.Leave_S, target);
                

        #endregion // Instruction target
        

        #region TypeReference type
            

        /// <summary>
        /// Convert a boxable value to its boxed form
        /// </summary>
        public static ILWriter Box(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Box, type);
                

        /// <summary>
        /// Cast obj to class.
        /// </summary>
        public static ILWriter Castclass(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Castclass, type);
                

        /// <summary>
        /// Call a virtual method on a type constrained to be type T
        /// </summary>
        public static ILWriter Constrained(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Constrained, type);
                

        /// <summary>
        /// Copy a value type from src to dest.
        /// </summary>
        public static ILWriter Cpobj(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Cpobj, type);
                

        /// <summary>
        /// Initialize the value at address dest.
        /// </summary>
        public static ILWriter Initobj(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Initobj, type);
                

        /// <summary>
        /// Test if obj is an instance of class, returning null or an instance of that class or interface.
        /// </summary>
        public static ILWriter Isinst(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Isinst, type);
                

        /// <summary>
        /// Copy the value stored at address src to the stack.
        /// </summary>
        public static ILWriter Ldobj(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Ldobj, type);
                

        /// <summary>
        /// Convert metadata token to its runtime representation.
        /// </summary>
        public static ILWriter Ldtoken(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Ldtoken, type);
                

        /// <summary>
        /// Push a typed reference to ptr of type class onto the stack.
        /// </summary>
        public static ILWriter Mkrefany(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Mkrefany, type);
                

        /// <summary>
        /// Push the address stored in a typed reference.
        /// </summary>
        public static ILWriter Refanyval(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Refanyval, type);
                

        /// <summary>
        /// Push the size, in bytes, of a type as an unsigned int32.
        /// </summary>
        public static ILWriter Sizeof(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Sizeof, type);
                

        /// <summary>
        /// Store a value of type typeTok at an address.
        /// </summary>
        public static ILWriter Stobj(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Stobj, type);
                

        /// <summary>
        /// Extract a value-type from obj, its boxed representation.
        /// </summary>
        public static ILWriter Unbox(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Unbox, type);
                

        /// <summary>
        /// Extract a value-type from obj, its boxed representation
        /// </summary>
        public static ILWriter Unbox_Any(this ILWriter il, TypeReference type)
            => il.Emit(OpCodes.Unbox_Any, type);
                

        #endregion // TypeReference type
        

        #region MethodReference method
            

        /// <summary>
        /// Call method described by method.
        /// </summary>
        public static ILWriter Call(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Call, method);
                

        /// <summary>
        /// Call a method associated with an object.
        /// </summary>
        public static ILWriter Callvirt(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Callvirt, method);
                

        /// <summary>
        /// Exit current method and jump to the specified method.
        /// </summary>
        public static ILWriter Jmp(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Jmp, method);
                

        /// <summary>
        /// Push a pointer to a method referenced by method, on the stack.
        /// </summary>
        public static ILWriter Ldftn(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Ldftn, method);
                

        /// <summary>
        /// Convert metadata token to its runtime representation.
        /// </summary>
        public static ILWriter Ldtoken(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Ldtoken, method);
                

        /// <summary>
        /// Push address of virtual method on the stack.
        /// </summary>
        public static ILWriter Ldvirtftn(this ILWriter il, MethodReference method)
            => il.Emit(OpCodes.Ldvirtftn, method);
                

        #endregion // MethodReference method
        

        #region CallSite call
            

        /// <summary>
        /// Call method indicated on the stack with arguments described by callsitedescr.
        /// </summary>
        public static ILWriter Calli(this ILWriter il, CallSite call)
            => il.Emit(OpCodes.Calli, call);
                

        #endregion // CallSite call
        

        #region byte nbr
            

        /// <summary>
        /// Load argument numbered num onto the stack, short form.
        /// </summary>
        public static ILWriter Ldarg_S(this ILWriter il, byte num)
            => il.Emit(OpCodes.Ldarg_S, num);
                

        /// <summary>
        /// Fetch the address of argument argNum, short form.
        /// </summary>
        public static ILWriter Ldarga_S(this ILWriter il, byte argNum)
            => il.Emit(OpCodes.Ldarga_S, argNum);
                

        /// <summary>
        /// Load local variable of index indx onto stack, short form.
        /// </summary>
        public static ILWriter Ldloc_S(this ILWriter il, byte indx)
            => il.Emit(OpCodes.Ldloc_S, indx);
                

        /// <summary>
        /// Load address of local variable with index indx, short form.
        /// </summary>
        public static ILWriter Ldloca_S(this ILWriter il, byte indx)
            => il.Emit(OpCodes.Ldloca_S, indx);
                

        /// <summary>
        /// Store value to the argument numbered num, short form.
        /// </summary>
        public static ILWriter Starg_S(this ILWriter il, byte num)
            => il.Emit(OpCodes.Starg_S, num);
                

        /// <summary>
        /// Pop a value from stack into local variable indx, short form.
        /// </summary>
        public static ILWriter Stloc_S(this ILWriter il, byte indx)
            => il.Emit(OpCodes.Stloc_S, indx);
                

        #endregion // byte nbr
        

        #region int nbr
            

        /// <summary>
        /// Push num of type int32 onto the stack as int32.
        /// </summary>
        public static ILWriter Ldc_I4(this ILWriter il, int num)
            => il.Emit(OpCodes.Ldc_I4, num);
                

        #endregion // int nbr
        

        #region sbyte nbr
            

        /// <summary>
        /// Push num onto the stack as int32, short form.
        /// </summary>
        public static ILWriter Ldc_I4_S(this ILWriter il, sbyte num)
            => il.Emit(OpCodes.Ldc_I4_S, num);
                

        #endregion // sbyte nbr
        

        #region long nbr
            

        /// <summary>
        /// Push num of type int64 onto the stack as int64.
        /// </summary>
        public static ILWriter Ldc_I8(this ILWriter il, long num)
            => il.Emit(OpCodes.Ldc_I8, num);
                

        #endregion // long nbr
        

        #region float nbr
            

        /// <summary>
        /// Push num of type float32 onto the stack as F.
        /// </summary>
        public static ILWriter Ldc_R4(this ILWriter il, float num)
            => il.Emit(OpCodes.Ldc_R4, num);
                

        #endregion // float nbr
        

        #region double nbr
            

        /// <summary>
        /// Push num of type float64 onto the stack as F.
        /// </summary>
        public static ILWriter Ldc_R8(this ILWriter il, double num)
            => il.Emit(OpCodes.Ldc_R8, num);
                

        #endregion // double nbr
        

        #region FieldReference field
            

        /// <summary>
        /// Push the value of field of object (or value type) obj, onto the stack.
        /// </summary>
        public static ILWriter Ldfld(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Ldfld, field);
                

        /// <summary>
        /// Push the address of field of object obj on the stack.
        /// </summary>
        public static ILWriter Ldflda(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Ldflda, field);
                

        /// <summary>
        /// Push the value of field on the stack.
        /// </summary>
        public static ILWriter Ldsfld(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Ldsfld, field);
                

        /// <summary>
        /// Push the address of the static field, field, on the stack.
        /// </summary>
        public static ILWriter Ldsflda(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Ldsflda, field);
                

        /// <summary>
        /// Convert metadata token to its runtime representation.
        /// </summary>
        public static ILWriter Ldtoken(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Ldtoken, field);
                

        /// <summary>
        /// Replace the value of field of the object obj with value.
        /// </summary>
        public static ILWriter Stfld(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Stfld, field);
                

        /// <summary>
        /// Replace the value of field with val.
        /// </summary>
        public static ILWriter Stsfld(this ILWriter il, FieldReference field)
            => il.Emit(OpCodes.Stsfld, field);
                

        #endregion // FieldReference field
        

        #region string str
            

        /// <summary>
        /// Push a string object for the literal string.
        /// </summary>
        public static ILWriter Ldstr(this ILWriter il, string str)
            => il.Emit(OpCodes.Ldstr, str);
                

        #endregion // string str
        

        #region TypeReference itemType
            

        /// <summary>
        /// Create a new array with elements of type etype.
        /// </summary>
        public static ILWriter Newarr(this ILWriter il, TypeReference itemType)
            => il.Emit(OpCodes.Newarr, itemType);
                

        #endregion // TypeReference itemType
        

        #region MethodReference ctor
            

        /// <summary>
        /// Allocate an uninitialized object or value type and call ctor.
        /// </summary>
        public static ILWriter Newobj(this ILWriter il, MethodReference ctor)
            => il.Emit(OpCodes.Newobj, ctor);
                

        #endregion // MethodReference ctor
        

        #region params Instruction[] instructions
            

        /// <summary>
        /// Jump to one of n values.
        /// </summary>
        public static ILWriter Switch(this ILWriter il, params Instruction[] instructions)
            => il.Emit(OpCodes.Switch, instructions);
                

        #endregion // params Instruction[] instructions
        

    }
}