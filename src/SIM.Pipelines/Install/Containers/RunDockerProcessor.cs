using JetBrains.Annotations;
using System;
using SIM.Loggers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install.Containers
{
  public class RunDockerProcessor : Processor
  {
    private ILogger _logger;

    protected virtual string Command => "docker-compose.exe up -d";

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      InstallContainerArgs args = (InstallContainerArgs)arguments;
      this._logger = args.Logger;
      string strCmdText = $"/C cd \"{args.Destination}\"&{this.Command}";
      System.Diagnostics.Process proc = new System.Diagnostics.Process();
      proc.StartInfo.Arguments = strCmdText;
      proc.StartInfo.FileName = "CMD.exe";
      proc.StartInfo.UseShellExecute = false;
      proc.StartInfo.RedirectStandardError = true;
      proc.StartInfo.RedirectStandardOutput = true;
      proc.OutputDataReceived += Proc_DataReceived;
      proc.ErrorDataReceived += Proc_DataReceived;
      proc.Start();
      proc.BeginOutputReadLine();
      proc.BeginErrorReadLine();
      proc.WaitForExit();
      if (proc.ExitCode != 0)
      {
        throw new AggregateException($"Failed to run '{this.Command}'\n{proc.StandardError.ReadToEnd()}");
      }
    }

    private void Proc_DataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.Data))
      {
        this._logger.Info(e.Data, false);
      }
    }
  }
}
