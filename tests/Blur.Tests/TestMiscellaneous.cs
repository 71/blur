using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Shouldly;
using Xunit;

namespace Blur.Tests
{
    public static class TestMiscellaneous
    {
        private static readonly bool HasBeenModified = false;

        [Mixin]
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private static void Mixin(TypeDefinition type)
        {
            MethodDefinition cctor = type.Methods.First(x => x.Name == ".cctor");

            cctor.Write().ForEach(ins =>
            {
                if (ins.OpCode.Code == Code.Stsfld
                && ins.Previous?.OpCode.Code == Code.Ldc_I4_0
                && ((FieldDefinition)ins.Operand).Name == nameof(HasBeenModified))
                    ins.Previous.OpCode = OpCodes.Ldc_I4_1;
            });
        }

        #region Tests
        [Fact]
        public static void ShouldHaveReplacedInstruction()
        {
            HasBeenModified.ShouldBeTrue();
        }
        #endregion
    }
}
