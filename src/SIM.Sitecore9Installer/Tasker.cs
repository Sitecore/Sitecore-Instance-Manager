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

    List<SitecoreTask> tasksToRun = new List<SitecoreTask>();
    List<InstallParam> mapping;
    string filesRoot;
    string globalParamsFile;
    string deployRoot;
    bool unInstall;
    JObject doc;
    List<InstallParam> globalParams;

    public static FileInfo ResolveGlobalFile(string packagePath)
    {
      string packageName = Path.GetFileNameWithoutExtension(packagePath);
      string jsonGlobalFilesMap = string.Empty;
      using (var reader = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), "GlobalFilesMap.json")))
      {
        jsonGlobalFilesMap = reader.ReadToEnd();
      }

      Dictionary<string, string> globalFilesMap = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonGlobalFilesMap);
      foreach (string pattern in globalFilesMap.Keys)
      {
        if (Regex.IsMatch(packageName, pattern))
        {
          return new FileInfo(Path.Combine(Directory.GetCurrentDirectory(), globalFilesMap[pattern]));
        }
      }

      return null;
    }

    public List<InstallParam> GlobalParams
    {
      get
      {
        return this.globalParams;
      }
    }

    public List<SitecoreTask> Tasks
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


    public Tasker(string root, string globalFile, string deployRoot, bool unInstall = false)
    {
      this.deployRoot = deployRoot;
      this.filesRoot = root;
      this.globalParamsFile = globalFile;
      this.mapping = this.GetPackageMapping();
      this.doc = JObject.Parse(File.ReadAllText(this.globalParamsFile));
      this.globalParams = this.GetGlobalParams();
      this.UnInstall = unInstall;
      this.LoadTasks();
    }

    public Tasker(string unInstallParamsPath)
    {
      this.UnInstall = true;
      this.UnInstallParamsPath = unInstallParamsPath;
      using (StreamReader reader = new StreamReader(Path.Combine(unInstallParamsPath, "globals.json")))
      {
        this.globalParamsFile =Path.Combine(Directory.GetCurrentDirectory(),reader.ReadLine());
        this.doc = JObject.Parse(File.ReadAllText(this.globalParamsFile));
        this.mapping = this.GetPackageMapping();
        string data = reader.ReadToEnd();
        this.globalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);
      }
      this.filesRoot = this.GlobalParams.Single(p => p.Name == "FilesRoot").Value;
      List<string> uninstallTasksNames = Directory.GetFiles(unInstallParamsPath, "*.json")
        .Where(name => !Path.GetFileName(name).Equals("globals.json", StringComparison.InvariantCultureIgnoreCase))        
        .ToList();
      foreach (string paramsFile in uninstallTasksNames)     
      {     
        string taskNameAdnOrder=Path.GetFileNameWithoutExtension(paramsFile);
        int index= taskNameAdnOrder.LastIndexOf('_');
        string taskName = taskNameAdnOrder.Substring(0, index);
        int order = int.Parse(taskNameAdnOrder.Substring(index + 1));
        SitecoreTask t = new SitecoreTask(taskName,order);
        t.GlobalParams = this.globalParams;
        using (StreamReader reader = new StreamReader(paramsFile))
        {
          string data = reader.ReadToEnd();
          if (!string.IsNullOrWhiteSpace(data))
          {
            t.LocalParams = JsonConvert.DeserializeObject<List<InstallParam>>(data);            
          }
          else
          {
            JProperty param = doc["ExecSequense"].Children().Cast<JProperty>().Single(p=>p.Name==taskName);
            var overridden = param.Value["Parameters"];
            string realName = param.Name;
            if (overridden != null && overridden["RealName"] != null)
            {
              realName = overridden["RealName"]?.ToString();
            }
            t.LocalParams = this.GetTaskParameters(realName);
          }

          t.UnInstall = true;
          this.tasksToRun.Add(t);
        }
      }
      this.tasksToRun = this.tasksToRun.OrderBy(t => t.ExecutionOrder).ToList();

      foreach (InstallParam p in this.globalParams)
      {
        p.ParamValueUpdated += this.GlobalParamValueUpdated;
      }
    }

    private List<InstallParam> GetGlobalParams()
    {
      List<InstallParam> list = new List<InstallParam>();
      list.Add(new InstallParam("FilesRoot", this.filesRoot));
      foreach (JProperty param in doc["Parameters"].Children())
      {
        InstallParam p = new InstallParam(param.Name, param.Value.ToString());
        if (p.Name == "LicenseFile")
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
      foreach (SitecoreTask task in this.Tasks)
      {
        InstallParam param = task.LocalParams.FirstOrDefault(p => p.Name == updatedParam.Name);
        if (param != null)
        {
          param.Value = updatedParam.Value;
        }
      }
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
      globalParamsEval.AppendFormat("Import-Module SitecoreInstallFramework{0}\n", importParam);
      globalParamsEval.AppendLine("$GlobalParams =@{");
      globalParamsEval.Append(this.tasksToRun.First().GetGlobalParamsScript(false));
      globalParamsEval.Append("}\n");
      globalParamsEval.AppendLine("$GlobalParamsSys =@{");
      globalParamsEval.Append(this.tasksToRun.First().GetGlobalParamsScript(false));
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

    public void SaveUninstallParams(string path)
    {
      string filesPath = Path.Combine(path, this.GlobalParams.First(p => p.Name == "SqlDbPrefix").Value);
      Directory.CreateDirectory(filesPath);
      using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, "globals.json")))
      {
        wr.WriteLine(Path.GetFileName(this.globalParamsFile));
        wr.WriteLine(this.GetSerializedGlobalParams());
      }
      foreach (SitecoreTask task in this.Tasks.Where(t => t.ShouldRun))
      {
        if (task.SupportsUninstall())
        {
          string parameters = task.GetSerializedParameters();
          using (StreamWriter wr = new StreamWriter(Path.Combine(filesPath, string.Format("{0}_{1}.json", task.Name, task.ExecutionOrder))))
          {
            wr.Write(parameters);
          }
        }
        else
        {
          File.Create(Path.Combine(filesPath, string.Format("{0}_{1}.json", task.Name, task.ExecutionOrder)));
        }
      }
    }

    public string GetSerializedGlobalParams()
    {
      return JsonConvert.SerializeObject(this.GlobalParams);
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

        SitecoreTask t = new SitecoreTask(param.Name,order);
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
