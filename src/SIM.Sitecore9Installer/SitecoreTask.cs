using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public enum TaskState
  {
    Pending, Running, Finished, Warning, Failed
  }
  public class SitecoreTask
  {
    TaskState state;
    string name;
    StringBuilder results = new StringBuilder();
    public SitecoreTask(string name)
    {
      this.name = name;
      this.InnerTasks = new List<SitecoreTask>();
      this.state = TaskState.Pending;
      this.ShouldRun = true;
    }

    public bool UnInstall { get; set; }
    public TaskState State { get => this.state; }
    public string Name { get => this.name; }
    public bool ShouldRun { get; set; }
    public List<InstallParam> GlobalParams { get; set; }
    public List<InstallParam> LocalParams { get; set; }
    public List<SitecoreTask> InnerTasks { get; }
    //public string Run()
    //{
    //  this.state = TaskState.Running;
    //  string tabName = this.name;
    //  StringBuilder script = new StringBuilder();
    //  foreach (InstallParam param in this.GlobalParams)
    //  {
    //    string value = param.Value.StartsWith("$") ? param.Value : string.Format("\"{0}\"", param.Value);
    //    script.AppendLine(string.Format("${0}={1}", param.Name, value));
    //  }

    //  script.AppendLine("$installParams=@{");
    //  foreach (InstallParam param in this.LocalParams)
    //  {
    //    if (string.IsNullOrEmpty(param.Value))
    //    {
    //      continue;
    //    }

    //    string value = param.Value.StartsWith("$") ? param.Value : string.Format("\"{0}\"", param.Value);
    //    script.AppendLine(string.Format("{0}={1}", param.Name, value));
    //  }
    //  script.Append("}");
    //  using (PowerShell PowerShellInstance = PowerShell.Create())
    //  {
    //    PowerShellInstance.AddScript(string.Format("cd \"{0}\"", Path.GetDirectoryName(this.LocalParams.First(p => p.Name == "Path").Value)));
    //    PowerShellInstance.AddScript("Set-ExecutionPolicy Bypass -Force");
    //    PowerShellInstance.AddScript("Import-Module SitecoreFundamentals");
    //    string sifVersion = this.GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value;
    //    string importParam = string.Empty;
    //    if (!string.IsNullOrEmpty(sifVersion))
    //    {
    //      importParam = string.Format(" -RequiredVersion {0}", sifVersion);
    //    }

    //    PowerShellInstance.AddScript(string.Format("Import-Module SitecoreInstallFramework{0}", importParam));
    //    PowerShellInstance.AddScript(script.ToString());
    //    PowerShellInstance.Invoke();
    //    PowerShellInstance.AddScript("Install-SitecoreConfiguration @installParams -Verbose");
    //    Collection<PSObject> PSOutput = PowerShellInstance.Invoke();

    //    if (PowerShellInstance.Streams.Error.Count > 0)
    //    {
    //      this.state = TaskState.Failed;
    //      foreach (var error in PowerShellInstance.Streams.Error)
    //      {
    //        results.AppendLine(error.ToString());
    //      }
    //    }
    //    else
    //    {
    //      this.state = TaskState.Finished;
    //    }
    //  }

    //  return results.ToString().Trim();
    //}

    public string Run()
    {
      this.state = TaskState.Running;
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(this.GetScript());
        try
        {
          PowerShellInstance.Invoke();
        }
        catch (Exception ex)
        {
          this.state = TaskState.Failed;
          return ex.ToString();
        }

        if (PowerShellInstance.Streams.Error.Count > 0)
        {
          this.state = TaskState.Warning;
          foreach (var error in PowerShellInstance.Streams.Error)
          {
            results.AppendLine(error.ToString());
          }
        }
        else
        {
          this.state = TaskState.Finished;
        }
      }

      return results.ToString();
    }

    public string GetScript()
    {
      StringBuilder script = new StringBuilder();
     

      script.Append(GetGlobalParamsScript());

      //script.AppendLine("$installParams=@{");
      string installParams = GetLocalParamsScript();
      
      script.Append(installParams);
      script.Append(installParams);
      script.AppendLine(string.Format("cd \"{0}\"", Path.GetDirectoryName(this.LocalParams.First(p => p.Name == "Path").Value)));
      script.AppendLine("Set-ExecutionPolicy Bypass -Force");
      script.AppendLine("Import-Module SitecoreFundamentals");
      string sifVersion = this.GlobalParams.FirstOrDefault(p => p.Name == "SIFVersion")?.Value ?? string.Empty;
      string importParam = string.Empty;
      if (!string.IsNullOrEmpty(sifVersion))
      {
        importParam = string.Format(" -RequiredVersion {0}", sifVersion);
      }

      script.AppendLine(string.Format("Import-Module SitecoreInstallFramework{0}", importParam));
      // script.AppendLine(script.ToString());
      string log = !sifVersion.StartsWith("1") ? string.Format("*>&1 | Tee-Object {0}.log", this.Name) : string.Empty;
      script.AppendLine(string.Format("{0} @installParams {1} -Verbose", this.UnInstall ? "Uninstall-SitecoreConfiguration" : "Install-SitecoreConfiguration", log));
      return script.ToString();
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


        string value = this.GetParameterValue(param);

        installParams.AppendLine(string.Format("{0}={1}", param.Name, value));

      }

      installParams.AppendLine("}");
      return installParams.ToString();
    }

    public string GetGlobalParamsScript(bool addPrefix=true)
    {
      StringBuilder script = new StringBuilder();
      foreach (InstallParam param in this.GlobalParams)
      {
        string value = GetParameterValue(param);
        script.AppendLine(string.Format("{0}{1}={2}",addPrefix?"$":string.Empty ,param.Name, value));
      }

      return script.ToString();
    }

    public string GetParameterValue(InstallParam param)
    {
      string value = param.Value;
      if (!value.StartsWith("$"))
      {
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
          value = value.Remove(0, 1);
          value = value.Remove(value.Length - 1, 1);
        }
        else
        {
          value = string.Format("\"{0}\"", value);
        }
      }

      return value;
    }
  }
}
