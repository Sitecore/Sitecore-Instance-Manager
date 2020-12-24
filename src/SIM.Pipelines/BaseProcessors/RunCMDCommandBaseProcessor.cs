using JetBrains.Annotations;
using System;
using SIM.Loggers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.BaseProcessors
{
  public abstract class RunCmdCommandBaseProcessor : Processor
  {
    protected ILogger _logger;

    protected abstract string GetCommand(ProcessorArgs procArgs);

    protected abstract string GetExecutionFolder(ProcessorArgs procArgs);

    protected virtual ILogger GetLogger(ProcessorArgs procArgs)
    {
      return new EmptyLogger();
    }

    protected override void Process([NotNull] ProcessorArgs procArgs)
    {
      string command = GetCommand(procArgs);

      string executionFolder = GetExecutionFolder(procArgs);

      this._logger = GetLogger(procArgs);

      RunCommand(executionFolder, command);
    }

    private void RunCommand(string executionFolder, string command)
    {
      string strCmdText = $"/C cd \"{executionFolder}\"&{command}";
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
        throw new AggregateException($"Failed to run '{command}' in '{executionFolder}'\n{proc.StandardError.ReadToEnd()}");
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