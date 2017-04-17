using System;
using System.Diagnostics.CodeAnalysis;
using Blur.Tests.Library;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    public static class TestContracts
    {
        private static int GetStringLength([NotNull] string str)
        {
            return str.Length;
        }

        [return: NotNull]
        private static string GetNull()
        {
            return null;
        }

        [Fact]
        public static void ShouldRespectContracts()
        {
            Should.Throw<ArgumentNullException>(() => GetStringLength(null));
            Should.Throw<ArgumentNullException>(() => GetNull());
        }
    }
}
