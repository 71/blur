using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Blur.Tests.Library;
using Shouldly;

namespace Blur.Tests
{
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

        [return: True]
        static extern bool ReturnTrue();

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

            TestExpressions.Add(1, 2).ShouldBe(3);
            TestExpressions.Greatest(1, 2).ShouldBe(2);

            ReturnTrue().ShouldBe(true);
            HasBeenModified.ShouldBe(true);

            Console.WriteLine("Tests were successful.");
        }
    }
}
