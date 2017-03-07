using System;
using Blur.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur
{
    /// <summary>
    /// Lazily retrieve the value returned by this member.
    /// <para>
    /// If a <see cref="PropertyDefinition"/> is given, only a get method must be declared.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false)]
    public sealed class LazyAttribute : Attribute, IPropertyWeaver, IMethodWeaver
    {
        /// <summary>
        /// Weave the given method to cache its value.
        /// </summary>
        public void Apply(MethodDefinition method)
        {
            // 0. Make sure the given method is good
            if (method.RVA == 0)
                throw new Exception("The given method cannot be external.");
            if (method.ReturnType == Bridge.TargetAssemblyDefinition.MainModule.TypeSystem.Void)
                throw new Exception("The given method cannot return void.");

            // 1. Create backing field
            //    It shouldn't already exist; if it does, not my problem
            FieldAttributes attrs = method.IsStatic
                ? FieldAttributes.Private
                : FieldAttributes.Private | FieldAttributes.Static;

            FieldDefinition backingField = new FieldDefinition($"{method.Name}+cachedValue", attrs, method.ReturnType);
            FieldDefinition booleanField = new FieldDefinition($"{method.Name}+cachedHasBeenRetrieved", attrs, Bridge.TargetAssemblyDefinition.MainModule.TypeSystem.Boolean);

            method.DeclaringType.Fields.Add(backingField);
            method.DeclaringType.Fields.Add(booleanField);

            // 2. Weave getter body
            MethodBody body = method.Body;
            ILWriter writer = body.Write();

            // 2.1. Check if field has already been set
            writer.ToStart()
                .Load(backingField)
                .If()
                    .Load(backingField)
                    .Return()
                .End();

            // 2.2. If not, change instructions to set body
            writer.ForEach(ins =>
            {
                if (ins.OpCode == OpCodes.Ret)
                    writer
                        .Before(ins)
                        .Dup()
                        .Save(backingField)
                        .True()
                        .Save(booleanField);
            });
        }

        /// <summary>
        /// Weave the given property to cache its value.
        /// </summary>
        public void Apply(PropertyDefinition property)
        {
            // Make sure the given property is good
            if (property.SetMethod != null)
                throw new Exception("A cached property mustn't have a set method.");
            if (property.GetMethod == null)
                throw new Exception("A cached property must declare a get method.");

            // Weave getter
            Apply(property.GetMethod);
        }
    }
}