using System;
using System.Diagnostics;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// Indicates that this method will launch the debugger
    /// when it is invoked.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
    public sealed class DebugAttribute : Attribute, IMethodWeaver
    {
        /// <summary>
        /// Add some statements to the method.
        /// </summary>
        public void Apply(MethodDefinition method)
        {
            // Make sure we've got a valid method.
            if (!method.HasBody)
                throw new Exception("Cannot debug a method with no body.");

            // Start the debugger.
            method.Write().ToStart()
                  .Action(() =>
                  {
                      if (Debugger.IsAttached)
                          Debugger.Break();
                      else
                          Debugger.Launch();
                  });
        }
    }
}
