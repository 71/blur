using System;
using System.Collections;
using Mono.Cecil;

namespace Blur.Annotations
{
    /// <summary>
    /// Makes sure the value of the marked parameter is within the specified range.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class RangeAttribute : Attribute, IParameterWeaver
    {
        public int Min { get; }
        public int Max { get; }

        public RangeAttribute(int min)
        {
            Min = min;
            Max = int.MaxValue;
        }

        public RangeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }

        /// <inheritdoc />
        public void Apply(ParameterDefinition parameter, MethodDefinition method)
        {
            ContractUtils.CheckExternal(method);

            // We accept this attribute on both numerical values, and
            // collections.

            TypeSystem ts = Bridge.TargetAssemblyDefinition.MainModule.TypeSystem;
            TypeDefinition def = parameter.ParameterType.Resolve();

            int paramIndex = parameter.Sequence;

            if (method.HasThis)
                paramIndex--;

            if (def.Implements<ICollection>())
            {
                method.Write().ToStart()
                      .Delegate(Context.Argument<ICollection>(paramIndex), Min, Max, parameter.Name, (coll, min, max, name) =>
                      {
                          if (coll.Value.Count < min || coll.Value.Count > max)
                              throw new ArgumentOutOfRangeException(name);
                      });
            }
            else if (def == ts.Int32)
            {
                method.Write().ToStart()
                      .Delegate(Context.Argument<int>(paramIndex), Min, Max, parameter.Name, (coll, min, max, name) =>
                      {
                          if (coll.Value < min || coll.Value > max)
                              throw new ArgumentOutOfRangeException(name);
                      });
            }
            else
            {
                throw new Exception("A collection or numeric value must be given.");
            }
        }
    }
}
