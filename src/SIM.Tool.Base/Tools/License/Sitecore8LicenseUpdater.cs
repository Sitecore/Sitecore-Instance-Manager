using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using SIM.Instances;

namespace SIM.Tool.Base.Tools.License
{
  internal class Sitecore8LicenseUpdater : LicenseUpdaterOnPremBase
  {
    public Sitecore8LicenseUpdater(ILogger logger) : base(logger)
    {
    }

    protected override string Name => "Sitecore8LicenseUpdater";

    protected override bool CanHandle(Instance instance)
    {
      if (instance == null)
      {
        return true;
      }

      if (instance.Type == Instance.InstanceType.Sitecore8AndEarlier)
      {
        return true;
      }

      return false;
    }

    /// <summary>
    /// The method returns true if licenses can be updated by this Updater
    /// </summary>
    /// <param name="licenseFilePath"></param>
    /// <param name="updateMode"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    protected override bool DoUpdate(
      string licenseFilePath,
      UpdateMode updateMode = UpdateMode.UpdateAllInstances,
      Instance instance = null
    )
    {
      IEnumerable<string> filesToUpdate = GetLicenseFilesToUpdate(updateMode, instance);

      foreach (var fileToUpdate in filesToUpdate)
      {
        this.UpdateLicenseFile(fileToUpdate, licenseFilePath);
      }

      return true;
    }

    private IEnumerable<string> GetLicenseFilesToUpdate(UpdateMode updateMode, Instance instance)
    {
      IEnumerable<string> licensesToUpdate;

      if (instance != null)
      {
        yield return instance.LicencePath;
      }

      List<Instance> instances = InstanceManager.Default.Instances
            ?.Where(i => i.Type == Instance.InstanceType.Sitecore8AndEarlier).ToList();

      if (instances == null || instances.Count < 1)
      {
        _Logger.Log(LogLevel.Debug,
          $"{Name}: The 'InstanceManager' returned an empty list. License updater: there are no Sitecore 8 and earlier versions to update.");

        yield break;
      }

      licensesToUpdate = instances.Select(i => i.LicencePath);

      foreach (var path in licensesToUpdate)
      {
        if (!string.IsNullOrEmpty(path))
        {
          yield return path;
        }
      }
    }
  }
}
