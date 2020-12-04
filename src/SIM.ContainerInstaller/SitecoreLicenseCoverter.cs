using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;

namespace SIM.ContainerInstaller
{
  public class SitecoreLicenseCoverter:ISitecoreLicenseConverter
  {
    public string Convert(string licenseFilePath)
    {
      string scriptfile = Path.Combine(Directory.GetCurrentDirectory(), "ContainerFiles/scripts/LicenseScript.txt");
      PSFileExecutor ps = new PSFileExecutor(scriptfile,Path.GetDirectoryName(licenseFilePath));
      Collection<PSObject> results= ps.Execute();
      return results[0].ToString();
    }    
  }
}
