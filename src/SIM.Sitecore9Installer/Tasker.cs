using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIM.Products;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class Tasker
  {

    List<PowerShellTask> tasksToRun = new List<PowerShellTask>();
    List<InstallParam> mapping;
    string filesRoot;
    string globalParamsFile;
    string deployRoot;
    bool unInstall;
    JObject doc;
    List<InstallParam> globalParams;
    JObject settingsDoc;

    //public static FileInfo ResolveGlobalFile(string packagePath)
    //{
    //  string packageName = Path.GetFileNameWithoutExtension(packagePath);
    //  string globalSettings = string.Empty;
    //  using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalSettings.json")))
    //  {
    //    globalSettings = reader.ReadToEnd();
    //  }

    //  JObject settingsDoc = JObject.Parse(globalSettings);
    //  Dictionary<string, string> globalFilesMap = settingsDoc["GlobalFilesMap"].ToObject<Dictionary<string, string>>();
    //  foreach (string pattern in globalFilesMap.Keys)
    //  {
    //    if (Regex.IsMatch(packageName, pattern))
    //    {
    //      return new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), globalFilesMap[pattern]));
    //    }
    //  }

    //  return null;
    //}

    public List<InstallParam> GlobalParams
    {
      get
      {
        return this.globalParams;
      }
    }

    public List<PowerShellTask> Tasks
    {
      get
      {
        return this.tasksToRun;
      }
    }

    public bool UnInstall
    {
      get
      {
        return this.unInstall;
      }
      set
      {
        this.unInstall = value;
        foreach (var task in this.Tasks)
        {
          task.UnInstall = value;
        }
      }
    }

    public string UnInstallParamsPath { get; }

    private Tasker()
    {
      string globalSettings = string.Empty;
      using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig/GlobalSettings.json")))
      {
        globalSettings = reader.ReadToEnd();
      }

      this.settingsDoc = JObject.Parse(globalSettings);
    }

    public Tasker(string root, string installationPackageName, string deployRoot, bool unInstall = false) : this()
    {
      this.deployRoot = deployRoot;
      this.filesRoot = root;
      Dictionary<string, string> globalFilesMap = this.settingsDoc["GlobalFilesMap"].ToObject<Dictionary<string, string>>();
      foreach (string pattern in globalFilesMap.Keys)
      {
        if (Regex.IsMatch(installationPackageName, pattern))
        {
          string path = Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig");
          string fileName= Directory.GetFiles(path, globalFilesMap[pattern], SearchOption.AllDirectories).Single();
          this.globalParamsFile =fileName;
        }
      }

      this.mapping = this.GetPackageMapping();
      this.doc = JObject.Parse(File.ReadAllText(this.globalParamsFile));
      this.globalParams = this.GetGlobalParams();
      this.UnInstall = unInstall;
      this.AddInstallSifTask();
      this.LoadTasks();
    }

    public Tasker(string unInstallParamsPath) : this()
    {
      this.UnInstall = true;
      this.UnInstallParamsPath = unInstallParamsPath;
      List<InstallParam> deserializedGlobalParams;
      using (StreamReader reader = new StreamReader(Path.Combine(unInstallParamsPath, "globals.json")))
      {
        string filePath=Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig"), reader.ReadLine(), SearchOption.AllDirectories).Single();
        this.globalParamsFile = filePath;
        string data = reader.ReadToEnd();
        deserializedGlobalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
      }

      this.filesRoot = deserializedGlobalParams.Single(p => p.Name == "FilesRoot").Value;
      this.doc = JObject.Parse(File.ReadAllText(this.globalParamsFile));
      this.mapping = this.GetPackageMapping();
      this.globalParams = this.GetGlobalParams();
      for (int i = 0; i < this.globalParams.Count; ++i)
      {
        InstallParam param = deserializedGlobalParams.FirstOrDefault(p => p.Name == this.globalParams[i].Name);
        if (param != null)
        {
          param.ParamValueUpdated += this.GlobalParamValueUpdated;
          this.globalParams[i] = param;
        }
      }

      List<string> uninstallTasksNames = Directory.GetFiles(unInstallParamsPath, "*.json")
        .Where(name => !Path.GetFileName(name).Equals("globals.json", StringComparison.InvariantCultureIgnoreCase))
        .ToList();
      foreach (string paramsFile in uninstallTasksNames)
      {
        string taskNameAdnOrder = Path.GetFileNameWithoutExtension(paramsFile);
        int index = taskNameAdnOrder.LastIndexOf('_');
        string taskName = taskNameAdnOrder.Substring(0, index);
        int order = int.Parse(taskNameAdnOrder.Substring(index + 1));
        SitecoreTask t = new SitecoreTask(taskName, order, this);
        t.GlobalParams = this.globalParams;
        using (StreamReader reader = new StreamReader(paramsFile))
        {
          JProperty param = doc["ExecSequense"].Children().Cast<JProperty>().Single(p => p.Name == taskName);
          var overridden = param.Value["Parameters"];
          string realName = param.Name;
          if (overridden != null && overridden["RealName"] != null)
          {
            realName = overridden["RealName"]?.ToString();
          }
          t.LocalParams = this.GetTaskParameters(realName);

          string data = reader.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(data))
          {
            List<InstallParam> deserializedLocalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
            for (int i = 0; i < t.LocalParams.Count; ++i)
            {
              InstallParam lParam = deserializedLocalParams.FirstOrDefault(p => p.Name == t.LocalParams[i].Name);
              if (lParam != null)
              {
                t.LocalParams[i] = lParam;
              }
            }
          }

          t.UnInstall = true;
          this.tasksToRun.Add(t);
        }
      }
      AddInstallSifTask();
      this.tasksToRun = this.tasksToRun.OrderBy(t => t.ExecutionOrder).ToList();
      foreach (InstallParam p in this.globalParams)
      {
        p.ParamValueUpdated += this.GlobalParamValueUpdated;
      }
    }

    private void AddInstallSifTask()
    {
      string sifVersion = this.globalParams.First(p => p.Name == "SIFVersion").Value;
      var installSifTask = new InstallSIFTask(sifVersion, "https://sitecore.myget.org/F/sc-powershell/api/v2");
      this.tasksToRun.Add(installSifTask);
    }

    private List<InstallParam> GetGlobalParams()
    {
      List<InstallParam> list = new List<InstallParam>();
      list.Add(new InstallParam("FilesRoot", this.filesRoot));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        InstallParam p = new InstallParam(param.Name, param.Value.ToString());
        if (p.Name == "LicenseFile" && !string.IsNullOrWhiteSpace(this.filesRoot))
        {
          string license = Path.Combine(this.filesRoot, "license.xml");
          if (File.Exists(license))
          {
            p.Value = license;
          }
        }

        p.ParamValueUpdated += GlobalParamValueUpdated;
        list.Add(p);
      }

      return list;
    }

    private void RegisterGlobalParam(InstallParam parameter)
    {
      InstallParam param = this.GlobalParams.FirstOrDefault(p => p.Name == parameter.Name);
      if (param == null)
      {
        this.globalParams.Insert(0, parameter);
        param = parameter;
        param.ParamValueUpdated += this.GlobalParamValueUpdated;
      }

      if (param.Value != parameter.Value)
      {
        param.Value = parameter.Value;
      }
    }

    private void GlobalParamValueUpdated(object sender, ParamValueUpdatedArgs e)
    {
      InstallParam updatedParam = (InstallParam)sender;
      foreach (PowerShellTask task in this.Tasks)
      {
        InstallParam param = task.LocalParams.FirstOrDefault(p => p.Name == updatedParam.Name);
        if (param != null)
        {
          param.Value = updatedParam.Value;
        }
      }
    }

    public string GetGlobalParamsScript(bool addPrefix = true)
    {
      return GetParamsScript(this.GlobalParams, addPrefix);
    }

    public string GetParamsScript(List<InstallParam> installParams, bool addPrefix = true)
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

    public string GetParamsScript(List<InstallParam> installParams)
    {
      StringBuilder script = new StringBuilder();
      foreach (InstallParam param in installParams)
      {
        if (param.Value != null)
        {
          string value = param.GetParameterValue();
          script.AppendLine(string.Format("${0}={1}", param.Name, value));
        }
      }

      return script.ToString();
    }

    public void EvaluateGlobalParams()
    {
      string sifVersion = this.GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      string importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      }
      StringBuilder globalParamsEval = new StringBuilder();
      globalParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      globalParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", importParam);
      globalParamsEval.AppendLine("$GlobalParams =@{");
      globalParamsEval.Append(this.GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n");
      globalParamsEval.AppendLine("$GlobalParamsSys =@{");
      globalParamsEval.Append(this.GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n$GlobalParamsSys");
      //string globalParamsEval = string.Format("Import-Module SitecoreInstallFramework{0}\n$GlobalParams =@{{1}}$GlobalParams", importParam, this.tasksToRun.First().GetGlobalParamsScript());
      Hashtable evaluatedParams = null;
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(globalParamsEval.ToString());
        var res = PowerShellInstance.Invoke();
        evaluatedParams = (Hashtable)res.First().ImmediateBaseObject;
      }

      StringBuilder evaluatedOaramsScript = new StringBuilder();
      foreach (var param in this.GlobalParams)
      {
        if (evaluatedParams[param.Name] == null)
        {
          continue;
        }

        param.Value = (string)evaluatedParams[param.Name];
      }
    }

    public Hashtable EvaluateLocalParams(PowerShellTask powerShellTask, List<InstallParam> globalParams)
    {
      string sifVersion = powerShellTask.GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      string importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      }
      StringBuilder localParamsEval = new StringBuilder();
      localParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      localParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", importParam);
      localParamsEval.AppendFormat(GetParamsScript(globalParams));
      localParamsEval.AppendLine("$LocalParams =@{");
      localParamsEval.Append(GetParamsScript(powerShellTask.LocalParams, false));
      localParamsEval.Append("}\n");
      localParamsEval.AppendLine("$LocalParamsSys =@{");
      localParamsEval.Append(GetParamsScript(powerShellTask.LocalParams, false));
      localParamsEval.Append("}\n$LocalParamsSys");
      Hashtable evaluatedParams = null;
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(localParamsEval.ToString());
        var res = PowerShellInstance.Invoke();
        if (res != null && res.Count > 0)
        {
          evaluatedParams = (Hashtable)res.First().ImmediateBaseObject;
        }
      }

      return evaluatedParams;
    }

    public void SaveUninstallParams(string path)
    {
      string filesPath = Path.Combine(path, this.GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      Directory.CreateDirectory(filesPath);
      IEnumerable<string> excludeParams = this.settingsDoc["ExcludeUninstallParams"].Values<string>();
      using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, "globals.json")))
      {
        wr.WriteLine(Path.GetFileName(this.globalParamsFile));
        wr.WriteLine(this.GetSerializedGlobalParams(excludeParams));
      }
      foreach (PowerShellTask task in this.Tasks.Where(t => t.ShouldRun&&t.SupportsUninstall()))
      {
        string parameters = task.GetSerializedParameters(excludeParams);
        using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, string.Format("{0}_{1}.json", task.Name, task.ExecutionOrder))))
        {
          wr.Write(parameters);
        }
      }
    }

    public string GetSerializedGlobalParams()
    {
      return JsonConvert.SerializeObject(this.GlobalParams);
    }

    public string GetSerializedGlobalParams(IEnumerable<string> excludeList)
    {
      return JsonConvert.SerializeObject(this.GlobalParams.Where(p => !excludeList.Contains(p.Name)));
    }

    public string RunAllTasks()
    {
      StringBuilder results = new StringBuilder();
      this.EvaluateGlobalParams();
      foreach (SitecoreTask task in this.tasksToRun.Where(t => t.ShouldRun))
      {
        try
        {
          results.AppendLine(task.Run());
          if (task.State == TaskState.Failed)
          {
            break;
          }
        }
        catch (Exception ex)
        {
          results.AppendLine(ex.ToString());
          break;
        }
      }

      return results.ToString().Trim();
    }

    public void GenerateScripts()
    {
      string path = Path.Combine(this.filesRoot, "generated_scripts");
      Directory.CreateDirectory(path);
      foreach (SitecoreTask task in this.Tasks.Where(t => t.ShouldRun))
      {
        using (StreamWriter writer = new StreamWriter(Path.Combine(path, string.Format("{0}.ps1", task.Name))))
        {
          writer.Write(task.GetScript());
        }
      }
    }

    private List<InstallParam> GetTaskParameters(string name)
    {
      string file = Directory.GetFiles(this.filesRoot, string.Format("{0}.json", name), SearchOption.AllDirectories).First();

      List<InstallParam> installParams = new List<InstallParam>();
      JObject doc = JObject.Parse(File.ReadAllText(file));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        string dafultValue = param.Value["DefaultValue"]?.ToString();

        InstallParam p = new InstallParam(param.Name, dafultValue);
        p.Description = param.Value["Description"]?.ToString();
        if (this.globalParams.Any(g => g.Name == p.Name && !string.IsNullOrEmpty(g.Value)))
        {
          p.Value = this.globalParams.First(g => g.Name == p.Name).Value;
        }
        installParams.Add(p);

        if (p.Name == "Package")
        {
          InstallParam pack = mapping.FirstOrDefault(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
          if (!string.IsNullOrEmpty(pack?.Value))
          {
            p.Value = Directory.GetFiles(this.filesRoot, pack.Value).FirstOrDefault();
          }
        }
      }
      installParams.Add(new InstallParam("Path", file));
      return installParams;
    }

    private void LoadTasks()
    {
      int order = 0;
      foreach (JProperty param in doc["ExecSequense"].Children())
      {
        var overridden = param.Value["Parameters"];
        string realName = param.Name;
        if (overridden != null && overridden["RealName"] != null)
        {
          realName = overridden["RealName"]?.ToString();
        }

        string taskFile = Directory.GetFiles(this.filesRoot, string.Format("{0}.json", realName), SearchOption.AllDirectories).FirstOrDefault();
        if (string.IsNullOrEmpty(taskFile))
        {
          continue;
        }

        //if (!File.Exists(string.Format("{0}.json", Path.Combine(filesRoot, realName))))
        //{
        //  continue;
        //}

        SitecoreTask t = new SitecoreTask(param.Name, order, this);
        if (!string.IsNullOrEmpty(this.deployRoot))
        {
          this.InjectLocalDeploymentRoot(taskFile);
          this.InjectGlobalDeploymentRoot(this.deployRoot);
        }
        t.GlobalParams = this.GlobalParams;
        t.LocalParams = this.GetTaskParameters(realName);

        if (overridden != null)
        {
          foreach (JProperty newJParam in overridden.Children())
          {
            InstallParam newParam = t.LocalParams.FirstOrDefault(p => p.Name == newJParam.Name);
            if (newParam != null)
            {
              newParam.Value = newJParam.Value.ToString();
            }
          }
        }
        t.UnInstall = this.UnInstall;
        this.tasksToRun.Add(t);
        ++order;
      }
    }

    private void InjectLocalDeploymentRoot(string taskFilePath)
    {
      JObject doc = JObject.Parse(File.ReadAllText(taskFilePath));
      var node = doc["Variables"]["Site.PhysicalPath"];
      if (node != null)
      {
        JObject deployRoot = new JObject();
        deployRoot["Type"] = "string";
        deployRoot["Description"] = "The path to installtion root folder.";
        deployRoot["DefaultValue"] = "";
        doc["Parameters"]["DeployRoot"] = deployRoot;

        ((JValue)node).Value = "[joinpath(parameter('DeployRoot'), parameter('SiteName'))]";

        using (StreamWriter wr = new StreamWriter(taskFilePath))
        {
          wr.Write(doc.ToString());
        }
      }
    }

    private void InjectGlobalDeploymentRoot(string path)
    {
      InstallParam deployRoot = this.GlobalParams.FirstOrDefault(p => p.Name == "DeployRoot");
      if (deployRoot == null)
      {
        deployRoot = new InstallParam("DeployRoot", path);
        this.RegisterGlobalParam(deployRoot);
      }
      else
      {
        deployRoot.Value = path;
      }
    }

    private List<InstallParam> GetPackageMapping()
    {
      List<InstallParam> list = new List<InstallParam>();

      string file = this.globalParamsFile;
      JObject doc = JObject.Parse(File.ReadAllText(file));
      foreach (JProperty param in doc["PackageMapping"].Children())
      {
        InstallParam p = new InstallParam(param.Name, param.Value.ToString());
        list.Add(p);
      }

      return list;
    }

  }
}
