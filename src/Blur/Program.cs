using System;
using System.IO;
using System.Reflection;

namespace Blur.Processing
{
    /// <summary>
    /// Represents the main class of the processor.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main function that'll call <see cref="Processor.Process(string, string)"/>
        /// with the given arguments.
        /// </summary>
        public static int Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 3)
            {
                Console.Error.WriteLine("Usage: blur.exe [target] [save] [references]");
                Console.Error.WriteLine(" - target:     Path to the assembly to process.");
                Console.Error.WriteLine(" - save:       Path to the file that'll be created.");
                Console.Error.WriteLine(" - references: Semicolon-separated list containing");
                Console.Error.WriteLine("               all the target's references.");

#if DEBUG
                Console.ReadLine();
#endif

                return 1;
            }

            try
            {
                if (args.Length == 3)
                    AssemblyResolver.References = args[2].Split(';');

                Processor.Process(Path.GetFullPath(args[0]), args.Length > 1 ? Path.GetFullPath(args[1]) : null);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                Console.WriteLine("Error encountered whilst processing the given assembly:");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine(e.StackTrace);

#if DEBUG
                Console.ReadLine();
#endif

                return 1;
            }

            return 0;
        }
    }
}
