namespace SIM.Pipelines.Agent
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Threading;
  using System.Web;
  using System.Xml;
  using SIM.Instances;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class AgentHelper
  {
    #region Constants

    private const string ActionsPerformed = @"Done: actions performed";

    private const string ActionsPerforming = "Started: actions performing";

    public const string AgentPath = "sitecore/shell/sim-agent";

    public const string InProgress = @"Pending: no information";

    private const string PackageInstalled = @"Done: package installed";

    private const string PackageInstalling = @"Started: package is being installed";

    public const string TimedOut = "timed out";

    #endregion

    #region Copy-Delete agent files

    public static void CopyAgentFiles([NotNull] Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      string agent = Path.Combine(instance.WebRootPath, AgentPath);
      FileSystem.FileSystem.Local.Directory.Ensure(agent);
      var files = new[]
      {
        new
        {
          FileName = AgentFiles.InstallPackageFileName, 
          Contents = AgentFiles.InstallPackageContents
        }, 
        new
        {
          FileName = AgentFiles.PostInstallActionsFileName, 
          Contents = AgentFiles.PostInstallActionsContents
        }, 
        new
        {
          FileName = AgentFiles.StatusFileName, 
          Contents = AgentFiles.StatusContents
        }
      };

      foreach (var file in files)
      {
        string targetFilePath = Path.Combine(agent, file.FileName);
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(targetFilePath);
        FileSystem.FileSystem.Local.File.WriteAllText(targetFilePath, file.Contents);
      }
    }

    public static void CopyPackages([NotNull] Instance instance, [NotNull] IEnumerable<Product> modules)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(modules, "modules");

      string packages = FileSystem.FileSystem.Local.Directory.Ensure(instance.PackagesFolderPath);
      foreach (Product product in modules)
      {
        string targetFilePath = Path.Combine(packages, Path.GetFileName(product.PackagePath).EmptyToNull().IsNotNull());
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(targetFilePath);

        FileSystem.FileSystem.Local.File.Copy(product.PackagePath, targetFilePath);
      }
    }

    public static void DeleteAgentFiles([NotNull] Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      string agent = Path.Combine(instance.WebRootPath, AgentPath);
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(agent);
    }

    #endregion

    #region Package installation

    public static void InstallPackage([NotNull] Instance instance, [NotNull] Product module)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(module, "module");

      string fileName = Path.GetFileName(module.PackagePath);
      Assert.IsNotNull(fileName, "name");

      string installPackageUrl = GetUrl(instance, AgentFiles.InstallPackageFileName, fileName);

      string statusUrl = GetUrl(instance, AgentFiles.StatusFileName);

      ExecuteAgent(AgentFiles.StatusFileName, statusUrl, AgentFiles.InstallPackageFileName, installPackageUrl, PackageInstalling, PackageInstalled);
    }

    public static void PerformPostStepAction([NotNull] Instance instance, [NotNull] Product module)
    {
      XmlDocument xmlDocument = module.Manifest;
      bool skipPostActions = module.SkipPostActions;

      string statusUrl = GetUrl(instance, AgentFiles.StatusFileName);

      if (xmlDocument != null)
      {
        IEnumerable<XmlElement> elements = xmlDocument.SelectElements(Product.ManifestPrefix + "package/install/postStepActions/*");
        List<string[]> custom = new List<string[]>();
        foreach (XmlElement element in elements)
        {
          switch (element.Name.ToLower())
          {
            case "add":
              string type = element.GetAttribute("type");
              if (string.IsNullOrEmpty(type))
              {
                continue;
              }

              string method = element.GetAttribute("method");
              if (string.IsNullOrEmpty(method))
              {
                continue;
              }

              custom.Add(new[]
              {
                type, method
              });
              break;
            case "clear":
              custom.Clear();
              break;
          }
        }

        bool customExists = custom.Count > 0;
        if (customExists)
        {
          string value = custom.Aggregate(string.Empty, (current, pair) => current + (";" + pair[0] + "-" + pair[1]));
          string postInstallUrl = GetUrl(instance, AgentFiles.PostInstallActionsFileName, value.TrimStart(';'), "custom");
          ExecuteAgent(AgentFiles.StatusFileName, statusUrl, AgentFiles.PostInstallActionsFileName, postInstallUrl, ActionsPerforming, ActionsPerformed);
          return;
        }
      }

      if (skipPostActions)
      {
        Log.Info("PostActions are skipped");
        return;
      }

      string fileName = Path.GetFileName(module.PackagePath);
      Assert.IsNotNull(fileName, "name");
      string url = GetUrl(instance, AgentFiles.PostInstallActionsFileName, fileName);
      ExecuteAgent(AgentFiles.StatusFileName, statusUrl, AgentFiles.PostInstallActionsFileName, url, ActionsPerforming, ActionsPerformed);
    }

    #endregion

    #region Agent communication layer

    #region Public methods

    public static string GetUrl(Instance instance, string pageName, string value = null, string key = null)
    {
      return instance.GetUrl(AgentPath + '/' + pageName) + (!string.IsNullOrEmpty(value) ? "?" + (key ?? "fileName") + "=" + HttpUtility.UrlEncode(value) : string.Empty);
    }

    [NotNull]
    public static string Request([NotNull] string url, [NotNull] string pageName)
    {
      Assert.ArgumentNotNull(url, "url");
      Assert.ArgumentNotNullOrEmpty(pageName, "pageName");

      string result;
      string errorPrefix = pageName + " returned an error: ";
      try
      {
        Log.Info("Requesting URL {0}", url);
        result = WebRequestHelper.DownloadString(url).Trim();
        if (result.ToLower().StartsWith("error:"))
        {
          throw new InvalidOperationException(errorPrefix + result);
        }
      }
      catch (WebException ex)
      {
        bool error500 = ex.Status == WebExceptionStatus.ProtocolError && ex.Message.Contains("(500)");
        Assert.IsTrue(!error500, errorPrefix + ex);
        result = TimedOut;
      }


      Log.Info("Install status: {0}", result);
      return result;
    }

    #endregion

    #region Private methods

    private static void ExecuteAgent(string statusFileName, string statusUrl, string agentName, string operationUrl, string operationStartedStatus, string operationCompletedStatus)
    {
      // Call agent main operation
      string status = Request(operationUrl, agentName);

      // If the package installation process takes more than http timeout, retrive status
      if (status != operationCompletedStatus)
      {
        // Retrive status while the previous request timed out, status is in progress or package is already being installed
        while (status == TimedOut || status == InProgress || status == operationStartedStatus)
        {
          status = Request(statusUrl, statusFileName);
          Thread.Sleep(2000);
        }

        // Break the process if something went wrong and the package is not installed
        Assert.IsTrue(status == operationCompletedStatus, status);
      }
    }

    #endregion

    #endregion

    #region Public methods

    public static void ResetStatus(Instance instance)
    {
      var simStatusFilePath = Path.Combine(instance.TempFolderPath, "sim.status");
      FileSystem.FileSystem.Local.Directory.DeleteIfExists(simStatusFilePath);
    }

    #endregion
  }
}