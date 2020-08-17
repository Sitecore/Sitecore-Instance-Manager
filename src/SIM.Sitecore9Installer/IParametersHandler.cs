using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace SIM.Sitecore9Installer
{
  public interface IParametersHandler
  {
    void AddOrUpdateParam(List<InstallParam> globalParams, string name, string value, string type = "string");
    void EvaluateGlobalParams(List<InstallParam> globalParams);
    void EvaluateLocalParams(List<InstallParam> localParams, List<InstallParam> globalParams);
    List<InstallParam> GetGlobalParams(JObject globalParamsDoc, string filesRoot);
    string GetGlobalParamsScript(List<InstallParam> globalParams, bool addPrefix = true);
  }
}