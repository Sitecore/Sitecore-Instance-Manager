using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using System;
using System.IO;
using Sitecore.Diagnostics.Base;

namespace SIM.Pipelines.Install.Containers
{
  public abstract class RunCommandBaseProcessor: Processor
  {
    protected abstract string GetDestination(ProcessorArgs arguments);
    protected abstract string GetCommand(ProcessorArgs arguments);

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");

      string command = GetCommand(arguments);

      if (string.IsNullOrEmpty(command))
      {
        throw new InvalidOperationException($"Failed to generate the command in '{this.GetType().Name}'");
      }

      string destination = GetDestination(arguments);

      if (string.IsNullOrEmpty(destination))
      {
        throw new InvalidOperationException($"Failed to resolve destination path in {this.GetType().Name}");
      }
      if (!Directory.Exists(destination))
      {
        throw new InvalidOperationException($"Resolve destination folder does not exist. Folder: '{destination}', type: '{this.GetType().Name}'");
      }

      RunCommandInProcess(destination, command);
    }

    private void RunCommandInProcess(string destination, string command)
    {
      string strCmdText = $"/C cd \"{destination}\"&{command}";
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
        throw new AggregateException($"Failed to run '{command}'\n{proc.StandardError.ReadToEnd()}");
      }
    }
  }
}