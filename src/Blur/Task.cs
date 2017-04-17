using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Blur.Processing
{
    public sealed class BlurTask : Task
    {
        [Required]
        public string TargetAssembly { get; set; }

        [Required]
        public string TargetReferences { get; set; }

        [Required]
        public string TargetPath { get; set; }

        public bool Preprocess { get; set; }

        public bool Debug { get; set; }

        public override bool Execute()
        {
            if (Debug)
                Debugger.Launch();

            try
            {
                AssemblyResolver.References = TargetReferences.Split(';');

                Processor.MessageLogged += Processor_MessageLogged;
                Processor.WarningLogged += Processor_WarningLogged;

                Processor.Initialize(Path.GetFullPath(TargetAssembly), TargetPath);

                if (Preprocess)
                    Processor.Preprocess();

                Processor.Process();
            }
            catch (Exception e)
            {
                Processor.Cancel();

                if (e is TargetInvocationException && e.InnerException != null)
                    e = e.InnerException;

                StringBuilder msg = new StringBuilder();

                while (true)
                {
                    msg.AppendLine(e.Message);
                    msg.AppendLine(e.StackTrace);
                    msg.AppendLine();

                    if (e.InnerException == null)
                        break;

                    e = e.InnerException;
                }

                string targetName = e.TargetSite == null
                        ? "Unknown"
                        : $"{e.TargetSite.DeclaringType.FullName}.{e.TargetSite.Name}";

                BuildEngine.LogErrorEvent(new BuildErrorEventArgs("Error", e.HResult.ToString(), e.Source, 0, 0, 0, 0, msg.ToString(), e.HelpLink, targetName));

                return false;
            }

            Processor.Dispose();

            return true;
        }

        private void Processor_WarningLogged(string obj)
        {
            BuildEngine.LogWarningEvent(new BuildWarningEventArgs("Warning", "", "", 0, 0, 0, 0, obj, "", ""));
        }

        private void Processor_MessageLogged(string obj)
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(obj, "", "", MessageImportance.Normal));
        }
    }
}
