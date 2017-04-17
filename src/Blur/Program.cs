using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Blur.Processing
{
    /// <summary>
    /// Represents the main class of the processor.
    /// </summary>
    public class Program
    {
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private sealed class Args
        {
            public bool Preprocess { get; set; }
            public bool Debug      { get; set; }
            public bool ShowHelp   { get; set; }

            public string AssemblyPath { get; set; }
            public string OutputPath   { get; set; }
            public string References   { get; set; }
        }

        /// <summary>
        /// Main function that'll interact with the <see cref="Processor"/>
        /// with the given arguments.
        /// </summary>
        public static int Main(string[] args)
        {
            Args parsedArgs = new FluentParser<Args>()
                .Define(x => x.Preprocess,   arg => arg.As('p').As("pre", "preprocess"))
                .Define(x => x.Debug,        arg => arg.As('d').As("debug"))
                .Define(x => x.ShowHelp,     arg => arg.As('h').As("help"))
                .Define(x => x.OutputPath,   arg => arg.As('o').As("output"))
                .Define(x => x.References,   arg => arg.As('r').As("ref", "refs", "references", "reference"))
                .Define(x => x.AssemblyPath, arg => arg.Main().Required(true))
                .Parse(args);

            if (args.Length == 0 || parsedArgs.ShowHelp)
            {
                Console.WriteLine("Usage: blur.exe [-p] [-d] [-o output] [-r references] [target]");
                Console.WriteLine(" - p:          Preprocess the assembly.");
                Console.WriteLine(" - d:          Debug the processing operation.");
                Console.WriteLine(" - target:     Path to the assembly to process.");
                Console.WriteLine(" - output:     Path to the file that'll be created.");
                Console.WriteLine(" - references: Semicolon-separated list containing");
                Console.WriteLine("               all the target's references.");

                return args.Length == 0 ? 1 : 0;
            }

            try
            {
                if (parsedArgs.Debug)
                    Debugger.Launch();

                if (parsedArgs.References != null)
                    AssemblyResolver.References = parsedArgs.References.Split(';');

                Processor.Initialize(parsedArgs.AssemblyPath, parsedArgs.OutputPath);

                if (parsedArgs.Preprocess)
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

                return 1;
            }

            Processor.Dispose();

            return 0;
        }
    }
}
