using System;
using System.Linq.Expressions;
using Mono.Cecil;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    public static class TestExpressions
    {
        public static int Add(int a, int b)
        {
            return a - b;
        }


        public static int Greatest(int a, int b)
        {
            return a > b ? b : a;
        }

        [Mixin]
        public static void FixMethods(TypeDefinition type)
        {
            MethodDefinition addMethod = type.FindMethod(nameof(Add), null);
            MethodDefinition greatestMethod = type.FindMethod(nameof(Greatest), null);

            ParameterExpression[] parameters =
            {
                Expression.Parameter(typeof(int), "a"), Expression.Parameter(typeof(int), "b")
            };

            Expression<Func<int, int, int>> expr = Expression.Lambda<Func<int, int, int>>(
                Expression.Add(parameters[0], parameters[1]), parameters
            );

            addMethod.Rewrite().Expression(expr);

            greatestMethod.Rewrite()
                          .Function(Context.Argument<int>(0), Context.Argument<int>(1), (a, b) => a > b ? a : b);
        }

        #region Tests
        [Fact]
        public static void AddShouldWork()
        {
            Add(1, 2).ShouldBe(3);
        }

        [Fact]
        public static void GreatestShouldReturnGreatest()
        {
            Greatest(1, 2).ShouldBe(2);
        }
        #endregion
    }
}
