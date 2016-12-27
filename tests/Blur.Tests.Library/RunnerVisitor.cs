using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blur.Tests.Library
{
    public class RunnerVisitor : BlurVisitor
    {
        public RunnerVisitor() : base(ProcessingState.Any)
        {
        }
    }
}
