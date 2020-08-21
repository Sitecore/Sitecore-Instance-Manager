using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class LocalParameters:BaseParameters
  {
    private List<InstallParam> _localParams;
    private GlobalParameters _globalParams;
    private bool _evaluated;

    public LocalParameters(List<InstallParam> loaclParameters, GlobalParameters globalParameters)
    {
      this._localParams = loaclParameters;
      this._globalParams = globalParameters;
    }

    protected override List<InstallParam> Parameters { get => _localParams; }

    public override void Evaluate()    
    {
      this._globalParams.Evaluate();
      if (!this._evaluated)
      {
        this.EvaluateLocalParams();
        this._evaluated = true;
      }
    }

    protected override InstallParam CreateParameter(string name, string value, InstallParamType type)
    {
      return new InstallParam(name, value, false, type);
    }

    private void EvaluateLocalParams()
    {
      Hashtable evaluatedParams = this.GetEvaluatedLocalParams();
      foreach (InstallParam param in this.Parameters)
      {
        if (evaluatedParams[param.Name] == null || param.Value == evaluatedParams[param.Name].ToString())
        {
          continue;
        }

        param.Value = (string)evaluatedParams[param.Name];
      }
    }

    private Hashtable GetEvaluatedLocalParams()
    {
      StringBuilder localParamsEval = new StringBuilder();
      localParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      localParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", GetSifVersion());
      localParamsEval.Append(this._globalParams.GetGlobalParamsScript());
      localParamsEval.AppendLine("$installParams =@{");
      string paramsScript = this.GetParamsScript(false);
      localParamsEval.Append(paramsScript);
      localParamsEval.Append("}\n");
      localParamsEval.AppendLine("$installParamsSys =@{");
      localParamsEval.Append(paramsScript);
      localParamsEval.Append("}\n$installParamsSys");

      return this.GetEvaluatedParams(localParamsEval.ToString());
    }

    private string GetSifVersion()
    {
      string sifVersion = this._globalParams["SIFVersion"]?.Value ?? string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        return string.Format(" -RequiredVersion {0}", sifVersion);
      }
      return string.Empty;
    }
  }
}
