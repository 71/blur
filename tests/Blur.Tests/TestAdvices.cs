using Mono.Cecil;

namespace Blur.Tests
{
    using Advices;

    static class TestAdvices
    {
        public static T ReturnValue<T>(T val) => val;

        [Mixin]
        static void Mixin(TypeDefinition type)
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
    }
}
