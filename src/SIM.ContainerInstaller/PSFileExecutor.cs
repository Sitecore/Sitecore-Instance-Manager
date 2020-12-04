using System.IO;

namespace SIM.ContainerInstaller
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
