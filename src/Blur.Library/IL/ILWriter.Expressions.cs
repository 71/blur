using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace Blur
{
    partial class ILWriter
    {
        #region Private utils
        private static readonly TypeInfo LambdaCompilerType =
            typeof(Expression).GetTypeInfo().Assembly.GetType("System.Linq.Expressions.Compiler.LambdaCompiler").GetTypeInfo();

        /// <summary>
        /// Create a <see cref="DynamicMethod"/> from a <see cref="LambdaExpression"/>,
        /// using internal type <code>System.Linq.Expressions.Compiler.LambdaCompiler</code>.
        /// </summary>
        private static ILGenerator Compile(LambdaExpression lambda, bool inlined)
        {
            // Internal types here:
            //  - https://source.dot.net/#System.Linq.Expressions/System/Linq/Expressions/Compiler/LambdaCompiler.cs
            //  - https://source.dot.net/#System.Private.CoreLib/src/System/Reflection/Emit/ILGenerator.cs
            //
            // The following code equals:
            //
            //  AnalyzedTree tree = LambdaCompiler.AnalyzeTree(ref lambda);
            //  LambdaCompiler compiler = new LambdaCompiler(tree, lambda);
            //
            //  compiler.EmitLambdaBody(null, true, lambda.TailCall ? CompilationFlags.EmitAsTail : CompilationFlags.EmitAsNoTail);
            //  return compiler._method;

            object tree = LambdaCompilerType
                .DeclaredMethods.First(x => x.Name == "AnalyzeLambda" && x.IsStatic)
                .Invoke(null, new object[] { lambda });

            object compiler = Activator.CreateInstance(LambdaCompilerType.AsType(), tree, lambda);

            LambdaCompilerType
                .DeclaredMethods.First(x => x.Name == "EmitLambdaBody" && !x.IsStatic)
                .Invoke(compiler, new object[] { null, inlined, lambda.TailCall ? 0x0100 : 0x0400 });

            return (ILGenerator)LambdaCompilerType
                    .DeclaredFields.First(x => x.Name == "_ilg" && !x.IsStatic)
                    .GetValue(compiler);
        }

        /// <summary>
        /// Return an <see cref="ILGenerator"/>'s body as a <see cref="byte"/>
        /// array.
        /// </summary>
        private static byte[] GetBodyAsBytes(ILGenerator il)
        {
            // Internal types here:
            //  - https://source.dot.net/#System.Private.CoreLib/src/System/Reflection/Emit/ILGenerator.cs
            //
            // The following code equals:
            // 
            //  return il.BakeByteArray();

            return (byte[])il.GetType().GetTypeInfo()
                .DeclaredMethods.First(x => x.Name == "BakeByteArray" && !x.IsStatic)
                .Invoke(il, new object[0]);
        }
        #endregion

        /// <summary>
        /// Convert a <see cref="LambdaExpression"/> to <see cref="Instruction"/>s.
        /// </summary>
        public static IEnumerable<Instruction> FromExpression<TDelegate>(Expression<TDelegate> expr)
        {
            return FromByteArray(GetBodyAsBytes(Compile(expr, false)));
        }

        /// <summary>
        /// Convert an <see cref="System.Linq.Expressions.Expression"/> to <see cref="Instruction"/>s.
        /// </summary>
        public static IEnumerable<Instruction> FromExpression(Expression expr)
        {
            return FromByteArray(GetBodyAsBytes(Compile(System.Linq.Expressions.Expression.Lambda(expr, false), true)));
        }

        /// <summary>
        /// Print the IL representation of a <see cref="LambdaExpression"/>.
        /// </summary>
        public ILWriter Expression<TDelegate>(Expression<TDelegate> expr)
            => this.Parse(GetBodyAsBytes(Compile(expr, false)));

        /// <summary>
        /// Print the IL representation of an <see cref="System.Linq.Expressions.Expression"/>.
        /// </summary>
        public ILWriter Expression(Expression expr)
            => this.Parse(GetBodyAsBytes(Compile(System.Linq.Expressions.Expression.Lambda(expr, false), true)));
    }
}