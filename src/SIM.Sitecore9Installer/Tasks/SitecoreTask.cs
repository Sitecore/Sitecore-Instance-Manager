using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIM.Sitecore9Installer.Tasks
{
  public class SitecoreTask : PowerShellTask
  {
    public SitecoreTask(string taskName, int executionOrder, GlobalParameters globalParams,
      LocalParameters localParams, Dictionary<string, string> taskOptions)
      : base(taskName, executionOrder, globalParams, localParams, taskOptions)
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

      script.AppendFormat("Import-Module SitecoreInstallFramework{0} -ErrorAction Stop", importParam).AppendLine();

      //script.Append(this.ParamsHandler.GetGlobalParamsScript(this.GlobalParams,true));

      //script.AppendLine("$installParams=@{");
      string installParams = GetLocalParamsScript();

      //script.Append(installParams);
      script.Append(installParams);
      script.AppendFormat("cd \"{0}\"", Path.GetDirectoryName(this.LocalParams.First(p => p.Name == "Path").Value)).AppendLine();

      // script.AppendLine(script.ToString());
      string log = !sifVersion.StartsWith("1") ? string.Format("*>&1 | Tee-Object {0}.log", this.Name) : string.Empty;
      script.AppendFormat("{0} @installParams {1} -Verbose", this.UnInstall ? "Uninstall-SitecoreConfiguration" : "Install-SitecoreConfiguration", log).AppendLine();
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
        installParams.AppendFormat("'{0}'={1}", param.Name, value).AppendLine();

      }

      installParams.AppendLine("}");
      return installParams.ToString();
    }
  }
}
