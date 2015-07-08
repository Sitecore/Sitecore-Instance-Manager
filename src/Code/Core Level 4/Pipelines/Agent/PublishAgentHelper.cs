using System;
using System.IO;
using System.Threading;
using SIM.Base;
using SIM.Instances;

namespace SIM.Pipelines.Agent
{
  public static class PublishAgentHelper
  {
    public static void Publish(Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      string publishUrl = AgentHelper.GetUrl(instance, PublishAgentFiles.PublishFileName);

      string statusUrl = AgentHelper.GetUrl(instance, PublishAgentFiles.StatusFileName);

      ExecuteAgent(AgentFiles.StatusFileName, statusUrl, PublishAgentFiles.PublishFileName, publishUrl);
    }

    public static void CopyAgentFiles(Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      string agent = Path.Combine(instance.WebRootPath, AgentHelper.AgentPath);
      FileSystem.Local.Directory.Ensure(agent);

      var files = new[]
        {
          new {FileName = PublishAgentFiles.PublishFileName, Contents = PublishAgentFiles.PublishContents}, 
          new {FileName = PublishAgentFiles.StatusFileName, Contents = PublishAgentFiles.StatusContents}
        };

      foreach (var file in files)
      {
        string targetFilePath = Path.Combine(agent, file.FileName);
        FileSystem.Local.Directory.DeleteIfExists(targetFilePath);
        FileSystem.Local.File.WriteAllText(targetFilePath, file.Contents);
      }
    }

    private static void ExecuteAgent(string statusFileName, string statusUrl, string agentName, string operationUrl)
    {
      // Call agent main operation
      string status = AgentHelper.Request(operationUrl, agentName);

      // If the package installation process takes more than http timeout, retrieve status
      if (!IsCompleted(status))
      {
        // Retrieve status while the previous request timed out, status is in progress or package is already being installed
        while (!IsCompleted(status) && !status.StartsWith("Failed", StringComparison.OrdinalIgnoreCase))
        {
          Thread.Sleep(2000);
          status = AgentHelper.Request(statusUrl, statusFileName);
        }

        // Break the process if something went wrong and the package is not installed
        Assert.IsTrue(IsCompleted(status), status);
      }
    }

    private static bool IsCompleted(string status)
    {
      return status.StartsWith("Completed", StringComparison.OrdinalIgnoreCase);
    }
  }
}