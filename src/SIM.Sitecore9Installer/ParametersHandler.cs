using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class ParametersHandler : IParametersHandler
  {
    public List<InstallParam> GetGlobalParams(JObject globalParamsDoc, string filesRoot)
    {
      List<InstallParam> globalParams = new List<InstallParam>();
      globalParams.Add(new InstallParam("FilesRoot", filesRoot));
      foreach (JProperty param in globalParamsDoc["Parameters"].Children())
      {
        InstallParam p = new InstallParam(param.Name, param.Value.ToString(),true);
        if (p.Name == "LicenseFile" && !string.IsNullOrWhiteSpace(filesRoot))
        {
          string license = Path.Combine(filesRoot, "license.xml");
          if (File.Exists(license)) p.Value = license;
        }

        globalParams.Add(p);
      }

      return globalParams;
    }
    public void AddOrUpdateParam(List<InstallParam> globalParams, string name, string value, string type="string")
    {
      InstallParam param = globalParams.FirstOrDefault(p => p.Name == name);
      if (param == null)
      {
        param = new InstallParam(name, value, true, type);
        globalParams.Insert(0, param);
      }

      if (param.Value != value) param.Value = value;
    }
    public void EvaluateGlobalParams(List<InstallParam> globalParams)
    {
      StringBuilder globalParamsEval = new StringBuilder();
      globalParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      globalParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", this.GetSifVersion(globalParams));
      globalParamsEval.AppendLine("$GlobalParams =@{");
      globalParamsEval.Append(GetGlobalParamsScript(globalParams,false));
      globalParamsEval.Append("}\n");
      globalParamsEval.AppendLine("$GlobalParamsSys =@{");
      globalParamsEval.Append(GetGlobalParamsScript(globalParams,false));
      globalParamsEval.Append("}\n$GlobalParamsSys");
      Hashtable evaluatedParams = this.GetEvaluatedParams(globalParamsEval.ToString());

      foreach (var param in globalParams)
      {
        if (evaluatedParams[param.Name] == null || param.Value == evaluatedParams[param.Name].ToString()) continue;

        param.Value = (string)evaluatedParams[param.Name];
      }
    }
    public string GetGlobalParamsScript(List<InstallParam> globalParams, bool addPrefix = true)
    {
      return GetParamsScript(globalParams, addPrefix);
    }
    public void EvaluateLocalParams(List<InstallParam> localParams, List<InstallParam> globalParams)
    {
      Hashtable evaluatedParams = this.GetEvaluatedLocalParams(localParams, globalParams);
      foreach (InstallParam param in localParams)
      {
        if (evaluatedParams[param.Name] == null || param.Value == evaluatedParams[param.Name].ToString())
        {
          continue;
        }

        param.Value = (string)evaluatedParams[param.Name];
      }
    }

    private string GetParamsScript(List<InstallParam> installParams, bool addPrefix = true)
    {
      StringBuilder script = new StringBuilder();
      foreach (InstallParam param in installParams)
      {
        if (param.Value != null)
        {
          string value = param.GetParameterValue();
          string paramName = addPrefix ? "{" + param.Name + "}" : string.Format("'{0}'", param.Name);
          script.AppendLine(string.Format("{0}{1}={2}", addPrefix ? "$" : string.Empty, paramName, value));
        }
      }

      return script.ToString();
    }
    private string GetSifVersion(List<InstallParam> globalParams)
    {
      string sifVersion = globalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        return string.Format(" -RequiredVersion {0}", sifVersion);
      }
      return string.Empty;
    }
    private Hashtable GetEvaluatedLocalParams(List<InstallParam> localParams, List<InstallParam> globalParams)
    {
      StringBuilder localParamsEval = new StringBuilder();
      localParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      localParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", GetSifVersion(globalParams));
      localParamsEval.Append(this.GetParamsScript(globalParams));
      localParamsEval.AppendLine("$installParams =@{");
      localParamsEval.Append(this.GetParamsScript(localParams, false));
      localParamsEval.Append("}\n");
      localParamsEval.AppendLine("$installParamsSys =@{");
      localParamsEval.Append(this.GetParamsScript(localParams, false));
      localParamsEval.Append("}\n$installParamsSys");

      return this.GetEvaluatedParams(localParamsEval.ToString());
    }
    private Hashtable GetEvaluatedParams(string script)
    {
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(script);
        var res = PowerShellInstance.Invoke();
        if (res != null && res.Count > 0)
        {
          return (Hashtable)res.First().ImmediateBaseObject;
        }
      }

      return null;
    }
  }
}
