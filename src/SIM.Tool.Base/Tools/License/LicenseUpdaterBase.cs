using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SIM.Instances;

namespace SIM.Tool.Base.Tools.License
{
  internal abstract class LicenseUpdaterBase
  {
    protected ILogger _Logger;

    protected abstract string Name { get; }

    protected const string LicenseFileName = "license.xml";

    protected LicenseUpdaterBase(ILogger logger)
    {
      _Logger = logger;
    }

    protected abstract bool CanHandle(Instance instance);

    /// <summary>
    /// The method returns 'true' if licenses can be updated
    /// </summary>
    /// <param name="licenseFilePath"></param>
    public bool Update(string licenseFilePath)
    {
      return this.Update(licenseFilePath, UpdateMode.UpdateAllInstances);
    }

    /// <summary>
    /// The method returns 'true' if licenses can be updated
    /// </summary>
    /// <param name="licenseFilePath"></param>
    /// <param name="updateMode"></param>
    public bool Update(string licenseFilePath, UpdateMode updateMode)
    {
      return this.Update(licenseFilePath, updateMode, null);
    }

    /// <summary>
    /// The method returns 'true' if licenses can be updated
    /// </summary>
    /// <param name="licenseFilePath"></param>
    /// <param name="updateMode"></param>
    /// <param name="instance"></param>
    public bool Update(
      string licenseFilePath,
      UpdateMode updateMode,
      Instance instance
    )
    {
      bool argumentsAreValid = ValidateArguments(licenseFilePath, updateMode, instance);

      if (!argumentsAreValid)
      {
        return false;
      }

      if (CanHandle(instance))
      {
        return DoUpdate(licenseFilePath, updateMode, instance);
      }

      return false;
    }

    /// <summary>
    /// The method returns true if when licenses are updated successfully
    /// </summary>
    /// <param name="licenseFilePath"></param>
    /// <param name="updateMode"></param>
    /// <param name="instance"></param>
    /// <returns></returns>
    protected abstract bool DoUpdate(
      string licenseFilePath,
      UpdateMode updateMode = UpdateMode.UpdateAllInstances,
      Instance instance = null
      );

    /// <summary>
    /// The method performs the validation of the incoming parameters. If the validation is successful method returns 'true'.
    /// </summary>
    /// <param name="licenseFilePath">should point to the exiting <c>license.xml</c> file</param>
    /// <param name="updateMode">defines if one or all instances/environments should be processed</param>
    /// <param name="instance">contains the instance that should be processed. Can be null the if the value of 'UpdateMode' is 'UpdateAllInstances'</param>
    /// <returns></returns>
    protected bool ValidateArguments(string licenseFilePath,
      UpdateMode updateMode,
      Instance instance
      )
    {
      const bool Valid = true;
      const bool Invalid = false;

      if (updateMode == UpdateMode.UpdateSingleInstance && instance == null)
      {
        _Logger.Log(LogLevel.Error, $"{this.Name}: the update mode is 'UpdateSingleInstance', however the 'instance' was not provided.");

        return Invalid;
      }

      if (string.IsNullOrEmpty(licenseFilePath))
      {
        _Logger.Log(LogLevel.Error, $"{this.Name}: license file path is null or empty.");

        return Invalid;
      }

      if (!File.Exists(licenseFilePath))
      {
        _Logger.Log(LogLevel.Error, $"{this.Name}: license file path ({licenseFilePath}) points to non existing file.");

        return Invalid;
      }

      if (!Path.GetFileName(licenseFilePath).Equals(LicenseFileName, StringComparison.InvariantCultureIgnoreCase))
      {
        _Logger.Log(LogLevel.Error, $"{this.Name}: license file '{licenseFilePath}' path should not point '{LicenseFileName}' file.");

        return Invalid;
      }

      return Valid;
    }
  }
}
