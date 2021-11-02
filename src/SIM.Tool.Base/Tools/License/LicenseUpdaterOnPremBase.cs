using Microsoft.Extensions.Logging;
using System;

namespace SIM.Tool.Base.Tools.License
{
  internal abstract class LicenseUpdaterOnPremBase : LicenseUpdaterBase
  {
    protected LicenseUpdaterOnPremBase(ILogger logger) : base(logger) { }

    protected bool UpdateLicenseFile(string oldLicenseFilePath, string newLicenseFilePath)
    {
      try
      {
        FileSystem.FileSystem.Local.File.Copy(newLicenseFilePath, oldLicenseFilePath, true);
      }
      catch (Exception ex)
      {
        _Logger.Log(LogLevel.Error, ex, $"License file '{oldLicenseFilePath}' could not be replaced with '{newLicenseFilePath}' due to exception in {Name}.");

        return false;
      }

      return true;
    }
  }
}
