# Blur
Blur uses [Mono.Cecil](https://github.com/jbevain/cecil) to edit assemblies.  
To make this easier, Blur provides fluent APIs for IL generation and `MethodDefinition` invocation.

## One-minute guide
```csharp
using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Blur;

/// <summary>
/// Forces all calls to ldc.i4 to load 2
/// instead of 1.
/// </summary>
class OneToTwoAttribute : MethodWeaverAttribute
{
	public override void Apply(MethodDefinition method)
    {
    	foreach (var instr in method.Body.Instructions)
        {
            if (instr.OpCode == OpCodes.Ldc_I4 && instr.Operand.Equals(1))
                instr.Operand = 2;
        }
    }
}

class Program
{
	[OneToTwo]
    static void Main(string[] args)
    {
    	int result = 1 + 1;
    	if (result == 2)
        	Console.Error.WriteLine("1+1 equals " + result + ", duh.");
    }
}
```

## How to install
#### `Install-Package Blur -Pre`
Currently compatible with:  
- .NET Standard 1.3 and over.
- .NET 4.5 PCL and over.