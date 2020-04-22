using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SIM.Sitecore9Installer.Tasks;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class DiskSpaceValidator : IValidator
  {
    public DiskSpaceValidator()
    {
      this.Data = new Dictionary<string, string>();
    }
    public Dictionary<string, string> Data { get; set; }

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      IEnumerable<InstallParam> deployRoots = tasks.Select(t => t.LocalParams.FirstOrDefault(l => l.Name == this.Data["DeployRoot"]))
        .Where(p => p != null);
      IEnumerable<string> uniqueDeployRoots = deployRoots.Select(x => x.Value).Distinct();
      IEnumerable<string> uniqueDrives = uniqueDeployRoots.Select(d => Path.GetPathRoot(d)).Distinct();

      long hardDriveWarningLimit = long.Parse(this.Data["HardDriveWarningLimit"]);
      long hardDriveErrorLimit = long.Parse(this.Data["HardDriveErrorLimit"]); ;

      foreach (string drive in uniqueDrives)
      {
        long freeSpace = GetHardDriveFreeSpace(drive);
        if (freeSpace == -1)
        {
          yield return new ValidationResult(ValidatorState.Error, $"Hard disk '{drive}' has not been found.", null);
        }
        else
         if (freeSpace < hardDriveErrorLimit)
        {
          yield return new ValidationResult(ValidatorState.Error, $"Hard disk '{drive}' does not have enough free space to continue installation.", null);
        }
        else
         if (freeSpace < hardDriveWarningLimit)
        {
          yield return new ValidationResult(ValidatorState.Warning, $"Hard disk '{drive}' has a little free space.", null);
        }
      }

      yield return new ValidationResult(ValidatorState.Success, null, null);
    }

    protected internal virtual long GetHardDriveFreeSpace(string driveName)
    {
      foreach (DriveInfo drive in DriveInfo.GetDrives())
      {
        if (drive.IsReady && drive.Name == driveName)
        {
          return drive.TotalFreeSpace;
        }
      }
      return -1;
    }
  }
}
