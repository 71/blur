using System;
using System.Linq;
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
        static bool HasBeenModified = false;

        static int GetStringLength([NotNull] string str)
        {
            return str.Length;
        }

        [return: NotNull]
        static string GetNull()
        {
            return null;
        }

        [Mixin]
        static void Mixin(TypeDefinition type)
        {
            FieldDefinition hbmField = type.Fields.First(x => x.Name == nameof(HasBeenModified));
            MethodDefinition cctor = type.Methods.First(x => x.Name == ".cctor");

            cctor.Write().ForEach(ins =>
            {
                if (ins.OpCode == OpCodes.Stsfld && (ins.Operand as FieldDefinition).Name == nameof(HasBeenModified)
                 && ins.Previous.OpCode == OpCodes.Ldc_I4_0)
                    ins.Previous.OpCode = OpCodes.Ldc_I4_1;
            });
        }

        [BlockMethod]
        static void Main(string[] args)
        {
            Should.Throw<ArgumentNullException>(() => GetStringLength(null));
            Should.Throw<ArgumentNullException>(() => GetNull());

            HasBeenModified.ShouldBe(true);

            Console.WriteLine("Tests were successful.");
        }
    }
}
