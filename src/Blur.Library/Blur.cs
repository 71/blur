using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Mono.Cecil;

namespace Blur
{
    /// <summary>
    /// This class contains static methods giving access
    /// to the build process, and some data.
    /// </summary>
    public static class Blur
    {
        /// <summary>
        /// Gets the <see cref="Assembly"/> that is being modified.
        /// </summary>
        public static Assembly TargetAssembly => Processor.Target;

        /// <summary>
        /// Gets the <see cref="AssemblyDefinition"/> of the <see cref="Assembly"/>
        /// being modified.
        /// </summary>
        public static AssemblyDefinition TargetAssemblyDefinition => Processor.TargetDefinition;

        /// <summary>
        /// Logs a message to the build process.
        /// </summary>
        public static void Log(string msg, bool isWarning = false)
        {
            Processor.LogMessage(msg, isWarning);
        }

        /// <summary>
        /// Debugs the code currently running.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Debug()
        {
            if (Debugger.IsAttached)
                Debugger.Break();
            else
                Debugger.Launch();
        }
    }
}
