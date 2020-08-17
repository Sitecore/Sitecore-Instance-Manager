﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SIM.Sitecore9Installer.Events;
using SIM.Sitecore9Installer.Tasks;
using SIM.Sitecore9Installer.Validation;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer
{
  public class Tasker
  {
    private readonly string deployRoot;
    private readonly JObject doc;
    private readonly string globalParamsFile;
    private readonly List<InstallParam> mapping;
    private readonly string uninstallTasksFolderName;
    private bool unInstall;
    private bool localParamsEvaluadted;
    private bool globalParamsEvauadted;
    private IParametersHandler paramsHandler;

    private Tasker(IParametersHandler handler)
    {
      this.Validators = new List<string>();   
      uninstallTasksFolderName = "UninstallTasks";
      this.paramsHandler = handler;
    }

    public Tasker(string root, string installationPackageName, string deployRoot, IParametersHandler handler,bool unInstall = false) : this(handler)
    {
      this.deployRoot = deployRoot;
      FilesRoot = root;
      Dictionary<string, string> globalFilesMap = Configuration.Configuration.Instance.GlobalFilesMap;
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
      this.LoadValidators();
      GlobalParams = this.paramsHandler.GetGlobalParams(this.doc, this.FilesRoot);
      EventManager.Instance.ParamValueUpdated += this.GlobalParamValueUpdated;
      UnInstall = unInstall;
      LoadTasks();
    }

    public Tasker(string unInstallParamsPath, IParametersHandler handler) : this(handler)
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
      this.LoadValidators();
      mapping = GetPackageMapping();
      GlobalParams = this.paramsHandler.GetGlobalParams(this.doc, this.FilesRoot);
      string deployRoot = deserializedGlobalParams.Single(p => p.Name == "DeployRoot")?.Value;
      if (deployRoot != null)
      {
        this.InjectGlobalDeploymentRoot(deployRoot);
      }

      for (int i = 0; i < GlobalParams.Count; ++i)
      {
        InstallParam param = deserializedGlobalParams.FirstOrDefault(p => p.Name == GlobalParams[i].Name);
        if (param != null)
        {
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
          Task t = this.CreateTask(order, param);
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
      EventManager.Instance.ParamValueUpdated += this.GlobalParamValueUpdated;
    }

    private Task CreateTask(int order, JProperty taskDefinition)
    {
      JToken overridden = taskDefinition.Value["Parameters"];
      string realName = this.GetTaskRealName(taskDefinition);
      //Resolves local parameters
      List<InstallParam> localParams = GetTaskParameters(FilesRoot, realName);
      if (overridden != null)
        foreach (JProperty newJParam in overridden.Children())
        {
          InstallParam newParam = localParams.FirstOrDefault(p => p.Name == newJParam.Name);
          if (newParam != null) newParam.Value = newJParam.Value.ToString();
        }

      //Resolves task options for the task
      Dictionary<string, string> taskOptions = GetTaskOptions(taskDefinition);
      //Creates a task based on type which is defined in the json
      string taskType = taskDefinition.Value["Type"]?.ToString() ?? typeof(SitecoreTask).FullName;
      //Each task should have the same ctor (TaskName(string),Order(int),Tasker,LocalParams(List<InstallParams>),TaskOptions(Dictionary<string,string>))
      return (Task)Activator.CreateInstance(Type.GetType(taskType), taskDefinition.Name, order, this.GlobalParams, localParams, taskOptions, this.paramsHandler);
    }

    private void LoadValidators()
    {
      JToken vals = doc["Validators"];
      if (vals != null)
      {
        this.Validators = JsonConvert.DeserializeObject<List<string>>(vals.ToString());
      }
      else
      {
        this.Validators = new List<string>();
      }
    }

    public IEnumerable<ValidationResult> GetValidationResults()
    {
      ConcurrentBag<ValidationResult> results = new ConcurrentBag<ValidationResult>();
      IEnumerable<IValidator> vals = ValidationFactory.Instance.GetValidators(this.Validators);
      Parallel.ForEach(vals, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount * 2 }, (validator) =>
      {
        try
        {
          IEnumerable<ValidationResult> result = validator.Evaluate(this.Tasks.Where(t => t.ShouldRun));
          foreach (ValidationResult r in result)
          {
            results.Add(r);
          }
        }
        catch (Exception e)
        {
          results.Add(new ValidationResult(ValidatorState.Error, e.Message, e));
        }
      });

      return results;
    }

    public void EvaluateAllParams()
    {
      this.EvaluateGlobalParams();
      this.EvaluateLocalParams();
    }

    public List<string> Validators { get; private set; }
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
    
    private void GlobalParamValueUpdated(object sender, ParamValueUpdatedArgs e)
    {    
      InstallParam updatedParam = (InstallParam) sender;
      if (!updatedParam.IsGlobal)
      {
        return;
      }

      foreach (Task task in Tasks)
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

    private void EvaluateGlobalParams()
    {
      if (this.globalParamsEvauadted)
      {
        return;
      }

      this.paramsHandler.EvaluateGlobalParams(this.GlobalParams);
      this.globalParamsEvauadted = true;
    }

    private void EvaluateLocalParams()
    {
      if (this.localParamsEvaluadted)
      {
        return;
      }
      Parallel.ForEach(this.Tasks.Where(t => t.ShouldRun),new ParallelOptions(){MaxDegreeOfParallelism=Environment.ProcessorCount*2}, 
        (task) =>
      {
        this.paramsHandler.EvaluateLocalParams(task.LocalParams, task.GlobalParams);
      });

      this.localParamsEvaluadted = true;
    }    

    public string SaveUninstallData(string path)
    {
      string filesPath = Path.Combine(path, GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      string unstallTasksPath = Path.Combine(filesPath, uninstallTasksFolderName);
      Directory.CreateDirectory(filesPath);
      Directory.CreateDirectory(unstallTasksPath);
      SaveUninstallParams(filesPath);
      SaveUninstallTasks(unstallTasksPath);
      return filesPath;
    }

    public void SaveUninstallParams(string path)
    {
      IEnumerable<string> excludeParams = Configuration.Configuration.Instance.ExcludedUninstallParams;
      using (StreamWriter wr = new StreamWriter(Path.Combine(path, "globals.json")))
      {
        wr.WriteLine(Path.GetFileName(globalParamsFile));
        wr.WriteLine(GetSerializedGlobalParams(excludeParams));
      }

      foreach (PowerShellTask task in Tasks.Where(t => t.ShouldRun && t.SupportsUninstall()&& t is PowerShellTask))
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
      return this.GetSerializedGlobalParams(Enumerable.Empty<string>());
    }

    public string GetSerializedGlobalParams(IEnumerable<string> excludeList)
    {
      return JsonConvert.SerializeObject(GlobalParams.Where(p => !excludeList.Contains(p.Name)));
    }

    public void RunLowlevelTasks()
    {
      foreach (Task t in this.Tasks.Where(t => t.ExecutionOrder < 0))
      {
        if (!t.ShouldRun || (!t.SupportsUninstall() && this.UnInstall))
        {
          continue;
        }

        string result= t.Run();
        if (t.State!=TaskState.Finished)
        {
          throw new AggregateException(result);
        }

        t.ShouldRun = false;
      }
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
          writer.Write(task.GetEvaluatedScript());
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

        InstallParam p = new InstallParam(param.Name, dafultValue, false, type);
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
        string realName = GetTaskRealName(param);
        string taskFile = Directory.GetFiles(FilesRoot, string.Format("{0}.json", realName), SearchOption.AllDirectories)
          .FirstOrDefault();
        if (!string.IsNullOrEmpty(taskFile))
        {
          if (!string.IsNullOrEmpty(deployRoot))
          {
            InjectLocalDeploymentRoot(taskFile);
            InjectGlobalDeploymentRoot(deployRoot);
          }
        }

        Task t = this.CreateTask(order, param);
        t.UnInstall = UnInstall;
        this.Tasks.Add(t);
        if (order != t.ExecutionOrder && t.ExecutionOrder >= 0)
        {
          order = t.ExecutionOrder;
          ++order;
        }
        else
        {
          if (t.ExecutionOrder >= 0)
          {
            ++order;
          }
        }
      }

      this.Tasks.OrderBy(t => t.ExecutionOrder);
    }

    private string GetTaskRealName(JProperty taskDefinition)
    {
      JToken overridden = taskDefinition.Value["Parameters"];
      string realName = taskDefinition.Name;
      if (overridden != null && overridden["RealName"] != null)
      {
        realName = overridden["RealName"]?.ToString();
      }

      return realName;
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
      this.paramsHandler.AddOrUpdateParam(this.GlobalParams, "DeployRoot", path);
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