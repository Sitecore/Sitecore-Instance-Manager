using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Validation.Factory
{
  public static class InstallParamsHelper
  {
    public static string GetActualValue(string name, Dictionary<string, string> installParams)
    {
      if (installParams[name].StartsWith("$"))
      {
        var subName = installParams[name].TrimStart('$');
        if (!string.IsNullOrEmpty(installParams[subName]))
          return InstallParamsHelper.GetActualValue(subName, installParams);
      }

      return installParams[name];
    }
  }
}
