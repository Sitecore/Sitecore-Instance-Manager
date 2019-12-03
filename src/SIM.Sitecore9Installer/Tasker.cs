using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    private readonly string uninstallTasksFolderName;
    private bool unInstall;

    private Tasker()
    {
      string globalSettings = string.Empty;
      using (StreamReader reader =
        new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig/GlobalSettings.json")))
      {
        globalSettings = reader.ReadToEnd();
      }

      settingsDoc = JObject.Parse(globalSettings);
      uninstallTasksFolderName = "UninstallTasks";
    }

    public Tasker(string root, string installationPackageName, string deployRoot, bool unInstall = false) : this()
    {
      this.deployRoot = deployRoot;
      FilesRoot = root;
      Dictionary<string, string> globalFilesMap = settingsDoc["GlobalFilesMap"].ToObject<Dictionary<string, string>>();
      foreach (string pattern in globalFilesMap.Keys)
        if (Regex.IsMatch(installationPackageName, pattern))
        {
          string path = Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig");
          string fileName = Directory.GetFiles(path, globalFilesMap[pattern], SearchOption.AllDirectories).Single();
          globalParamsFile = fileName;
          break;
        }

      mapping = GetPackageMapping();
      doc = JObject.Parse(File.ReadAllText(globalParamsFile));
      GlobalParams = GetGlobalParams();
      UnInstall = unInstall;
      LoadTasks();
    }

    public Tasker(string unInstallParamsPath) : this()
    {
      UnInstall = true;
      UnInstallParamsPath = unInstallParamsPath;
      string unInstallTasksPath = Path.Combine(unInstallParamsPath, uninstallTasksFolderName);
      List<InstallParam> deserializedGlobalParams;
      using (StreamReader reader = new StreamReader(Path.Combine(unInstallParamsPath, "globals.json")))
      {
        string filePath = Directory.GetFiles(Path.Combine(Directory.GetCurrentDirectory(), "GlobalParamsConfig"),
          reader.ReadLine(), SearchOption.AllDirectories).Single();
        globalParamsFile = filePath;
        string data = reader.ReadToEnd();
        deserializedGlobalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
      }

      FilesRoot = deserializedGlobalParams.Single(p => p.Name == "FilesRoot").Value;
      doc = JObject.Parse(File.ReadAllText(globalParamsFile));
      mapping = GetPackageMapping();
      GlobalParams = GetGlobalParams();
      for (int i = 0; i < GlobalParams.Count; ++i)
      {
        InstallParam param = deserializedGlobalParams.FirstOrDefault(p => p.Name == GlobalParams[i].Name);
        if (param != null)
        {
          param.ParamValueUpdated += GlobalParamValueUpdated;
          GlobalParams[i] = param;
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
        using (StreamReader reader = new StreamReader(paramsFile))
        {
          JProperty param = doc["ExecSequense"].Children().Cast<JProperty>().Single(p => p.Name == taskName);
          string taskType = param.Value["Type"]?.ToString() ?? typeof(SitecoreTask).FullName;
          JToken overridden = param.Value["Parameters"];
          string realName = param.Name;
          if (overridden != null && overridden["RealName"] != null) realName = overridden["RealName"]?.ToString();
          List<InstallParam> localParams = GetTaskParameters(realName);
          Dictionary<string, string> taskOptions = GetTaskOptions(param);

          //Each task should have the same ctor (TaskName(string),Order(int),Tasker,LocalParams(List<InstallParams>),TaskOptions(Dictionary<string,string>))
          Task t = (Task)Activator.CreateInstance(Type.GetType(taskType), param.Name, order, this, localParams, taskOptions);
          string data = reader.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(data))
          {
            List<InstallParam> deserializedLocalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
            for (int i = 0; i < t.LocalParams.Count; ++i)
            {
              InstallParam lParam = deserializedLocalParams.FirstOrDefault(p => p.Name == t.LocalParams[i].Name);
              if (lParam != null) t.LocalParams[i] = lParam;
            }
          }

          t.UnInstall = true;
          Tasks.Add(t);
        }
      }

      Tasks = Tasks.OrderBy(t => t.ExecutionOrder).ToList();
      foreach (InstallParam p in GlobalParams) p.ParamValueUpdated += GlobalParamValueUpdated;
    }


    public string FilesRoot { get; }

    public List<InstallParam> GlobalParams { get; }

    public List<Task> Tasks { get; } = new List<Task>();

    public bool UnInstall
    {
      get => unInstall;
      set
      {
        unInstall = value;
        foreach (Task task in Tasks) task.UnInstall = value;
      }
    }

    public string UnInstallParamsPath { get; }

    private List<InstallParam> GetGlobalParams()
    {
      List<InstallParam> list = new List<InstallParam>();
      list.Add(new InstallParam("FilesRoot", FilesRoot));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        InstallParam p = new InstallParam(param.Name, param.Value.ToString());
        if (p.Name == "LicenseFile" && !string.IsNullOrWhiteSpace(FilesRoot))
        {
          string license = Path.Combine(FilesRoot, "license.xml");
          if (File.Exists(license)) p.Value = license;
        }

        p.ParamValueUpdated += GlobalParamValueUpdated;
        list.Add(p);
      }

      return list;
    }

    private void RegisterGlobalParam(InstallParam parameter)
    {
      InstallParam param = GlobalParams.FirstOrDefault(p => p.Name == parameter.Name);
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
      InstallParam updatedParam = (InstallParam) sender;
      foreach (PowerShellTask task in Tasks)
      {
        InstallParam param = task.LocalParams.FirstOrDefault(p => p.Name == updatedParam.Name);
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
      StringBuilder globalParamsEval = new StringBuilder();
      globalParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      globalParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", this.GetSifVersionParam());
      globalParamsEval.AppendLine("$GlobalParams =@{");
      globalParamsEval.Append(GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n");
      globalParamsEval.AppendLine("$GlobalParamsSys =@{");
      globalParamsEval.Append(GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n$GlobalParamsSys");
      Hashtable evaluatedParams = this.GetEvaluatedParams(globalParamsEval.ToString());

      foreach (var param in this.GlobalParams)
      {
        if (evaluatedParams[param.Name] == null || param.Value == evaluatedParams[param.Name].ToString()) continue;

        param.Value = (string) evaluatedParams[param.Name];
      }
    }

    public Hashtable EvaluateLocalParams(List<InstallParam> localParams, List<InstallParam> globalParams)
    {
      StringBuilder localParamsEval = new StringBuilder();
      localParamsEval.Append("Set-ExecutionPolicy Bypass -Force\n");
      localParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", GetSifVersionParam());
      localParamsEval.AppendFormat(this.GetParamsScript(globalParams));
      localParamsEval.AppendLine("$LocalParams =@{");
      localParamsEval.Append(this.GetParamsScript(localParams, false));
      localParamsEval.Append("}\n");
      localParamsEval.AppendLine("$LocalParamsSys =@{");
      localParamsEval.Append(this.GetParamsScript(localParams, false));
      localParamsEval.Append("}\n$LocalParamsSys");

      return this.GetEvaluatedParams(localParamsEval.ToString());
    }

    public string GetSifVersionParam()
    {
      string sifVersion = this.GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        return string.Format(" -RequiredVersion {0}", sifVersion);
      }
      return string.Empty;
    }

    public Hashtable GetEvaluatedParams(string script)
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

    public void SaveUninstallData(string path)
    {
      string filesPath = Path.Combine(path, GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      string unstallTasksPath = Path.Combine(filesPath, uninstallTasksFolderName);
      Directory.CreateDirectory(filesPath);
      Directory.CreateDirectory(unstallTasksPath);
      SaveUninstallParams(filesPath);
      SaveUninstallTasks(unstallTasksPath);
    }

    public void SaveUninstallParams(string path)
    {
      IEnumerable<string> excludeParams = settingsDoc["ExcludeUninstallParams"].Values<string>();
      using (StreamWriter wr = new StreamWriter(Path.Combine(path, "globals.json")))
      {
        wr.WriteLine(Path.GetFileName(globalParamsFile));
        wr.WriteLine(GetSerializedGlobalParams(excludeParams));
      }

      foreach (PowerShellTask task in Tasks.Where(t => t.ShouldRun && t.SupportsUninstall()))
      {
        string parameters = task.GetSerializedParameters(excludeParams);
        using (StreamWriter wr =
          new StreamWriter(Path.Combine(path, string.Format("{0}_{1}.json", task.Name, task.ExecutionOrder))))
        {
          wr.Write(parameters);
        }
      }
    }

    private void SaveUninstallTasks(string targetFilePath)
    {
      string[] files = Directory.GetFiles(this.FilesRoot, "*.json", System.IO.SearchOption.TopDirectoryOnly);

      foreach (string filePath in files)
      {
        var newFile = System.IO.Path.Combine(targetFilePath, Path.GetFileName(filePath));
        File.Copy(filePath, newFile, true);
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
      StringBuilder results = new StringBuilder();
      this.EvaluateGlobalParams();
      foreach (SitecoreTask task in this.Tasks.Where(t =>
        t.ShouldRun && ((this.unInstall && t.SupportsUninstall()) || (!this.unInstall))))
      {
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
      }

      return results.ToString().Trim();
    }

    public void GenerateScripts()
    {
      string path = Path.Combine(FilesRoot, "generated_scripts");
      Directory.CreateDirectory(path);
      foreach (SitecoreTask task in Tasks.Where(t => t.ShouldRun))
        using (StreamWriter writer = new StreamWriter(Path.Combine(path, string.Format("{0}.ps1", task.Name))))
        {
          writer.Write(task.GetScript());
        }
    }

    private List<InstallParam> GetTaskParameters(string taskFolder, string taskName)
    {
      string file = Directory.GetFiles(taskFolder, string.Format("{0}.json", taskName), SearchOption.AllDirectories)
        .FirstOrDefault();

      if (string.IsNullOrEmpty(file)) return new List<InstallParam>();

      List<InstallParam> installParams = new List<InstallParam>();
      JObject doc = JObject.Parse(File.ReadAllText(file));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        string dafultValue = param.Value["DefaultValue"]?.ToString();

        string type = param.Value["Type"]?.ToString();

        InstallParam p = new InstallParam(param.Name, dafultValue, type);
        p.Description = param.Value["Description"]?.ToString();
        if (GlobalParams.Any(g => g.Name == p.Name && !string.IsNullOrEmpty(g.Value)))
          p.Value = GlobalParams.First(g => g.Name == p.Name).Value;
        installParams.Add(p);

        if (p.Name == "Package" && !unInstall)
        {
          InstallParam pack = mapping.FirstOrDefault(g => g.Name.Equals(taskName, StringComparison.InvariantCultureIgnoreCase));
          if (!string.IsNullOrEmpty(pack?.Value)) p.Value = Directory.GetFiles(FilesRoot, pack.Value).FirstOrDefault();
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
        JToken overridden = param.Value["Parameters"];
        string realName = param.Name;
        if (overridden != null && overridden["RealName"] != null) realName = overridden["RealName"]?.ToString();

        string taskFile = Directory.GetFiles(FilesRoot, string.Format("{0}.json", realName), SearchOption.AllDirectories)
          .FirstOrDefault();
        if (!string.IsNullOrEmpty(taskFile))
          if (!string.IsNullOrEmpty(deployRoot))
          {
            InjectLocalDeploymentRoot(taskFile);
            InjectGlobalDeploymentRoot(deployRoot);
          }

        //Resolves local parameters
        List<InstallParam> localParams = GetTaskParameters(FilesRoot, realName);
        if (overridden != null)
          foreach (JProperty newJParam in overridden.Children())
          {
            InstallParam newParam = localParams.FirstOrDefault(p => p.Name == newJParam.Name);
            if (newParam != null) newParam.Value = newJParam.Value.ToString();
          }

        //Resolves task options for the task
        Dictionary<string, string> taskOptions = GetTaskOptions(param);

        //Creates a task based on type which is defined in the json
        string taskType = param.Value["Type"]?.ToString() ?? typeof(SitecoreTask).FullName;

        //Each task should have the same ctor (TaskName(string),Order(int),Tasker,LocalParams(List<InstallParams>),TaskOptions(Dictionary<string,string>))
        Task t = (Task) Activator.CreateInstance(Type.GetType(taskType), param.Name, order, this, localParams,
          taskOptions);

        t.UnInstall = UnInstall;
        Tasks.Add(t);
        ++order;
      }
    }

    private void InjectLocalDeploymentRoot(string taskFilePath)
    {
      JObject doc = JObject.Parse(File.ReadAllText(taskFilePath));
      JToken node = doc["Variables"]["Site.PhysicalPath"];
      if (node != null)
      {
        JObject deployRoot = new JObject();
        deployRoot["Type"] = "string";
        deployRoot["Description"] = "The path to installtion root folder.";
        deployRoot["DefaultValue"] = "";
        doc["Parameters"]["DeployRoot"] = deployRoot;

        ((JValue) node).Value = "[joinpath(parameter('DeployRoot'), parameter('SiteName'))]";

        using (StreamWriter wr = new StreamWriter(taskFilePath))
        {
          wr.Write(doc.ToString());
        }
      }
    }

    private void InjectGlobalDeploymentRoot(string path)
    {
      InstallParam deployRoot = GlobalParams.FirstOrDefault(p => p.Name == "DeployRoot");
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
      List<InstallParam> list = new List<InstallParam>();

      string file = globalParamsFile;
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