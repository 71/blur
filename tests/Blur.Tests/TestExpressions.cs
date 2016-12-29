using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;
using Blur.Extensions;

namespace Blur.Tests
{
    static class TestExpressions
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
            greatestMethod.Rewrite().Body<Func<int, int, int>>((a, b) => a > b ? a : b);

            //greatestMethod.Rewrite()
            //              .Body<Func<int>>(() =>
            //              {
            //                  return Context.Argument<int>(0) > Context.Argument<int>(1)
            //                      ? Context.Argument<int>(0)
            //                      : Context.Argument<int>(1);
            //              });
        }
    }
}
