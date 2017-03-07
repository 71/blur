using System;
using Mono.Cecil;

namespace Blur
{
    internal static class ContractUtils
    {
        public static void CheckExternal(MethodDefinition method)
        {
            if (!method.HasBody)
                throw new InvalidOperationException("The given method must declare a body.");
        }
    }
}
