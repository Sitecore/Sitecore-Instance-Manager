using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer.Tasks
{
  public class SitecoreTask: SIM.Sitecore9Installer.Tasks.PowerShellTask
  {
    public SitecoreTask(string taskName, int executionOrder, GlobalParameters globalParams, 
      LocalParameters localParams, Dictionary<string, string> taskOptions) 
      :base(taskName, executionOrder, globalParams, localParams, taskOptions)
    {
    }

    protected override string GetScript()
    {
      StringBuilder script = new StringBuilder();
      script.AppendLine("Set-ExecutionPolicy Bypass -Force");

      string sifVersion = GetSifVersion(this.UnInstall, this.GlobalParams);

      string importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      }

      script.AppendLine(string.Format("Import-Module SitecoreInstallFramework{0} -ErrorAction Stop", importParam));

      //script.Append(this.ParamsHandler.GetGlobalParamsScript(this.GlobalParams,true));

      //script.AppendLine("$installParams=@{");
      string installParams = GetLocalParamsScript();

      //script.Append(installParams);
      script.Append(installParams);
      script.AppendLine(string.Format("cd \"{0}\"", Path.GetDirectoryName(this.LocalParams.First(p => p.Name == "Path").Value)));
      
      // script.AppendLine(script.ToString());
      string log = !sifVersion.StartsWith("1") ? string.Format("*>&1 | Tee-Object {0}.log", this.Name) : string.Empty;
      script.AppendLine(string.Format("{0} @installParams {1} -Verbose", this.UnInstall ? "Uninstall-SitecoreConfiguration" : "Install-SitecoreConfiguration", log));
      return script.ToString();
    }

    private string GetSifVersion(bool unInstall, BaseParameters globalParams)
    {
      string sifVersion = string.Empty;

      if (unInstall)
      {
        sifVersion = globalParams["SIFVersionUninstall"]?.Value ?? string.Empty;
      }

      if (!string.IsNullOrEmpty(sifVersion))
      {
        return sifVersion;
      }
      else
      {
        return globalParams["SIFVersion"]?.Value ?? string.Empty;
      }
    }

    private string GetLocalParamsScript()
    {
      StringBuilder installParams = new StringBuilder();
      installParams.AppendLine("$installParams=@{");
      foreach (InstallParam param in this.LocalParams)
      {
        if (string.IsNullOrEmpty(param.Value))
        {
          continue;
        }


        string value = param.GetParameterValue();
        installParams.AppendLine(string.Format("'{0}'={1}", param.Name, value));

      }

      installParams.AppendLine("}");
      return installParams.ToString();
    }
  }
}
