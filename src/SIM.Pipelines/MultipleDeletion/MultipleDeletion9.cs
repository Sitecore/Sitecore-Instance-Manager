using System;
using System.IO;
using System.Linq;
using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using SIM.SitecoreEnvironments;

namespace SIM.Pipelines.MultipleDeletion
{
  public class MultipleDeletion9 : MultipleDeletion9Processor
  {
    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      return ((MultipleDeletion9Args)args).Environments.Count;
    }

    protected override void Process(MultipleDeletion9Args args)
    {
      foreach (string environmentName in args.Environments)
      {
        args.Logger.Info($"Uninstall process has been started for '{environmentName}'.");

        string uninstallPath = this.GetUninstallPath(environmentName);
        if (string.IsNullOrEmpty(uninstallPath))
        {
          args.Logger.Error("Uninstall files have not been found.");
          return;
        }
        args.Logger.Info($"Uninstall files have been found in '{uninstallPath}'.");

        Tasker tasker = null;
        try
        {
          tasker = new Tasker(uninstallPath);
        }
        catch (Exception ex)
        {
          args.Logger.Error($"Tasks creation has failed with the following exception:\n{ex}");
          return;
        }
        this.InitializeTasksParams(tasker, args);

        try
        {
          tasker.RunLowlevelTasks();
        }
        catch(Exception ex)
        {
          args.Logger.Error($"Preparing for uninstall has failed with the following exception:\n{ex}");
          return;
        }
        args.Logger.Info("Preparing for uninstall has been finished successfully.");

        Install9Args delete9Args = new Install9Args(tasker);
        delete9Args.ScriptsOnly = args._ScriptsOnly;

        try
        {
          args.Logger.Info($"The pipeline to delete '{environmentName}' has been started.");
          PipelineManager.StartPipeline("delete9", delete9Args, null, false);
          args.Logger.Info($"The pipeline to delete '{environmentName}' has been finished successfully.");
        }
        catch (Exception ex)
        {
          args.Logger.Error($"The deletion pipeline has failed with the following exception:\n{ex}");
          return;
        }

        IncrementProgress();
      }
    }

    private string GetUninstallPath(string environmentName)
    {
      SitecoreEnvironment sitecoreEnvironment = SitecoreEnvironmentHelper.SitecoreEnvironments.Where(environment => environment.Name == environmentName).FirstOrDefault();
      if (!string.IsNullOrEmpty(sitecoreEnvironment?.UnInstallDataPath))
      {
        return sitecoreEnvironment.UnInstallDataPath;
      }
      else
      {
        foreach (string installName in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder).OrderByDescending(s => s.Length))
        {
          if (environmentName.Equals(Path.GetFileName(installName), StringComparison.InvariantCultureIgnoreCase))
          {
            return installName;
          }
        }
      }

      return null;
    }

    private void InitializeTasksParams(Tasker tasker, MultipleDeletion9Args args)
    {
      InstallParam sqlServer = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlServer");
      if (sqlServer != null)
      {
        sqlServer.Value = args._ConnectionString.DataSource;
      }

      InstallParam sqlAdminUser = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminUser");
      if (sqlAdminUser != null)
      {
        sqlAdminUser.Value = args._ConnectionString.UserID;
      }

      InstallParam sqlAdminPass = tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlAdminPassword");
      if (sqlAdminPass != null)
      {
        sqlAdminPass.Value = args._ConnectionString.Password;
      }
    }
  }
}