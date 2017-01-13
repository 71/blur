# Blur
Blur uses [Mono.Cecil](https://github.com/jbevain/cecil) to weave assemblies from the inside.
To make this easier, Blur provides fluent APIs for IL generation and `MethodDefinition` invocation.

## Five-minutes annotated guide
```csharp
using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Blur;

// The only requirement when using Blur is to mark
// the assembly with the BlurAttribute.
// This attribute also exposes some useful settings.
// Here, the CleanUp property is set to true:
//  bool CleanUp { get; set; }
// In this case, all references to Mono.Cecil and Blur will
// be removed from the assembly, including all weavers
// and visitors.
[assembly: Blur(CleanUp = true)]

/// <summary>
/// Forces the marked method to block its execution before
/// returning.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
sealed class BlockMethodAttribute : Attribute, IMethodWeaver
{
    // The IMethodWeaver interface must be set on this attribute
    // to register it as a compile-time attribute.
    // Its only declaration is:
    //  void Apply(MethodDefinition)
    // There are other interfaces such as this one, for every element
    // that can be marked with an attribute, EXCEPT assemblies and modules.

    private readonly static MethodReference ReadLineMethod
        = typeof(Console).GetMethod(nameof(Console.ReadLine)).GetReference();

    /// <summary>
    /// Prepends <see cref="Console.ReadLine"/> before all
    /// <see langword="ret"/> op codes.
    /// </summary>
    public void Apply(MethodDefinition method)
    {
        // The ILWriter is the class used by Blur users to edit methods.
        // It provides a Fluent API, and the ability to inject non-IL code,
        // such as LINQ Expressions and delegates, directly into bodies.
        //
        // The ILWriter can be obtained with 4 methods:
        //  ILWriter Write(this MethodBody body)
        //  ILWriter Write(this MethodDefinition method)
        //  ILWriter Rewrite(this MethodBody body)
        //  ILWriter Rewrite(this MethodDefinition method)
        //
        // Using Rewrite() will reset the method's body.
        ILWriter writer = method.Write();

        // Iterate over every instruction in the method's body.
        // If the body is changed during an iteration, the
        // current position of the ILWriter (obtainable with the
        // ILWriter.Position property) will be automatically updated.
        writer.ForEach(ins =>
        {
            // If the opcode of the instruction is "ret", ...
            // For those of you that are not familiar with IL, "ret" is
            // the opcode that corresponds to "return the value at the top of the stack".
            if (ins.OpCode == OpCodes.Ret)
                // ILWriter's fluent API chains calls together.
                // The following expression will write the following IL code:
                //  call    string [mscorlib]System.Console::ReadLine()
                //  pop
                // Not that Call() automatically inserts this if the method is
                // an instance method, and uses Callvirt if needed.
                writer
                    .Before(ins)            // Position writer before the given instruction
                    .Call(ReadLineMethod)   //  call string [mscorlib]System.Console::ReadLine()
                    .Pop();                 //  pop
        });
    }
}

class Program
{
    [BlockMethod]
    public static void Main(string[] args)
    {
        Console.WriteLine("Hopefully, you will have time to see this message...");
    }
}
```

## How to install
#### `Install-Package Blur -Pre`
Currently compatible with:
- .NET Standard 1.3 and over.
- .NET 4.5 PCL and over.

## Features
#### `ILWriter`
The `ILWriter` class provides fluent methods meant to make editing
IL easy. IL can be emitted:
- Using delegates (via `ILWriter.Delegate(delegate)`).
- Using LINQ Expressions (via `ILWriter.Expression(Expression)`).
- Using a simplified emitter.
- Using raw IL (via `ILWriter.Emit(OpCode, ...)` and `Blur.Extensions.ILWriterExtensions`).

#### `BlurVisitor`
The `BlurVisitor` provides many methods that will be called whenever
a declaration is about to or has been modified by Blur:
- `void Visit(TypeDefinition type)`
- `void Visit(FieldDefinition field)`
- `void Visit(MethodDefinition method)`
- ...

Any class that inherits `BlurVisitor` and is in `BlurAttribute.Visitors` will automatically be created and used when weaving an assembly.

#### In-Assembly weaving
- The entire assembly is available to you during compilation.
- Methods can be invoked at any time, even if they have been previously modified.
- Placing a breakpoint in a `IWeaver` method is enough to debug the modification of your assembly.

#### Miscellaneous
- Conversion helpers, adding the ability to convert from `System.Reflection` to `Mono.Cecil`, and vise-versa (via `Blur.BlurExtensions`).
- **Fully** documented assembly, including every IL instruction.