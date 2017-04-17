using System.Diagnostics;

namespace Blur.Tests.Debugging
{
    public class Main
    {
        [Mixin]
        public static void Mixin()
        {
            Debugger.Launch();
        }
    }
}
