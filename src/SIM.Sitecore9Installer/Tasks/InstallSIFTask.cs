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

    private readonly string sifVersion;

    //TO DO This ctor should be removed.
    public InstallSIFTask(string sifVersion, string repo, Tasker owner) : base("Install SIF " + sifVersion,
      int.MinValue, owner, new List<InstallParam>(), new Dictionary<string, string>())
    {
      this.sifVersion = sifVersion;
      this.repo = repo;
      ShouldRun = true;
    }

    //TO DO Add SIF task to json. sifversion and repo should be passed in the taskOptions. This ctor is not used currently.
    public InstallSIFTask(string taskName, int executionOrder, Tasker owner, List<InstallParam> localParams,
      Dictionary<string, string> taskOptions)
      : base(taskName, executionOrder, owner, localParams, taskOptions)
    {
      sifVersion = sifVersion;
      repo = repo;
      Name = string.Format("Install SIF {0}", sifVersion);
      ExecutionOrder = int.MinValue;
    }

    public override string GetScript()
    {
      return scriptTemaplate.Replace("$sifVersion", sifVersion).Replace("$repoAddress", repo);
    }
  }
}