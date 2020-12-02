using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ContainerInstaller
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
