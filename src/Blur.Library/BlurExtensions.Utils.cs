using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    partial class BlurExtensions
    {
        /// <summary>
        /// Returns whether or not the given <paramref name="method"/>
        /// is <see langword="extern"/>.
        /// </summary>
        /// <remarks>
        /// This method checks if <see cref="MethodDefinition.RVA"/> is 0,
        /// and if it wasn't previously marked by Blur has an external method.
        /// </remarks>
        public static bool IsExtern(this MethodDefinition method)
        {
            if (method.RVA == 0)
                return true;

            if (!method.HasBody || method.Body.Instructions.Count != 2)
                return false;

            Instruction firstIns = method.Body.Instructions[0];
            MethodReference operand = firstIns.Operand as MethodReference;

            return operand != null
                && firstIns.OpCode.Code == Code.Newobj
                && operand.DeclaringType.Name == nameof(ExternalMethodException)
                && operand.Name == ".ctor";
        }

        #region IsMatch

        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// implements the interface <typeparamref name="T"/>.
        /// </summary>
        public static bool Implements<T>(this TypeDefinition type) => type.Implements(typeof(T));

        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// implements the given interface.
        /// </summary>
        public static bool Implements(this TypeDefinition type, Type interfaceType)
        {
            while (type != null)
            {
                var interfaces = type.Interfaces;

                for (int i = 0; i < interfaces.Count; i++)
                    if (interfaces[i].InterfaceType.Is(interfaceType, false))
                        return true;

                type = type.BaseType?.Resolve();
            }

            return false;
        }

        /// <summary>
        /// Returns whether or not the given <paramref name="type"/>
        /// inherits the type <typeparamref name="T"/>.
        /// </summary>
        public static bool Inherits<T>(this TypeReference type)
            => type.Inherits(typeof(T));

        /// <summary>
        /// Returns whether or not <paramref name="self"/> inherits <paramref name="type"/>.
        /// </summary>
        public static bool Inherits(this TypeReference self, Type type)
            => self.FullName != type.GetReferenceName() && (self.Resolve()?.Is(type, true) ?? false);

        /// <summary>
        /// Returns whether or not <paramref name="self"/> inherits <paramref name="typeRef"/>.
        /// </summary>
        public static bool Inherits(this TypeReference self, TypeReference typeRef)
            => self != typeRef && (self.Resolve()?.Is(typeRef, true) ?? false);

        /// <summary>
        /// Returns whether or not <paramref name="self"/> inherits <paramref name="typeDef"/>.
        /// </summary>
        public static bool Inherits(this TypeDefinition self, TypeDefinition typeDef)
            => self != typeDef && self.Is(typeDef, true);
        #endregion

        #region CreateInstance
        /// <summary>
        /// Creates an attribute of type <typeparamref name="T"/>, given
        /// its <see cref="CustomAttribute"/> data.
        /// </summary>
        public static T CreateInstance<T>(this CustomAttribute attribute)
        {
            TypeInfo type = attribute.AttributeType.AsTypeInfo();

            T weaver = (T)Activator.CreateInstance(type.AsType(), attribute.ConstructorArguments.Convert(GetValue));

            foreach (var field in attribute.Fields)
                type.GetDeclaredField(field.Name)
                    .SetValue(weaver, field.Argument.GetValue());

            foreach (var property in attribute.Properties)
                type.GetDeclaredProperty(property.Name)
                    .SetValue(weaver, property.Argument.GetValue());

            return weaver;
        }

        /// <summary>
        /// Creates an object of type <paramref name="typeRef"/>, given
        /// its constructor <paramref name="arguments"/>.
        /// </summary>
        public static object CreateInstance(this TypeReference typeRef, params object[] arguments)
        {
            return Activator.CreateInstance(typeRef.AsType(), arguments);
        }
        #endregion

        #region Convert
        /// <summary>
        /// Converts a list of type <typeparamref name="T"/> to
        /// an array of type <typeparamref name="U"/>.
        /// </summary>
        internal static U[] Convert<T, U>(this IReadOnlyList<T> self, Func<T, U> converter)
        {
            U[] array = new U[self.Count];

            for (int i = 0; i < array.Length; i++)
                array[i] = converter(self[i]);

            return array;
        }

        /// <summary>
        /// Converts a list of type <typeparamref name="T"/> to
        /// an array of type <typeparamref name="U"/>.
        /// </summary>
        internal static U[] Convert<T, U>(this Mono.Collections.Generic.Collection<T> self, Func<T, U> converter)
        {
            U[] array = new U[self.Count];

            for (int i = 0; i < array.Length; i++)
                array[i] = converter(self[i]);

            return array;
        }

        /// <summary>
        /// Convert <see cref="TypeReference.FullName"/> to <see cref="Type.FullName"/>
        /// and return the converted name.
        /// </summary>
        public static string GetReflectionName(this TypeReference type)
        {
            if (!type.IsGenericInstance)
                return type.FullName;

            var genericInstance = (GenericInstanceType)type;
            return
                $"{genericInstance.Namespace}.{type.Name}[{string.Join(",", genericInstance.GenericArguments.Select(p => p.GetReflectionName()).ToArray())}]";
        }

        /// <summary>
        /// Convert <see cref="Type.FullName"/> to <see cref="TypeReference.FullName"/>
        /// and return the converted name.
        /// </summary>
        public static string GetReferenceName(this Type type)
        {
            return type.GenericTypeArguments.Length == 0
                ? type.FullName
                : $"{type.Namespace}.{type.Name}<{string.Join(",", type.GenericTypeArguments.Select(x => x.GetReferenceName()).ToArray())}>";
        }

        /// <summary>
        /// Make a <see cref="GenericInstanceMethod"/>, given its <see cref="MethodReference"/>
        /// and its generic arguments.
        /// </summary>
        public static GenericInstanceMethod MakeGenericMethod(this MethodReference method, params TypeReference[] genericArguments)
        {
            var result = new GenericInstanceMethod(method);
            foreach (var argument in genericArguments)
                result.GenericArguments.Add(argument);
            return result;
        }

        /// <summary>
        /// Make a <see cref="GenericInstanceType"/>, given its <see cref="TypeReference"/>
        /// and its generic arguments.
        /// </summary>
        public static GenericInstanceType MakeGenericType(this TypeReference type, params TypeReference[] genericArguments)
        {
            var result = new GenericInstanceType(type);
            foreach (var argument in genericArguments)
                result.GenericArguments.Add(argument);
            return result;
        }
        #endregion

        #region Clone
        /// <summary>
        /// Returns a new <see cref="Instruction"/> matching the
        /// given <paramref name="instruction"/>.
        /// </summary>
        public static Instruction Clone(this Instruction instruction)
        {
            OpCode opcode = instruction.OpCode;
            object operand = instruction.Operand;

            if (operand == null)
                return Instruction.Create(opcode);
            if (operand is string)
                return Instruction.Create(opcode, (string)operand);

            if (operand is byte)
                return Instruction.Create(opcode, (byte)operand);
            if (operand is sbyte)
                return Instruction.Create(opcode, (sbyte)operand);
            if (operand is float)
                return Instruction.Create(opcode, (float)operand);
            if (operand is double)
                return Instruction.Create(opcode, (double)operand);
            if (operand is int)
                return Instruction.Create(opcode, (int)operand);
            if (operand is long)
                return Instruction.Create(opcode, (long)operand);

            if (operand is Instruction)
                return Instruction.Create(opcode, (Instruction)operand);
            if (operand is Instruction[])
                return Instruction.Create(opcode, (Instruction[])operand);
            if (operand is CallSite)
                return Instruction.Create(opcode, (CallSite)operand);

            if (operand is MethodReference)
                return Instruction.Create(opcode, Processor.TargetModuleDefinition.ImportReference((MethodReference)operand));
            if (operand is FieldReference)
                return Instruction.Create(opcode, Processor.TargetModuleDefinition.ImportReference((FieldReference)operand));
            if (operand is TypeReference)
                return Instruction.Create(opcode, Processor.TargetModuleDefinition.ImportReference((TypeReference)operand));
            if (operand is VariableDefinition)
                return Instruction.Create(opcode, ((VariableDefinition)operand).Clone());
            if (operand is ParameterDefinition)
                return Instruction.Create(opcode, ((ParameterDefinition)operand).Clone());

            throw new ArgumentException("Invalid instruction given.", nameof(instruction));
        }

        /// <summary>
        /// Returns a new <see cref="VariableDefinition"/> matching the
        /// given <paramref name="variable"/>.
        /// </summary>
        public static VariableDefinition Clone(this VariableDefinition variable)
        {
            return new VariableDefinition(variable.VariableType);
        }

        /// <summary>
        /// Returns a new <see cref="ParameterDefinition"/> matching the
        /// given <paramref name="parameter"/>.
        /// </summary>
        public static ParameterDefinition Clone(this ParameterDefinition parameter, bool withCustomAttributes = true)
        {
            ParameterDefinition newParameter = new ParameterDefinition(parameter.Name, parameter.Attributes, parameter.ParameterType)
            {
                HasConstant = parameter.HasConstant,
                HasDefault = parameter.HasDefault,
                HasFieldMarshal = parameter.HasFieldMarshal,

                IsIn = parameter.IsIn,
                IsOut = parameter.IsOut,
                IsReturnValue = parameter.IsReturnValue,
                IsLcid = parameter.IsLcid,
                IsOptional = parameter.IsOptional,

                MarshalInfo = parameter.MarshalInfo,
                Constant = parameter.Constant
            };

            if (withCustomAttributes && parameter.HasCustomAttributes)
            {
                for (int i = 0; i < parameter.CustomAttributes.Count; i++)
                {
                    CustomAttribute oldAttr = parameter.CustomAttributes[i];
                    newParameter.CustomAttributes.Add(new CustomAttribute(oldAttr.Constructor, oldAttr.GetBlob()));
                }
            }

            return newParameter;
        }
        #endregion

        #region Attributes
        /// <summary>
        /// Returns a <see cref="CustomAttributeArgument"/>'s <see cref="object"/>
        /// value.
        /// </summary>
        public static object GetValue(this CustomAttributeArgument arg)
        {
            CustomAttributeArgument[] arguments = arg.Value as CustomAttributeArgument[];

            if (arguments != null)
                return arguments.Select(GetValue).ToArray();

            object value = arg.Value;
            while (value is CustomAttributeArgument)
                value = ((CustomAttributeArgument)value).Value;

            return value;
        }

        /// <summary>
        /// Returns all <see cref="CustomAttribute"/>s that match <typeparamref name="TAttr"/>.
        /// </summary>
        public static IEnumerable<CustomAttribute> GetAttributes<TAttr>(this IMemberDefinition def) where TAttr : Attribute
            => def.CustomAttributes.Where(x => x.AttributeType.Is<TAttr>(true));

        /// <summary>
        /// Returns the first <see cref="CustomAttribute"/>s that matches <typeparamref name="TAttr"/>,
        /// or <code>null</code>.
        /// </summary>
        public static CustomAttribute GetAttribute<TAttr>(this IMemberDefinition def) where TAttr : Attribute
            => GetAttributes<TAttr>(def).FirstOrDefault();

        /// <summary>
        /// Returns all <see cref="CustomAttribute"/>s that match <typeparamref name="TAttr"/>.
        /// </summary>
        public static IEnumerable<CustomAttribute> GetAttributes<TAttr>(this AssemblyDefinition def) where TAttr : Attribute
            => def.CustomAttributes.Where(x => x.AttributeType.Is<TAttr>(true));

        /// <summary>
        /// Returns the first <see cref="CustomAttribute"/>s that matches <typeparamref name="TAttr"/>,
        /// or <code>null</code>.
        /// </summary>
        public static CustomAttribute GetAttribute<TAttr>(this AssemblyDefinition def) where TAttr : Attribute
            => GetAttributes<TAttr>(def).FirstOrDefault();

        /// <summary>
        /// Returns all <see cref="CustomAttribute"/>s that match <typeparamref name="TAttr"/>.
        /// </summary>
        public static IEnumerable<CustomAttribute> GetAttributes<TAttr>(this ParameterDefinition def) where TAttr : Attribute
            => def.CustomAttributes.Where(x => x.AttributeType.Is<TAttr>(true));

        /// <summary>
        /// Returns the first <see cref="CustomAttribute"/>s that matches <typeparamref name="TAttr"/>,
        /// or <code>null</code>.
        /// </summary>
        public static CustomAttribute GetAttribute<TAttr>(this ParameterDefinition def) where TAttr : Attribute
            => GetAttributes<TAttr>(def).FirstOrDefault();
        #endregion

        #region Compare
        /// <summary>
        /// Returns whether or not <paramref name="typeRef"/> is or inherits <paramref name="type"/>.
        /// </summary>
        public static bool Is(this TypeReference typeRef, Type type, bool acceptDerivedTypes = true)
        {
            if (typeRef is GenericInstanceType)
            {
                var genArgs = ((GenericInstanceType)typeRef).GenericArguments;

                if (type.GenericTypeArguments.Length != genArgs.Count)
                    return false;

                return typeRef.Namespace == type.Namespace && typeRef.Name == type.Name
                       && type.GenericTypeArguments.CompareSequences(genArgs, (sr, mc) => sr.Is(mc));
            }

            TypeDefinition def;
            return typeRef.FullName == type.FullName ||
                (acceptDerivedTypes && (def = typeRef.Resolve())?.BaseType != null && def.BaseType.Is(type, true));
        }

        /// <inheritdoc cref="Is(TypeReference, Type, bool)"/>
        public static bool Is(this TypeReference typeRef, TypeReference type, bool acceptDerivedTypes = true)
        {
            TypeDefinition def;

            if (typeRef.IsGenericInstance == type.IsGenericInstance && typeRef is GenericInstanceType)
            {
                var genArgs = ((GenericInstanceType)typeRef).GenericArguments;
                var genArgs2 = ((GenericInstanceType)type).GenericArguments;

                if (genArgs2.Count != genArgs.Count)
                    return false;

                return typeRef.Namespace == type.Namespace && typeRef.Name == type.Name && genArgs.SequenceEqual(genArgs2);
            }

            typeRef = typeRef.GetElementType();
            type = type.GetElementType();

            return typeRef.FullName == type.FullName ||
                (acceptDerivedTypes && (def = typeRef.Resolve())?.BaseType != null && def.BaseType.Is(type, true));
        }

        /// <summary>
        /// Returns whether or not <paramref name="typeRef"/> is or inherits <typeparamref name="T"/>.
        /// </summary>
        public static bool Is<T>(this TypeReference typeRef, bool acceptDerivedTypes = true) => Is(typeRef, typeof(T), acceptDerivedTypes);

        /// <summary>
        /// Returns whether or not <paramref name="typeRef"/> is or inherits <paramref name="type"/>.
        /// </summary>
        public static bool Is(this Type type, TypeReference typeRef, bool acceptDerivedTypes = true) => Is(typeRef, type, acceptDerivedTypes);

        /// <summary>
        /// Returns whether or not <paramref name="methodRef"/> is or inherits <paramref name="method"/>.
        /// </summary>
        public static bool Is(this MethodReference methodRef, MethodInfo method)
            => methodRef.Name == method.Name && methodRef.DeclaringType.Is(method.DeclaringType)
            && methodRef.Parameters.Select(x => x.ParameterType.AsType()).SequenceEqual(method.GetParameters().Select(x => x.ParameterType));

        /// <summary>
        /// Returns whether or not <paramref name="propRef"/> is or inherits <paramref name="prop"/>.
        /// </summary>
        public static bool Is(this PropertyReference propRef, PropertyInfo prop) => propRef.Name == prop.Name && propRef.DeclaringType.Is(prop.DeclaringType);

        /// <summary>
        /// Returns whether or not <paramref name="fieldRef"/> is or inherits <paramref name="field"/>.
        /// </summary>
        public static bool Is(this FieldReference fieldRef, FieldInfo field) => fieldRef.Name == field.Name && fieldRef.DeclaringType.Is(field.DeclaringType);
        #endregion

        #region Import
        /// <summary>
        /// Imports the given <see cref="TypeReference"/>, and returns it.
        /// </summary>
        public static TypeReference Import(this TypeReference reference) => Processor.TargetModuleDefinition.ImportReference(reference);

        /// <summary>
        /// Imports the given <see cref="MethodReference"/>, and returns it.
        /// </summary>
        public static MethodReference Import(this MethodReference reference) => Processor.TargetModuleDefinition.ImportReference(reference);

        /// <summary>
        /// Imports the given <see cref="FieldReference"/>, and returns it.
        /// </summary>
        public static FieldReference Import(this FieldReference reference) => Processor.TargetModuleDefinition.ImportReference(reference); 
        #endregion
    }
}
