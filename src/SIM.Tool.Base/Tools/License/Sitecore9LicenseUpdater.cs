using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using SIM.Instances;
using SIM.SitecoreEnvironments;

namespace SIM.Tool.Base.Tools.License
{
  internal class Sitecore9LicenseUpdater : LicenseUpdaterOnPremBase
  {
    public Sitecore9LicenseUpdater(ILogger logger) : base(logger)
    {
    }

    protected override string Name => "Sitecore9LicenseUpdater";

    protected override bool CanHandle(Instance instance)
    {
      if (instance == null)
      {
        return true;
      }

      if (instance.Type == Instance.InstanceType.Sitecore9AndLater &&
          instance.Type != Instance.InstanceType.SitecoreContainer)
      {
        return true;
      }

      return false;
    }

    protected override bool DoUpdate(string newLicenseFilePath, UpdateMode updateMode = UpdateMode.UpdateAllInstances,
      Instance instance = null)
    {
      IEnumerable<string> filesToUpdate = GetLicenseFilesToUpdate(updateMode, instance);

      foreach (var licenseToUpdate in filesToUpdate)
      {
        this.UpdateLicenseFile(licenseToUpdate, newLicenseFilePath);
      }

      return true;
    }

    private IEnumerable<string> GetLicenseFilesToUpdate(UpdateMode updateMode, Instance instance)
    {
      IEnumerable<SitecoreEnvironment> environmentsToUpdate = new List<SitecoreEnvironment>();

      if (updateMode == UpdateMode.UpdateSingleInstance)
      {
        environmentsToUpdate = GetSingleEnvironment(instance);
      }

      if (updateMode == UpdateMode.UpdateAllInstances)
      {
        environmentsToUpdate = GetAllEnvironments();
      }

      IEnumerable<string> licenseFilesToUpdate = environmentsToUpdate.SelectMany(env => GetEnvironmentLicensesPaths(env));

      return licenseFilesToUpdate;
    }

    [NotNull]
    private IEnumerable<SitecoreEnvironment> GetAllEnvironments()
    {
      return SitecoreEnvironmentHelper.SitecoreEnvironments;
    }

    [NotNull]
    private IEnumerable<SitecoreEnvironment> GetSingleEnvironment(Instance instance)
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

    [CanBeNull]
    private string GetEnvironmentRootFolderPath(SitecoreEnvironment environment)
    {
      // "SitecoreEnvironment" does not contain path to the root folder, therefore we use a fallback logic to construct path to the environment root folder.

      string defaultWwwRoot = "C:\\inetpub\\wwwroot";

      string pathToEnvironmentFolder = Path.Combine(defaultWwwRoot, environment.Name);

      if (!Directory.Exists(pathToEnvironmentFolder))
      {
        _Logger.Log(LogLevel.Warning, $"{Name}: could not resolve the root folder for environment:" + environment.Name);

        return null;
      }

      return pathToEnvironmentFolder;
    }

    [NotNull]
    private IEnumerable<string> GetEnvironmentLicensesPaths(SitecoreEnvironment environment)
    {
      string environmentRootFolder = GetEnvironmentRootFolderPath(environment);

      if (string.IsNullOrEmpty(environmentRootFolder))
      {
        yield break;
      }

      IEnumerable<string> existingLicensesPaths;

      try
      {
        existingLicensesPaths = Directory.GetFiles(environmentRootFolder, LicenseFileName, SearchOption.AllDirectories).Where(p => !string.IsNullOrEmpty(p));
      }
      catch (Exception ex)
      {
        _Logger.Log(LogLevel.Error, $"{Name}: could not resolve licenses paths for environment: {environment.Name}\r\n{ex.StackTrace}");

        yield break;
      }

      foreach (var licensePath in existingLicensesPaths)
      {
        yield return licensePath;
      }
    }
  }
}

