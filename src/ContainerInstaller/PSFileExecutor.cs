using SIM.Sitecore9Installer;
using SIM.Sitecore9Installer.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ContainerInstaller
{
  public class PSFileExecutor:PSExecutor 
  {
    private string script;
    public PSFileExecutor(string scriptFile, string executionDir) : base(executionDir)
    {
      using(StreamReader sr= new StreamReader(scriptFile))
      {
        this.script = sr.ReadToEnd();
      }           
    }  

    public override string GetScript()
    {
      return this.script;
    }
  }
}
