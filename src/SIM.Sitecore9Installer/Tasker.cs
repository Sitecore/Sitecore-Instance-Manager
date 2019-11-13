using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer
{
  public class Tasker
  {
    private readonly string deployRoot;
    private readonly JObject doc;
    private readonly string globalParamsFile;
    private readonly List<InstallParam> mapping;
    private readonly JObject settingsDoc;

    private bool unInstall;

    private Tasker()
    {
      var globalSettings = string.Empty;
      using (var reader =
        new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig/GlobalSettings.json")))
      {
        globalSettings = reader.ReadToEnd();
      }

      settingsDoc = JObject.Parse(globalSettings);
    }

    public Tasker(string root, string installationPackageName, string deployRoot, bool unInstall = false) : this()
    {
      this.deployRoot = deployRoot;
      FilesRoot = root;
      var globalFilesMap = settingsDoc["GlobalFilesMap"].ToObject<Dictionary<string, string>>();
      foreach (var pattern in globalFilesMap.Keys)
        if (Regex.IsMatch(installationPackageName, pattern))
        {
          var path = Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig");
          var fileName = Directory.GetFiles(path, globalFilesMap[pattern], SearchOption.AllDirectories).Single();
          globalParamsFile = fileName;
          break;
        }

      mapping = GetPackageMapping();
      doc = JObject.Parse(File.ReadAllText(globalParamsFile));
      GlobalParams = GetGlobalParams();
      UnInstall = unInstall;
      AddInstallSifTask();
      LoadTasks();
    }

    public Tasker(string unInstallParamsPath) : this()
    {
      UnInstall = true;
      UnInstallParamsPath = unInstallParamsPath;
      List<InstallParam> deserializedGlobalParams;
      using (var reader = new StreamReader(Path.Combine(unInstallParamsPath, "globals.json")))
      {
        var filePath = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig"),
          reader.ReadLine(), SearchOption.AllDirectories).Single();
        globalParamsFile = filePath;
        var data = reader.ReadToEnd();
        deserializedGlobalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
      }

      FilesRoot = deserializedGlobalParams.Single(p => p.Name == "FilesRoot").Value;
      doc = JObject.Parse(File.ReadAllText(globalParamsFile));
      mapping = GetPackageMapping();
      GlobalParams = GetGlobalParams();
      for (var i = 0; i < GlobalParams.Count; ++i)
      {
        var param = deserializedGlobalParams.FirstOrDefault(p => p.Name == GlobalParams[i].Name);
        if (param != null)
        {
          param.ParamValueUpdated += GlobalParamValueUpdated;
          GlobalParams[i] = param;
        }
      }

      var uninstallTasksNames = Directory.GetFiles(unInstallParamsPath, "*.json")
        .Where(name => !Path.GetFileName(name).Equals("globals.json", StringComparison.InvariantCultureIgnoreCase))
        .ToList();
      foreach (var paramsFile in uninstallTasksNames)
      {
        var taskNameAdnOrder = Path.GetFileNameWithoutExtension(paramsFile);
        var index = taskNameAdnOrder.LastIndexOf('_');
        var taskName = taskNameAdnOrder.Substring(0, index);
        var order = int.Parse(taskNameAdnOrder.Substring(index + 1));
        var t = new SitecoreTask(taskName, order, this);
        //t.GlobalParams = this.globalParams;
        using (var reader = new StreamReader(paramsFile))
        {
          var param = doc["ExecSequense"].Children().Cast<JProperty>().Single(p => p.Name == taskName);
          var overridden = param.Value["Parameters"];
          var realName = param.Name;
          if (overridden != null && overridden["RealName"] != null) realName = overridden["RealName"]?.ToString();
          t.LocalParams = GetTaskParameters(realName);

          var data = reader.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(data))
          {
            var deserializedLocalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
            for (var i = 0; i < t.LocalParams.Count; ++i)
            {
              var lParam = deserializedLocalParams.FirstOrDefault(p => p.Name == t.LocalParams[i].Name);
              if (lParam != null) t.LocalParams[i] = lParam;
            }
          }

          t.UnInstall = true;
          Tasks.Add(t);
        }
      }

      AddInstallSifTask();
      Tasks = Tasks.OrderBy(t => t.ExecutionOrder).ToList();
      foreach (var p in GlobalParams) p.ParamValueUpdated += GlobalParamValueUpdated;
    }

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

    public string FilesRoot { get; }

    public List<InstallParam> GlobalParams { get; }

    public List<Task> Tasks { get; } = new List<Task>();

    public bool UnInstall
    {
      get => unInstall;
      set
      {
        unInstall = value;
        foreach (var task in Tasks) task.UnInstall = value;
      }
    }

    public string UnInstallParamsPath { get; }

    //TO DO Move InstallSiffTask to JSON with the parameters and remove this method.
    private void AddInstallSifTask()
    {
      var sifVersion = GlobalParams.First(p => p.Name == "SIFVersion").Value;
      var installSifTask = new InstallSIFTask(sifVersion, "https://sitecore.myget.org/F/sc-powershell/api/v2", this);
      Tasks.Add(installSifTask);
    }

    private List<InstallParam> GetGlobalParams()
    {
      var list = new List<InstallParam>();
      list.Add(new InstallParam("FilesRoot", FilesRoot));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        var p = new InstallParam(param.Name, param.Value.ToString());
        if (p.Name == "LicenseFile" && !string.IsNullOrWhiteSpace(FilesRoot))
        {
          var license = Path.Combine(FilesRoot, "license.xml");
          if (File.Exists(license)) p.Value = license;
        }

        p.ParamValueUpdated += GlobalParamValueUpdated;
        list.Add(p);
      }

      return list;
    }

    private void RegisterGlobalParam(InstallParam parameter)
    {
      var param = GlobalParams.FirstOrDefault(p => p.Name == parameter.Name);
      if (param == null)
      {
        GlobalParams.Insert(0, parameter);
        param = parameter;
        param.ParamValueUpdated += GlobalParamValueUpdated;
      }

      if (param.Value != parameter.Value) param.Value = parameter.Value;
    }

    private void GlobalParamValueUpdated(object sender, ParamValueUpdatedArgs e)
    {
      var updatedParam = (InstallParam) sender;
      foreach (PowerShellTask task in Tasks)
      {
        var param = task.LocalParams.FirstOrDefault(p => p.Name == updatedParam.Name);
        if (param != null) param.Value = updatedParam.Value;
      }
    }

    /// <summary>
    ///   Returns dictionary of task options are defined in json files.
    /// </summary>
    /// <param name="taskDefinition"></param>
    /// <returns></returns>
    private Dictionary<string, string> GetTaskOptions(JProperty taskDefinition)
    {
      Dictionary<string, string> options = null;
      if (taskDefinition.Value["TaskOptions"] != null)
        options = JsonConvert.DeserializeObject<Dictionary<string, string>>(taskDefinition.Value["TaskOptions"]
          .ToString());
      else
        options = new Dictionary<string, string>();

      return options;
    }

    public string GetGlobalParamsScript(bool addPrefix = true)
    {
      var script = new StringBuilder();
      foreach (var param in GlobalParams)
      {
        var value = param.GetParameterValue();
        var paramName = addPrefix ? "{" + param.Name + "}" : string.Format("'{0}'", param.Name);
        script.AppendLine(string.Format("{0}{1}={2}", addPrefix ? "$" : string.Empty, paramName, value));
      }

      return script.ToString();
    }

    public void EvaluateGlobalParams()
    {
      var sifVersion = GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      var importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion)) importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      var globalParamsEval = new StringBuilder();
      globalParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      globalParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", importParam);
      globalParamsEval.AppendLine("$GlobalParams =@{");
      globalParamsEval.Append(GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n");
      globalParamsEval.AppendLine("$GlobalParamsSys =@{");
      globalParamsEval.Append(GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n$GlobalParamsSys");
      //string globalParamsEval = string.Format("Import-Module SitecoreInstallFramework{0}\n$GlobalParams =@{{1}}$GlobalParams", importParam, this.tasksToRun.First().GetGlobalParamsScript());
      Hashtable evaluatedParams = null;
      using (var PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(globalParamsEval.ToString());
        var res = PowerShellInstance.Invoke();
        evaluatedParams = (Hashtable) res.First().ImmediateBaseObject;
      }

      var evaluatedOaramsScript = new StringBuilder();
      foreach (var param in GlobalParams)
      {
        if (evaluatedParams[param.Name] == null || param.Value == evaluatedParams[param.Name].ToString()) continue;

        param.Value = (string) evaluatedParams[param.Name];
      }
    }

    public void SaveUninstallParams(string path)
    {
      var filesPath = Path.Combine(path, GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      Directory.CreateDirectory(filesPath);
      var excludeParams = settingsDoc["ExcludeUninstallParams"].Values<string>();
      using (var wr = new StreamWriter(Path.Combine(filesPath, "globals.json")))
      {
        wr.WriteLine(Path.GetFileName(globalParamsFile));
        wr.WriteLine(GetSerializedGlobalParams(excludeParams));
      }

      foreach (PowerShellTask task in Tasks.Where(t => t.ShouldRun && t.SupportsUninstall()))
      {
        var parameters = task.GetSerializedParameters(excludeParams);
        using (var wr =
          new StreamWriter(Path.Combine(filesPath, string.Format("{0}_{1}.json", task.Name, task.ExecutionOrder))))
        {
          wr.Write(parameters);
        }
      }
    }

    public string GetSerializedGlobalParams()
    {
      return JsonConvert.SerializeObject(GlobalParams);
    }

    public string GetSerializedGlobalParams(IEnumerable<string> excludeList)
    {
      return JsonConvert.SerializeObject(GlobalParams.Where(p => !excludeList.Contains(p.Name)));
    }

    public string RunAllTasks()
    {
      var results = new StringBuilder();
      EvaluateGlobalParams();
      foreach (SitecoreTask task in Tasks.Where(t => t.ShouldRun))
        try
        {
          results.AppendLine(task.Run());
          if (task.State == TaskState.Failed) break;
        }
        catch (Exception ex)
        {
          results.AppendLine(ex.ToString());
          break;
        }

      return results.ToString().Trim();
    }

    public void GenerateScripts()
    {
      var path = Path.Combine(FilesRoot, "generated_scripts");
      Directory.CreateDirectory(path);
      foreach (SitecoreTask task in Tasks.Where(t => t.ShouldRun))
        using (var writer = new StreamWriter(Path.Combine(path, string.Format("{0}.ps1", task.Name))))
        {
          writer.Write(task.GetScript());
        }
    }

    private List<InstallParam> GetTaskParameters(string name)
    {
      var file = Directory.GetFiles(FilesRoot, string.Format("{0}.json", name), SearchOption.AllDirectories)
        .FirstOrDefault();

      if (string.IsNullOrEmpty(file)) return new List<InstallParam>();

      var installParams = new List<InstallParam>();
      var doc = JObject.Parse(File.ReadAllText(file));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        var dafultValue = param.Value["DefaultValue"]?.ToString();

        var p = new InstallParam(param.Name, dafultValue);
        p.Description = param.Value["Description"]?.ToString();
        if (GlobalParams.Any(g => g.Name == p.Name && !string.IsNullOrEmpty(g.Value)))
          p.Value = GlobalParams.First(g => g.Name == p.Name).Value;
        installParams.Add(p);

        if (p.Name == "Package")
        {
          var pack = mapping.FirstOrDefault(g => g.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
          if (!string.IsNullOrEmpty(pack?.Value)) p.Value = Directory.GetFiles(FilesRoot, pack.Value).FirstOrDefault();
        }
      }

      installParams.Add(new InstallParam("Path", file));
      return installParams;
    }

    private void LoadTasks()
    {
      var order = 0;
      foreach (JProperty param in doc["ExecSequense"].Children())
      {
        var overridden = param.Value["Parameters"];
        var realName = param.Name;
        if (overridden != null && overridden["RealName"] != null) realName = overridden["RealName"]?.ToString();

        var taskFile = Directory.GetFiles(FilesRoot, string.Format("{0}.json", realName), SearchOption.AllDirectories)
          .FirstOrDefault();
        if (!string.IsNullOrEmpty(taskFile))
          if (!string.IsNullOrEmpty(deployRoot))
          {
            InjectLocalDeploymentRoot(taskFile);
            InjectGlobalDeploymentRoot(deployRoot);
          }

        //Resolves local parameters
        var localParams = GetTaskParameters(realName);
        if (overridden != null)
          foreach (JProperty newJParam in overridden.Children())
          {
            var newParam = localParams.FirstOrDefault(p => p.Name == newJParam.Name);
            if (newParam != null) newParam.Value = newJParam.Value.ToString();
          }

        //Resolves task options for the task
        var taskOptions = GetTaskOptions(param);

        //Creates a task based on type which is defined in the json
        var taskType = param.Value["Type"]?.ToString() ?? typeof(SitecoreTask).FullName;

        //Each task should have the same ctor (TaskName(string),Order(int),Tasker,LocalParams(List<InstallParams>),TaskOptions(Dictionary<string,string>))
        var t = (Task) Activator.CreateInstance(Type.GetType(taskType), param.Name, order, this, localParams,
          taskOptions);

        t.UnInstall = UnInstall;
        Tasks.Add(t);
        ++order;
      }
    }

    private void InjectLocalDeploymentRoot(string taskFilePath)
    {
      var doc = JObject.Parse(File.ReadAllText(taskFilePath));
      var node = doc["Variables"]["Site.PhysicalPath"];
      if (node != null)
      {
        var deployRoot = new JObject();
        deployRoot["Type"] = "string";
        deployRoot["Description"] = "The path to installtion root folder.";
        deployRoot["DefaultValue"] = "";
        doc["Parameters"]["DeployRoot"] = deployRoot;

        ((JValue) node).Value = "[joinpath(parameter('DeployRoot'), parameter('SiteName'))]";

        using (var wr = new StreamWriter(taskFilePath))
        {
          wr.Write(doc.ToString());
        }
      }
    }

    private void InjectGlobalDeploymentRoot(string path)
    {
      var deployRoot = GlobalParams.FirstOrDefault(p => p.Name == "DeployRoot");
      if (deployRoot == null)
      {
        deployRoot = new InstallParam("DeployRoot", path);
        RegisterGlobalParam(deployRoot);
      }
      else
      {
        deployRoot.Value = path;
      }
    }

    private List<InstallParam> GetPackageMapping()
    {
      var list = new List<InstallParam>();

      var file = globalParamsFile;
      var doc = JObject.Parse(File.ReadAllText(file));
      foreach (JProperty param in doc["PackageMapping"].Children())
      {
        var p = new InstallParam(param.Name, param.Value.ToString());
        list.Add(p);
      }

      return list;
    }
  }
}