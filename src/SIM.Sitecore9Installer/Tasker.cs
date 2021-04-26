using Newtonsoft.Json;
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
    private readonly PackageMapping mapping;
    private readonly string uninstallTasksFolderName;
    private bool unInstall;

    private Tasker()
    {
      this.Validators = new List<string>();   
      uninstallTasksFolderName = "UninstallTasks";
    }

    public Tasker(string root, string installationPackageName, string deployRoot, bool unInstall = false) : this()
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

      doc = JObject.Parse(File.ReadAllText(globalParamsFile));
      mapping = GetPackageMapping();
      this.LoadValidators();
      this.GlobalParams = new GlobalParameters(this.doc, this.FilesRoot);
      EventManager.Instance.ParamValueUpdated += this.GlobalParamValueUpdated;
      UnInstall = unInstall;
      LoadTasks(this.FilesRoot);
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
      this.LoadValidators();
      mapping = GetPackageMapping();
      this.GlobalParams = new GlobalParameters(this.doc, this.FilesRoot);
      string deployRoot = deserializedGlobalParams.Single(p => p.Name == "DeployRoot")?.Value;
      if (deployRoot != null)
      {
        this.deployRoot = deployRoot;
        this.InjectGlobalDeploymentRoot(deployRoot);
      }

      for (int i = 0; i < GlobalParams.Count; ++i)
      {
        InstallParam param = deserializedGlobalParams.FirstOrDefault(p => p.Name == GlobalParams[i].Name);
        if (param != null)
        {
          GlobalParams.AddOrUpdateParam(param.Name,param.Value,param.Type);
        }
      }

      this.LoadTasks(unInstallTasksPath);
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
          Task t = this.Tasks.FirstOrDefault(task => task.Name == taskName);
          string data = reader.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(data))
          {
            List<InstallParam> deserializedLocalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
            for (int i = 0; i < t.LocalParams.Count; ++i)
            {
              InstallParam lParam = deserializedLocalParams.FirstOrDefault(p => p.Name == t.LocalParams[i].Name);
              if (lParam != null)
              {
                t.LocalParams.AddOrUpdateParam(lParam.Name,lParam.Value,lParam.Type);
              }
            }
          }
        }
      }

      Tasks = Tasks.OrderBy(t => t.ExecutionOrder).ToList();
      EventManager.Instance.ParamValueUpdated += this.GlobalParamValueUpdated;
    }

    private Task CreateTask(int order, JProperty taskDescriptor, string taskFolder)
    {
      TaskDefinition definition = new TaskDefinition(taskDescriptor);
      return definition.CreateTask(order, this.GlobalParams, taskFolder, this.UnInstall, this.mapping);
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
      this.EvaluateLocalParams();
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

    public GlobalParameters GlobalParams { get; }

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
        InstallParam param = task.LocalParams[updatedParam.Name];
        if (param != null) param.Value = updatedParam.Value;
      }
    }

    private void EvaluateGlobalParams()
    {
      this.GlobalParams.Evaluate();
    }

    private void EvaluateLocalParams()
    {      
      Parallel.ForEach(this.Tasks.Where(t => t.ShouldRun),new ParallelOptions(){MaxDegreeOfParallelism=Environment.ProcessorCount*2}, 
        (task) =>
      {
        task.LocalParams.Evaluate();
      });
    }    

    public string SaveUninstallData(string path)
    {
      string filesPath = Path.Combine(path, GlobalParams["SqlDbPrefix"].Value);
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

      foreach (PowerShellTask task in Tasks.Where(t => t.ShouldRun && t is PowerShellTask))
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

    private void LoadTasks(string path)
    {
      int order = 0;
      foreach (JProperty param in doc["ExecSequense"].Children())
      {        
        string realName = GetTaskRealName(param);
        string taskFile = Directory.GetFiles(path, string.Format("{0}.json", realName), SearchOption.AllDirectories)
          .FirstOrDefault();
          if (!string.IsNullOrEmpty(taskFile))
          {
            if (!string.IsNullOrEmpty(deployRoot))
            {
              InjectLocalDeploymentRoot(taskFile);
              InjectGlobalDeploymentRoot(deployRoot);
            }
          }

        Task t = this.CreateTask(order, param, path);
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
      JToken node = doc["Variables"]?["Site.PhysicalPath"];//have to use null propagation since there need not be a variables section at all like in case of SXA-SingleDeveloper.json
      if (node != null)
      {
        JObject deployRoot = new JObject();
        deployRoot["Type"] = "string";
        deployRoot["Description"] = "The path to installation root folder.";
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
      this.GlobalParams.AddOrUpdateParam("DeployRoot", path,InstallParamType.String);
    }

    private PackageMapping GetPackageMapping()
    {
      return new PackageMapping(doc["PackageMapping"]);
    }
  }
}