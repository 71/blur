using System.Diagnostics.CodeAnalysis;
using Mono.Cecil;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    using Advices;

    public static class TestAdvices
    {
        public static T ReturnValue<T>(T val) => val;

        [Mixin]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static void Mixin(TypeDefinition type)
        {
            MethodDefinition method = type.FindMethod(nameof(ReturnValue), null);

            method.Advise(() =>
            {
                object val = Advice.Arguments[0];

                if (val is bool)
                    Advice.Return(true);

                Advice.Return(Advice.Invoke(Advice.Arguments));
            });
        }

        #region Tests
        [Fact]
        public static void ReturnValueShouldReturnGivenValue()
        {
            ReturnValue(true).ShouldBeFalse();
            ReturnValue(false).ShouldBeTrue();
            ReturnValue("hello").ShouldBe("hello");
        }
        #endregion
    }
}
