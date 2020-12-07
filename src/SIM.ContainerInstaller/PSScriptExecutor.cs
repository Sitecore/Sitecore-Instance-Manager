using Sitecore.Diagnostics.Base;

namespace SIM.ContainerInstaller
{
  public class PSScriptExecutor : PSExecutor
  {
    private readonly string _script;
    public PSScriptExecutor(string executionDir, string script) : base(executionDir)
    {
      Assert.ArgumentNotNullOrEmpty(script, "script");

      this._script = script;
    }

    public override string GetScript()
    {
      return this._script;
    }
  }
}