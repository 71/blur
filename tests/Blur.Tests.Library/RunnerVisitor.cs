// TODO: Try this out once InvokeDefinition(this MethodDefinition) has been fixed.
#if false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Blur.Tests.Library
{
    /// <summary>
    /// <see cref="BlurVisitor"/> that runs the entry point of an assembly.
    /// </summary>
    public class RunnerVisitor : BlurVisitor
    {
        private RunnerVisitor() : base(ProcessingState.After)
        {
        }

        /// <summary>
        /// Run the entry point of an assembly.
        /// </summary>
        protected override void Visit(AssemblyDefinition assembly)
        {
            assembly.EntryPoint?.InvokeDefinition(null, new object[] { new string[0] });
        }
    }
}
#endif