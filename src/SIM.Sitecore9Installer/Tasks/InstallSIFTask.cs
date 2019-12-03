using System.Collections.Generic;

namespace SIM.Sitecore9Installer.Tasks
{
  public class InstallSIFTask : PowerShellTask
  {
    private readonly string repo;

    private readonly string scriptTemaplate =
      "if ((Get-Module -Name SitecoreInstallFramework -ListAvailable | Where-Object { $_.Version -eq \"$sifVersion\" }) -And !(\"$sifVersion\" -eq \"1.2.1\" -And !(Get-Module -Name SitecoreFundamentals -ListAvailable))) {" +
      "\r\n" +
      "                return" +
      "\r\n" +
      "            }" +
      "\r\n " +
      "           $tempRepositoryName = \"Temp\" + (New-Guid)" +
      "\r\n" +
      "            $repository = Get-PSRepository | Where-Object { $_.SourceLocation -eq \"$repoAddress\" }" +
      "\r\n" +
      "            try " +
      "{\r\n " +
      "               if (!$repository) {" +
      "\r\n" +
      "                    Register-PSRepository -Name $tempRepositoryName -SourceLocation \"$repoAddress\" -InstallationPolicy Trusted" +
      "\r\n" +
      "                    $repository = Get-PSRepository | Where-Object { $_.SourceLocation -eq \"$repoAddress\" }" +
      "\r\n" +
      "                }" +
      "\r\n" +
      "                if (!(Get-Module -Name SitecoreInstallFramework -ListAvailable | Where-Object { $_.Version -eq \"$sifVersion\" })) {" +
      "\r\n" +
      "                Install-Module -Name SitecoreInstallFramework -RequiredVersion $sifVersion -Repository $repository.Name -AllowClobber -Force -ErrorAction \"Stop\"" +
      "\r\n" +
      "                }" +
      "\r\n" +
      "                if (\"$sifVersion\" -eq \"1.2.1\" -And !(Get-Module -Name SitecoreFundamentals -ListAvailable)) {" +
      "\r\n" +
      "                Install-Module -Name SitecoreFundamentals -Repository $repository.Name -AllowClobber -Force -ErrorAction \"Stop\"" +
      "\r\n" +
      "                }" +
      "\r\n" +
      "            }" +
      "\r\n " +
      "           finally {" +
      "\r\n" +
      "                if ($repository -and $repository.Name -eq $tempRepositoryName) {" +
      "\r\n" +
      "                    Unregister-PSRepository -Name $tempRepositoryName" +
      "\r\n" +
      "                }" +
      "\r\n" +
      "            }";

    private readonly string sifVersionInstall;
    private readonly string sifVersionUnInstall;
    
    public InstallSIFTask(string taskName, int executionOrder, Tasker owner, List<InstallParam> localParams,
      Dictionary<string, string> taskOptions)
      : base(taskName, executionOrder, owner, localParams, taskOptions)
    {
      this.sifVersionInstall = this.TaskOptions["InstallVersion"];
      if (this.TaskOptions.ContainsKey("UninstallVersion"))
      {
        this.sifVersionUnInstall = this.TaskOptions["UninstallVersion"];
      }
      else
      {
        this.sifVersionUnInstall = this.sifVersionInstall;
      }

      this.repo = this.TaskOptions["Repository"];
      this.Name = taskName;
      this.ExecutionOrder = int.MinValue;
    }

    public override string GetScript()
    {
      return scriptTemaplate.Replace("$sifVersion", this.UnInstall?this.sifVersionUnInstall:this.sifVersionInstall).Replace("$repoAddress", repo);
    }
  }
}