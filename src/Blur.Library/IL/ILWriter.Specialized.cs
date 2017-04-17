using System;
using System.Collections.Generic;
using System.Reflection;
using Blur.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class ILWriter
    {
        #region Get / Set / Load / Save

        #region Arguments
        /// <summary>
        /// Load the argument at the specified <paramref name="position"/>.
        /// </summary>
        public ILWriter LoadArg(int position)
        {
            switch (position)
            {
                case 0:
                    return this.Emit(OpCodes.Ldarg_0);
                case 1:
                    return this.Emit(OpCodes.Ldarg_1);
                case 2:
                    return this.Emit(OpCodes.Ldarg_2);
                case 3:
                    return this.Emit(OpCodes.Ldarg_3);
                default:
                    return position <= byte.MaxValue
                        ? this.Emit(OpCodes.Ldarg_S, (byte)position)
                        : this.Emit(OpCodes.Ldarg, position);
            }
        }

        /// <summary>
        /// Load the argument referenced by the given <paramref name="parameter"/>.
        /// </summary>
        public ILWriter LoadArg(ParameterDefinition parameter) => this.Emit(OpCodes.Ldarg, parameter);

        /// <summary>
        /// Load the address of the argument at the specified <paramref name="position"/>.
        /// </summary>
        public ILWriter LoadArgAddress(int position)
            => position < byte.MaxValue
                ? this.Emit(OpCodes.Ldarga_S, (byte)position)
                : this.Emit(OpCodes.Ldarga, position);

        /// <summary>
        /// Load the address of the argument referenced by the given <paramref name="parameter"/>.
        /// </summary>
        public ILWriter LoadArgAddress(ParameterDefinition parameter) => this.Emit(OpCodes.Ldarga, parameter);

        /// <summary>
        /// Load <see langword="this"/>.
        /// </summary>
        public ILWriter This() => this.Emit(OpCodes.Ldarg_0);
        #endregion

        #region Fields
        /// <summary>
        /// Load a <paramref name="field"/>'s value to the stack.
        /// <para>
        /// If the field is <see langword="static"/>, <see cref="OpCodes.Ldsfld"/> will be used.
        /// Otherwise, <see cref="OpCodes.Ldfld"/> will be used, and <see langword="this"/> will be loaded first-hand.
        /// </para>
        /// </summary>
        public ILWriter Get(FieldDefinition field)
            => field.IsStatic ? this.Emit(OpCodes.Ldsfld, field) : this.This().Emit(OpCodes.Ldfld, field);

        /// <summary>
        /// Save the value at the top of the stack
        /// to a <paramref name="field"/>.
        /// <para>
        /// If the property is <see langword="static"/>, <see cref="OpCodes.Call"/> will be used.
        /// Otherwise, <see cref="OpCodes.Callvirt"/> will be used, and <see langword="this"/> will be loaded first-hand.
        /// </para>
        /// </summary>
        public ILWriter Set(FieldDefinition field)
        {
            if (field.IsStatic)
                return this.Emit(OpCodes.Stsfld, field);

            try
            {
                return Reposition(CountedRepositionType.LessValues, 1)
                    .This()
                    .Reposition()
                    .Emit(OpCodes.Stfld, field);
            }
            catch
            {
                throw new InvalidOperationException("Invalid body.");
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Load a <paramref name="property"/>'s value to the stack.
        /// <para>
        /// If the property is <see langword="static"/>, <see cref="OpCodes.Call"/> will be used.
        /// Otherwise, <see cref="OpCodes.Callvirt"/> will be used, and <see langword="this"/> will be loaded first-hand.
        /// </para>
        /// </summary>
        public ILWriter Get(PropertyDefinition property)
            => property.IsStatic() ? this.Emit(OpCodes.Call, property.GetMethod) : this.This().Emit(OpCodes.Callvirt, property.GetMethod);

        /// <summary>
        /// Save the value at the top of the stack
        /// to a <paramref name="property"/>.
        /// <para>
        /// If the property is <see langword="static"/>, <see cref="OpCodes.Call"/> will be used.
        /// Otherwise, <see cref="OpCodes.Callvirt"/> will be used, and <see langword="this"/> will be loaded first-hand.
        /// </para>
        /// </summary>
        public ILWriter Set(PropertyDefinition property)
        {
            MethodDefinition setMethod = property.SetMethod;

            if (setMethod.IsStatic)
                return this.Emit(OpCodes.Call, setMethod);

            try
            {
                return this.Reposition(CountedRepositionType.LessValues, 1)
                    .This()
                    .Reposition()
                    .Emit(OpCodes.Callvirt, setMethod);
            }
            catch
            {
                throw new InvalidOperationException("Invalid body.");
            }
        }
        #endregion

        #region Variables
        /// <summary>
        /// Adds the value of the given variable to the stack.
        /// </summary>
        public ILWriter Load(VariableDefinition variable) => this.Emit(OpCodes.Ldloc, variable);

        /// <summary>
        /// Adds the value of the variable at the given index to the stack.
        /// </summary>
        public ILWriter Load(int varIndex)
        {
            switch (varIndex)
            {
                case 0:
                    return this.Ldloc_0();
                case 1:
                    return this.Ldloc_1();
                case 2:
                    return this.Ldloc_2();
                case 3:
                    return this.Ldloc_3();
                default:
                    return this.Emit(varIndex > byte.MaxValue ? OpCodes.Ldloc : OpCodes.Ldloc_S, varIndex);
            }
        }

        /// <summary>
        /// Saves the value at the top of the stack to the given variable.
        /// </summary>
        public ILWriter SaveTo(VariableDefinition variable) => this.Emit(OpCodes.Stloc, variable);

        /// <summary>
        /// Saves the value at the top of the stack to the variable at the given index.
        /// </summary>
        public ILWriter SaveTo(int varIndex)
        {
            switch (varIndex)
            {
                case 0:
                    return this.Stloc_0();
                case 1:
                    return this.Stloc_1();
                case 2:
                    return this.Stloc_2();
                case 3:
                    return this.Stloc_3();
                default:
                    return this.Emit(varIndex > byte.MaxValue ? OpCodes.Stloc : OpCodes.Stloc_S, varIndex);
            }
        }
        #endregion

        #region Indexes
        /// <summary>
        /// Loads the item at the given index to the stack.
        /// </summary>
        public ILWriter LoadIndex(bool isValueType = false) => this.Emit(isValueType ? OpCodes.Ldelem_Any : OpCodes.Ldelem_Ref);

        /// <summary>
        /// Saves the item at the top of the stack to the given index.
        /// </summary>
        public ILWriter SaveToIndex(bool isValueType = false) => this.Emit(isValueType ? OpCodes.Stelem_Any : OpCodes.Stelem_Ref);
        #endregion

        /// <summary>
        /// Load a <paramref name="method"/> reference to the stack.
        /// </summary>
        public ILWriter Load(MethodReference method) => this.Emit(OpCodes.Ldftn, method);
        #endregion

        #region Literal
#if !NET_CORE
        private static readonly string[] ConstantTypes = {"Boolean","Sbyte","Int16","Int32","Int64","Single","Double","Char","Byte","UInt16","UInt32","UInt64","Decimal","String"};
#endif

        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// is constant, in which case it can be directly
        /// added on the stack using the <see cref="Constant(object)"/>
        /// method.
        /// </summary>
        public static bool IsConstant(TypeDefinition type)
        {
#if NET_CORE
            return System.Array.IndexOf(Enum.GetNames(typeof(TypeCode)), type.Name) != -1;
#else
            return System.Array.IndexOf(ConstantTypes, type.Name) != -1;
#endif
        }

        /// <inheritdoc cref="IsConstant(TypeDefinition)"/>
        public static bool IsConstant(Type type)
        {
#if NS15
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Char:
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Decimal:
                case TypeCode.String:
                    return true;
            }
            return false;
#elif NET_CORE
            return System.Array.IndexOf(Enum.GetNames(typeof(TypeCode)), type.Name) != -1;
#else
            return System.Array.IndexOf(ConstantTypes, type.Name) != -1;
#endif
        }

        /// <summary>
        /// Returns whether or not the type <typeparamref name="T"/>
        /// is constant, in which case it can be directly
        /// added on the stack using the <see cref="Constant(object)"/>
        /// method.
        /// </summary>
        public static bool IsConstant<T>() => IsConstant(typeof(T));


        /// <summary>
        /// Push <see langword="null"/> on the stack.
        /// </summary>
        public ILWriter Null() => this.Emit(OpCodes.Ldnull);

        /// <summary>
        /// Push a <see cref="string"/> on the stack.
        /// </summary>
        public ILWriter String(string str) => this.Emit(OpCodes.Ldstr, str);

        /// <summary>
        /// Push a <see cref="string"/> on the stack.
        /// </summary>
        public ILWriter Char(char character) => this.UShort(character);

        /// <summary>
        /// Push a <see cref="bool"/> on the stack.
        /// </summary>
        public ILWriter Boolean(bool value) => this.Emit(value ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

        /// <summary>
        /// Push <see langword="true"/> on the stack.
        /// </summary>
        public ILWriter True() => this.Boolean(true);

        /// <summary>
        /// Push <see langword="false"/> on the stack.
        /// </summary>
        public ILWriter False() => this.Boolean(false);

        /// <summary>
        /// Push a constant <see langword="value"/> on the stack.
        /// </summary>
        public ILWriter Constant(object arg)
        {
            return this.Emit(InstructionsForConstant(arg));
        }

        /// <summary>
        /// Push all the given constants on the stack, in order.
        /// </summary>
        /// <param name="args">
        /// An array of constant values (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </param>
        public ILWriter All(params object[] args)
        {
            for (int i = 0; i < args.Length; i++)
                this.Constant(args[i]);

            return this;
        }

        /// <summary>
        /// Push all the given constants on the stack, in order.
        /// </summary>
        /// <param name="args">
        /// An array of constant values (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </param>
        public ILWriter All(IEnumerable<object> args)
        {
            foreach (object arg in args)
                this.Constant(arg);

            return this;
        }

#region Integers
        /// <summary>
        /// Push an <see cref="int"/> on the stack
        /// </summary>
        public ILWriter Int(int value)
        {
            OpCode c;

            switch (value)
            {
                case -1:
                    c = OpCodes.Ldc_I4_M1;
                    break;
                case 0:
                    c = OpCodes.Ldc_I4_0;
                    break;
                case 1:
                    c = OpCodes.Ldc_I4_1;
                    break;
                case 2:
                    c = OpCodes.Ldc_I4_2;
                    break;
                case 3:
                    c = OpCodes.Ldc_I4_3;
                    break;
                case 4:
                    c = OpCodes.Ldc_I4_4;
                    break;
                case 5:
                    c = OpCodes.Ldc_I4_5;
                    break;
                case 6:
                    c = OpCodes.Ldc_I4_6;
                    break;
                case 7:
                    c = OpCodes.Ldc_I4_7;
                    break;
                case 8:
                    c = OpCodes.Ldc_I4_8;
                    break;
                default:
                    if (value >= -128 && value <= 127)
                        return Emit(OpCodes.Ldc_I4_S, (sbyte) value);
                    else
                        return Emit(OpCodes.Ldc_I4, value);
            }
            return this.Emit(c);
        }

        /// <summary>
        /// Push a <see cref="uint"/> on the stack.
        /// </summary>
        public ILWriter UInt(uint value)
        {
            this.Int((int)value);
            return this.Emit(OpCodes.Conv_U4);
        }

        /// <summary>
        /// Push a <see cref="byte"/> on the stack.
        /// </summary>
        public ILWriter Byte(byte value)
        {
            this.Int(value);
            return this.Emit(OpCodes.Conv_U1);
        }

        /// <summary>
        /// Push a <see cref="sbyte"/> on the stack.
        /// </summary>
        public ILWriter SByte(sbyte value)
        {
            this.Int(value);
            return this.Emit(OpCodes.Conv_I1);
        }

        /// <summary>
        /// Push a <see cref="short"/> on the stack.
        /// </summary>
        public ILWriter Short(short value)
        {
            this.Int(value);
            return this.Emit(OpCodes.Conv_I2);
        }

        /// <summary>
        /// Push a <see cref="ushort"/> on the stack.
        /// </summary>
        public ILWriter UShort(ushort value)
        {
            this.Int(value);
            return this.Emit(OpCodes.Conv_U2);
        }

        /// <summary>
        /// Push a <see cref="long"/> on the stack.
        /// </summary>
        public ILWriter Long(long value)
        {
            return this.Emit(OpCodes.Ldc_I8, value);
        }

        /// <summary>
        /// Push a <see cref="ulong"/> on the stack.
        /// </summary>
        public ILWriter ULong(ulong value)
        {
            this.Emit(OpCodes.Ldc_I8, (long)value);
            return this.Emit(OpCodes.Conv_U8);
        }
#endregion

#region Reals
        /// <summary>
        /// Push a <see cref="float"/> on the stack.
        /// </summary>
        public ILWriter Single(float value) => this.Emit(OpCodes.Ldc_R4, value);

        /// <summary>
        /// Push a <see cref="double"/> on the stack.
        /// </summary>
        public ILWriter Double(double value) => this.Emit(OpCodes.Ldc_R8, value);
#endregion

#endregion

#region New
        /// <summary>
        /// Push a new <see cref="object"/> of a specified
        /// <paramref name="type"/> on the stack.
        /// <para>
        /// Only invoke this method if all the needed arguments are
        /// in the given <paramref name="args"/>. If other parameters are on
        /// the stack, please use <see cref="Call(MethodReference, object[])"/> instead.
        /// </para>
        /// </summary>
        public ILWriter New(TypeReference type, params object[] args)
        {
            if (type.IsValueType)
                return this.All(args).Emit(OpCodes.Newobj, type);

            // TODO: Check previous arguments to make sure we have all arguments loaded.
            MethodDefinition ctor = null;
            var methods = type.Resolve().Methods;

            for (int i = 0; i < methods.Count; i++)
            {
                MethodDefinition method = methods[i];

                if (method.Parameters.Count != args.Length)
                    continue;

                for (int o = 0; o < args.Length; o++)
                {
                    if (args[o] == null && method.Parameters[o].ParameterType.IsValueType)
                        goto NoMatch;
                    if (args[o] != null && method.Parameters[o].ParameterType.FullName == o.GetType().FullName)
                        goto NoMatch;
                }

                ctor = method;
                break;

                NoMatch: ;
            }

            if (ctor == null)
                throw new ArgumentException("Could not find a constructor matching the given arguments.");

            return this.All(args).Emit(OpCodes.Newobj, Processor.TargetModuleDefinition.ImportReference(ctor));
        }

        /// <summary>
        /// Push a new <see cref="object"/> of
        /// type <typeparamref name="T"/> on the stack.
        /// <para>
        /// Only invoke this method if all the needed arguments are
        /// in the given <paramref name="args"/>. If other parameters are on
        /// the stack, please use <see cref="Call(MethodReference, object[])"/> instead.
        /// </para>
        /// </summary>
        public ILWriter New<T>(params object[] args) => this.New(typeof(T), args);

        /// <summary>
        /// Push a new <see cref="object"/> of a specified
        /// <paramref name="type"/> on the stack.
        /// <para>
        /// Only invoke this method if all the needed arguments are
        /// in the given <paramref name="args"/>. If other parameters are on
        /// the stack, please use <see cref="Call(MethodReference, object[])"/> instead.
        /// </para>
        /// </summary>
        public ILWriter New(Type type, params object[] args) => this.New(type.GetReference(), args);

#region Array
        /// <summary>
        /// Create a new <see langword="array"/> on the stack.
        /// <para>
        /// The length of the array must have been previously added on the stack.
        /// </para>
        /// </summary>
        public ILWriter Array(TypeReference itemType) => this.Emit(OpCodes.Newarr, Processor.TargetModuleDefinition.ImportReference(itemType));

        /// <summary>
        /// Create a new <see langword="array"/> on the stack.
        /// </summary>
        public ILWriter Array(TypeReference itemType, int count) => this.Int(count).Emit(OpCodes.Newarr, Processor.TargetModuleDefinition.ImportReference(itemType));

        /// <inheritdoc cref="Array(TypeReference)"/>
        public ILWriter Array(Type itemType) => this.Emit(OpCodes.Newarr, itemType);

        /// <inheritdoc cref="Array(TypeReference, int)"/>
        public ILWriter Array(Type itemType, int count) => this.Int(count).Emit(OpCodes.Newarr, itemType);

        /// <inheritdoc cref="Array(TypeReference)"/>
        public ILWriter Array<T>(Type itemType) => this.Array(typeof(T));

        /// <inheritdoc cref="Array(TypeReference, int)"/>
        public ILWriter Array<T>(Type itemType, int count) => this.Array(typeof(T), count);

        /// <summary>
        /// Create an <see langword="array"/> of <typeparamref name="T"/>
        /// on the stack, and populate it with the given <paramref name="items"/>.
        /// <para>
        /// The given items must be constant (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </para>
        /// </summary>
        public ILWriter Array<T>(T[] items)
        {
            this.Int(items.Length)
                .Emit(OpCodes.Newarr, typeof(T));

            for (int i = 0; i < items.Length; i++)
            {
                this.Emit(OpCodes.Dup)
                    .Int(i)
                    .Constant(items[i])
                    .Emit(OpCodes.Stobj, typeof(T));
            }

            return this;
        }
#endregion

#endregion

#region Call
        /// <summary>
        /// Call a method with the given arguments.
        /// <para>
        /// The given arguments must be constant (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </para>
        /// </summary>
        public ILWriter Call(MethodReference method, params object[] args)
            => this.All(args)
                .Emit(method.Resolve().IsStatic || method.DeclaringType.IsValueType
                    ? OpCodes.Call : OpCodes.Callvirt, method);

        /// <summary>
        /// Call a method with the given arguments.
        /// <para>
        /// The given arguments must be constant (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </para>
        /// </summary>
        public ILWriter Call(MethodInfo method, params object[] args) => this.Call(method.GetReference(), args);
#endregion

#region Default
        /// <summary>
        /// Push a <paramref name="type"/>'s <see langword="default"/> value on the stack.
        /// </summary>
        public ILWriter Default(Type type)
        {
#if !NET_CORE
            if (type.Namespace == "System")
            {
                switch (type.Name)
                {
                    case "Boolean":
                    case "Char":
                    case "SByte":
                    case "Byte":
                    case "Int16":
                    case "UInt16":
                    case "Int32":
                    case "UInt32":
                        return this.Int(default(int));
                    case "Int64":
                    case "UInt64":
                        return this.Long(default(long));
                    case "Single":
                        return this.Single(default(float));
                    case "Double":
                        return this.Double(default(double));
                    case "Decimal":
                        return this.Emit(OpCodes.Ldc_I4_0).New<decimal>();
                }
            }
#else
#if !NS15
            TypeCode code;
            if (type.Namespace == "System" && Enum.TryParse(type.Name, out code))
            {
                switch (code)
#else
                switch (Type.GetTypeCode(type))
#endif
                {
                    case TypeCode.Boolean:
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        return this.Int(default(int));
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        return this.Long(default(long));
                    case TypeCode.Single:
                        return this.Single(default(float));
                    case TypeCode.Double:
                        return this.Double(default(double));
                    case TypeCode.Decimal:
                        return this.Emit(OpCodes.Ldc_I4_0).New<decimal>();
                }
#if !NS15
            }
#endif
#endif

            if (!type.GetTypeInfo().IsValueType)
                return this.Null();

            VariableDefinition lv = this.Define(type);
            return this.Emit(OpCodes.Ldloca, lv)
                .Emit(OpCodes.Initobj, type)
                .Emit(OpCodes.Ldloc, lv);
        }

        /// <summary>
        /// Push <see langword="default"/>(<typeparamref name="T"/>) on the stack.
        /// </summary>
        public ILWriter Default<T>() => this.Default(typeof(T));
#endregion

#region Throw
        /// <summary>
        /// Throw the exception at the top of the stack.
        /// </summary>
        public ILWriter Throw() => this.Emit(OpCodes.Throw);

        /// <summary>
        /// Create an exception of the specified <paramref name="type"/>,
        /// and throw it.
        /// </summary>
        public ILWriter Throw(TypeDefinition type) => this.New(type).Throw();

        /// <summary>
        /// Create an exception of type <typeparamref name="T"/>,
        /// and throw it.
        /// </summary>
        public ILWriter Throw<T>() where T : Exception => this.New<T>().Throw();

        /// <summary>
        /// Create an exception of the specified <paramref name="type"/>,
        /// and throw it.
        /// </summary>
        public ILWriter Throw(Type type) => this.New(type).Throw();

        /// <summary>
        /// Push a new <see cref="object"/> of a specified
        /// <paramref name="type"/> on the stack.
        /// </summary>
        public ILWriter Throw(TypeDefinition type, params object[] args) => this.New(type, args).Throw();

        /// <summary>
        /// Create an exception of type <typeparamref name="T"/>,
        /// and throw it.
        /// </summary>
        public ILWriter Throw<T>(params object[] args) where T : Exception => this.New<T>(args).Throw();

        /// <summary>
        /// Create an exception of the specified <paramref name="type"/>,
        /// and throw it.
        /// </summary>
        public ILWriter Throw(Type type, params object[] args) => this.New(type, args).Throw();
#endregion

#region Other
        /// <summary>
        /// Do nothing.
        /// </summary>
        public ILWriter Nop() => this.Emit(OpCodes.Nop);

        /// <summary>
        /// Pop the value at the top of the stack.
        /// </summary>
        public ILWriter Pop() => this.Emit(OpCodes.Pop);

        /// <summary>
        /// Duplicate the value at the top of the stack.
        /// </summary>
        public ILWriter Dup() => this.Emit(OpCodes.Dup);

        /// <summary>
        /// Return from this function, optionally with the value at
        /// the top of the stack.
        /// </summary>
        public ILWriter Return() => this.Emit(OpCodes.Ret);

        /// <summary>
        /// Return <paramref name="obj"/>.
        /// <para>
        /// The given object must be constant (call <see cref="IsConstant{T}"/> if
        /// you have doubts).
        /// </para>
        /// </summary>
        public ILWriter Return(object obj) => this.Constant(obj).Emit(OpCodes.Ret);
#endregion

#region Comparison
        /// <summary>
        /// Pushes <see langword="true"/> if the two values at the top
        /// of the stack are equal. Else, pushes <see langword="false"/>.
        /// </summary>
        public ILWriter Equal() => this.Ceq();

        /// <summary>
        /// Pushes <see langword="false"/> if the two values at the top
        /// of the stack are equal. Else, pushes <see langword="true"/>.
        /// </summary>
        public ILWriter NotEqual() => this.Ceq().Not();

        /// <inheritdoc cref="NotEqual"/>.
        public ILWriter Different() => this.Ceq().Not();
#endregion
    }
}
