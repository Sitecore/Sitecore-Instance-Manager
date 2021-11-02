using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SIM.ContainerInstaller;
using SIM.Instances;
using SIM.SitecoreEnvironments;

namespace SIM.Tool.Base.Tools.License
{
  internal class DockerComposeLicenseUpdater : LicenseUpdaterBase
  {
    protected override string Name => "DockerComposeLicenseUpdater";

    public DockerComposeLicenseUpdater(ILogger logger) : base(logger) { }

    protected override bool CanHandle(Instance instance)
    {
      if (instance == null)
      {
        return true;
      }

      SitecoreEnvironment environment = SitecoreEnvironmentHelper.GetExistingSitecoreEnvironment(instance.Name);

      if (environment != null && environment.EnvType == SitecoreEnvironment.EnvironmentType.Container)
      {
        return true;
      }

      return false;
    }

    private string EnvFileName = ".env";

    protected override bool DoUpdate(string licenseFilePath, UpdateMode updateMode = UpdateMode.UpdateAllInstances, Instance instance = null)
    {
      IEnumerable<string> envFilesToUpdate = GetEnvFilesToUpdate(updateMode, instance);

      UpdateEnvFilesWithNewLicense(licenseFilePath, envFilesToUpdate);

      return true;
    }

    private void UpdateEnvFilesWithNewLicense(string licenseFilePath, IEnumerable<string> envFilesToUpdate)
    {
      foreach (var envFilePath in envFilesToUpdate)
      {
        try
        {
          UpdateEnvFile(licenseFilePath, envFilePath);
        }
        catch (Exception ex)
        {
          _Logger.Log(LogLevel.Error, ex, $"{Name}: could not update the license in: '{envFilePath}'.");
        }
      }
    }

    private void UpdateEnvFile(string licenseFilePath, string envFilePath)
    {
      string licenseValue = new SitecoreLicenseCoverter().Convert(licenseFilePath);

      if (string.IsNullOrEmpty(licenseValue))
      {
        _Logger.Log(LogLevel.Error, $"{Name}: license could not be converted.");

        return;
      }

      EnvModel model = EnvModel.LoadFromFile(envFilePath);

      model.SitecoreLicense = licenseValue;

      model.SaveToFile(envFilePath);
    }

    [NotNull]
    private IEnumerable<string> GetEnvFilesToUpdate(UpdateMode updateMode, Instance instance)
    {
      IEnumerable<SitecoreEnvironment> environmentsToUpdate = new List<SitecoreEnvironment>();

      if (updateMode == UpdateMode.UpdateAllInstances)
      {
        environmentsToUpdate = GetAllContainerizedEnvironments();
      }

      if (updateMode == UpdateMode.UpdateSingleInstance)
      {
        environmentsToUpdate = GetSingleContainerizedEnvironment(instance);
      }

      IEnumerable<string> envFilesToUpdate = environmentsToUpdate.Select(e => GetEnvFile(e)).Where(p => !string.IsNullOrEmpty(p));

      return envFilesToUpdate;
    }

    [NotNull]
    private IEnumerable<SitecoreEnvironment> GetSingleContainerizedEnvironment(Instance instance)
    {
      SitecoreEnvironment environment = SitecoreEnvironmentHelper.GetExistingSitecoreEnvironment(instance.Name);

      if (environment == null)
      {
        // Sitecore 9 and later must be a part of an environment
        _Logger.Log(LogLevel.Error, $"{Name}: could not resolve the environment for instance: {instance.Name}");

        yield break;
      }

      yield return environment;
    }

    [NotNull]
    private IEnumerable<SitecoreEnvironment> GetAllContainerizedEnvironments()
    {
      return SitecoreEnvironmentHelper.SitecoreEnvironments.Where(e =>
        e.EnvType == SitecoreEnvironment.EnvironmentType.Container);
    }

    [CanBeNull]
    private string GetEnvFile(SitecoreEnvironment environment)
    {
      string pathToEnvironmentFolder = environment.UnInstallDataPath;

      if (!Directory.Exists(pathToEnvironmentFolder))
      {
        _Logger.Log(LogLevel.Error, $"{Name}: could not resolve the root folder for environment: {environment.Name}");

        return null;
      }

      string envFilePath = Path.Combine(pathToEnvironmentFolder, EnvFileName);

      if (File.Exists(envFilePath))
      {
        return envFilePath;
      }

      _Logger.Log(LogLevel.Error, $"{Name}: '.env' file could not be found for environment '{environment.Name}'.");

      return null;
    }
  }
}