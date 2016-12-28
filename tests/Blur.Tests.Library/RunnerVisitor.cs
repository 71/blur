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
        private RunnerVisitor() : base(ProcessingState.Any)
        {
        }
    }
}
