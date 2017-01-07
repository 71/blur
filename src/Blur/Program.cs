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
        /// Main function that'll interact with the <see cref="Processor"/>
        /// with the given arguments.
        /// </summary>
        public static int Main(string[] args)
        {
            bool willPreprocess = Array.IndexOf(args, "-p") == 0;

            if (willPreprocess)
            {
                string[] tmp = new string[args.Length - 1];
                Array.Copy(args, 1, tmp, 0, tmp.Length);
                args = tmp;
            }

            if (args.Length == 0 || args.Length > 3)
            {
                Console.WriteLine("Usage: blur.exe [-p] [target] [save] [references]");
                Console.WriteLine(" - p:          Preprocess the assembly.");
                Console.WriteLine(" - target:     Path to the assembly to process.");
                Console.WriteLine(" - save:       Path to the file that'll be created.");
                Console.WriteLine(" - references: Semicolon-separated list containing");
                Console.WriteLine("               all the target's references.");

#if DEBUG
                Console.ReadLine();
#endif

                return 1;
            }

            try
            {
                if (args.Length == 3)
                    AssemblyResolver.References = args[2].Split(';');

                Processor.Initialize(Path.GetFullPath(args[0]), args.Length > 1 ? Path.GetFullPath(args[1]) : null);

                if (willPreprocess)
                    Processor.Preprocess();

                Processor.Process();
            }
            catch (Exception e)
            {
                Processor.Cancel();

                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                Console.Error.WriteLine("Error encountered whilst processing the given assembly:");

                while (e != null)
                {
                    Console.Error.WriteLine(e.Message);
                    Console.Error.WriteLine();
                    Console.Error.WriteLine(e.StackTrace);
                    Console.Error.WriteLine();

                    e = e.InnerException;
                }

#if DEBUG
                Console.ReadLine();
#endif

                return 1;
            }

            Processor.Dispose();

            return 0;
        }
    }
}
