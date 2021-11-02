using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public virtual string SuccessMessage => "Hard disk has enough free space to continue the installation.";

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)
    {
      IEnumerable<InstallParam> deployRoots = tasks.Select(t => t.LocalParams.FirstOrDefault(l => l.Name == this.Data["DeployRoot"]))
        .Where(p => p != null);
      IEnumerable<string> uniqueDeployRoots = deployRoots.Select(x => x.Value).Distinct();
      IEnumerable<string> uniqueDrives = uniqueDeployRoots.Select(d => Path.GetPathRoot(d)).Distinct();
      bool errors = false;
      long hardDriveWarningLimit = long.Parse(this.Data["HardDriveWarningLimit"]);
      long hardDriveErrorLimit = long.Parse(this.Data["HardDriveErrorLimit"]); ;

      foreach (string drive in uniqueDrives)
      {
        long freeSpace = GetHardDriveFreeSpace(drive);
        if (freeSpace == -1)
        {
          errors = true;
          yield return new ValidationResult(ValidatorState.Error, $"Hard disk '{drive}' has not been found.", null);
        }
        else
         if (freeSpace < hardDriveErrorLimit)
        {
          errors = true;
          yield return new ValidationResult(ValidatorState.Error, $"Hard disk '{drive}' does not have enough free space to continue the installation.", null);
        }
        else
         if (freeSpace < hardDriveWarningLimit)
        {
          errors = true;
          yield return new ValidationResult(ValidatorState.Warning, $"Hard disk '{drive}' has a little free space.", null);
        }
      }

      if (!errors)
      {
        yield return new ValidationResult(ValidatorState.Success, this.SuccessMessage, null);
      }
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
