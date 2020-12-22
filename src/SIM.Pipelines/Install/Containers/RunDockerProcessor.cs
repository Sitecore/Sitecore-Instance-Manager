using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install.Containers
{
  public class RunDockerProcessor : Processor
  {
    protected virtual string Command => "docker-compose.exe up -d";

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      InstallContainerArgs args = (InstallContainerArgs)arguments;
      string strCmdText = $"/C cd \"{args.Destination}\"&{this.Command}";
      System.Diagnostics.Process proc = new System.Diagnostics.Process();
      proc.StartInfo.Arguments = strCmdText;
      proc.StartInfo.FileName = "CMD.exe";
      proc.StartInfo.UseShellExecute = false;
      proc.StartInfo.RedirectStandardError = true;
      proc.StartInfo.RedirectStandardOutput = true;
      proc.Start();
      proc.WaitForExit();
      if (proc.ExitCode != 0)
      {
        throw new AggregateException($"Failed to run '{this.Command}'\n{proc.StandardError.ReadToEnd()}");
      }
    }
  }
}
