using JetBrains.Annotations;
using System;
using SIM.Loggers;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  public class RunDockerProcessor : Processor
  {
    protected virtual string Command => "docker-compose.exe up -d";

    private ILogger _logger;

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      InstallContainerArgs args = (InstallContainerArgs)arguments;
      Assert.ArgumentNotNull(args.Logger, nameof(args.Logger));
      this._logger = args.Logger;
      string strCmdText = $"/C cd \"{args.Destination}\"&{this.Command}";
      System.Diagnostics.Process proc = new System.Diagnostics.Process();
      proc.StartInfo.Arguments = strCmdText;
      proc.StartInfo.FileName = "CMD.exe";
      proc.StartInfo.UseShellExecute = false;
      proc.StartInfo.RedirectStandardError = true;
      proc.StartInfo.RedirectStandardOutput = true;
      proc.OutputDataReceived += Proc_OutputDataReceived;
      proc.Start();
      proc.BeginOutputReadLine();
      proc.WaitForExit();
      if (proc.ExitCode != 0)
      {
        throw new AggregateException($"Failed to run '{this.Command}'\n{proc.StandardError.ReadToEnd()}");
      }
    }

    private void Proc_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
    {
      if (!string.IsNullOrEmpty(e.Data))
      {
        this._logger.Info(e.Data);
      }
    }
  }
}
