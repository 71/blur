using System;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    /// <summary>
    /// Forces the given method to return <see langword="true"/>. 
    /// </summary>
    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class TrueAttribute : Attribute, IReturnValueWeaver
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

    public static class TestTrueAttribute
    {
        [return: True]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("Analyzers", "CS0626")]
        private static extern bool ReturnTrue();

        #region Tests
        [Fact]
        public static void ShouldReturnTrue()
        {
            ReturnTrue().ShouldBeTrue();
        }
        #endregion
    }
}

