using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Blur.Tests
{
    /// <summary>
    /// Forces the given method to return <see langword="true"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue)]
    internal class TrueAttribute : Attribute, IReturnValueWeaver
    {
        /// <summary>
        /// Replaces the body of this method by <c>return <see langword="true"/></c>.
        /// </summary>
        public void Apply(MethodReturnType returnType, MethodDefinition method)
        {
            if (!method.IsExtern())
                throw new Exception("The given method must be extern.");

            method.Rewrite().True().Return();
        }
    }
}

