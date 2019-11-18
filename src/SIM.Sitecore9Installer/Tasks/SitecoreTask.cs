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
    //TO DO It's still used by uninstallation task.
    public SitecoreTask(string name, int executionOrder, Tasker owner) : base(name, executionOrder, owner, new List<InstallParam>(), new Dictionary<string, string>())
    {
    }

    public SitecoreTask(string taskName, int executionOrder, Tasker owner, List<InstallParam> localParams, Dictionary<string, string> taskOptions) : base(taskName, executionOrder, owner, localParams, taskOptions)
    {
    }

    public override string GetScript()
    {
      StringBuilder script = new StringBuilder();


      script.Append(this.Owner.GetGlobalParamsScript());

      //script.AppendLine("$installParams=@{");
      string installParams = GetLocalParamsScript();

      script.Append(installParams);
      script.Append(installParams);
      script.AppendLine(string.Format("cd \"{0}\"", Path.GetDirectoryName(this.LocalParams.First(p => p.Name == "Path").Value)));
      script.AppendLine("Set-ExecutionPolicy Bypass -Force");

      string sifVersion = GetSifVersion(this.UnInstall, this.GlobalParams);

      string importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      }

      script.AppendLine(string.Format("Import-Module SitecoreInstallFramework{0} -ErrorAction Stop", importParam));
      // script.AppendLine(script.ToString());
      string log = !sifVersion.StartsWith("1") ? string.Format("*>&1 | Tee-Object {0}.log", this.Name) : string.Empty;
      script.AppendLine(string.Format("{0} @installParams {1} -Verbose", this.UnInstall ? "Uninstall-SitecoreConfiguration" : "Install-SitecoreConfiguration", log));
      return script.ToString();
    }

    private string GetSifVersion(bool unInstall, List<InstallParam> globalParams)
    {
      string sifVersion = string.Empty;

      if (unInstall)
      {
        sifVersion = globalParams.FirstOrDefault(p => p.Name == "SIFVersionUninstall")?.Value ?? string.Empty;
      }

      if (!string.IsNullOrEmpty(sifVersion))
      {
        return sifVersion;
      }
      else
      {
        return globalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
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

    public override bool SupportsUninstall()
    {
      return true;
    }
  }
}
