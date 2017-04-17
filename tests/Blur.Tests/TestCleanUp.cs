using System;
using System.Diagnostics.CodeAnalysis;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    public static class TestCleanUp
    {
        [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
        private static Type GetBlurType() => typeof(Bridge);

        [Fact]
        public static void ShouldThrowIfReferencingBlur()
        {
            Should.Throw<Exception>(() => GetBlurType());
        }

        [Fact]
        public static void ShouldNotFindBlurTypes()
        {
            Type.GetType(nameof(TrueAttribute)).ShouldBeNull();

            Type.GetType("Blur.Bridge").ShouldBeNull();
            Type.GetType("Mono.Cecil.AssemblyDefinition").ShouldBeNull();
        }
    }
}
