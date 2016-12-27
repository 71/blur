using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Blur.Tests.Library;
using Shouldly;

namespace Blur.Tests
{
    /// <summary>
    /// Forces the marked method to block its execution before
    /// returning.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    class BlockMethodAttribute : Attribute, IMethodWeaver
    {
        /// <summary>
        /// Prepends <see cref="Console.ReadLine"/> before all
        /// <see langword="ret"/> op codes.
        /// </summary>
        public void Apply(MethodDefinition method)
        {
            ILWriter writer = method.Write();

            writer.ForEach(ins =>
            {
                if (ins.OpCode == OpCodes.Ret)
                    writer.Before(ins).Call(typeof(Console).GetMethod(nameof(Console.ReadLine))).Pop();
            });
        }
    }

    class Program
    {
        static int GetStringLength([NotNull] string str)
        {
            return str.Length;
        }

        [return: NotNull]
        static string GetNull()
        {
            return null;
        }

        [BlockMethod]
        static void Main(string[] args)
        {
            Should.Throw<ArgumentNullException>(() => GetStringLength(null));
            Should.Throw<ArgumentNullException>(() => GetNull());

            Console.WriteLine("Tests were successful.");
        }
    }
}
