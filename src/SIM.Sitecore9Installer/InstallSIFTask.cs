using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public class InstallSIFTask:PowerShellTask
  {
    private string sifVersion;
    private string fundamentalsVersion;
    private string repo;
    private string scriptTemaplate = "if ((Get-Module -Name SitecoreInstallFramework -ListAvailable | Where-Object { $_.Version -eq \"$sifVersion\" }) -And !(\"$sifVersion\" -eq \"1.2.1\" -And !(Get-Module -Name SitecoreFundamentals -ListAvailable | Where-Object { $_.Version -eq \"$fundamentalsVersion\" }))) {" +
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
                                     "                Install-Module -Name SitecoreInstallFramework -RequiredVersion $sifVersion -Repository $repository.Name -AllowClobber -Force -ErrorAction \"Stop\"" +
                                     "\r\n" +
                                     "                Install-Module -Name SitecoreFundamentals -RequiredVersion $fundamentalsVersion -Repository $repository.Name -AllowClobber -Force -ErrorAction \"Stop\"" +
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

    public InstallSIFTask(string sifVersion, string repo)
    {
      this.sifVersion = sifVersion;
      this.fundamentalsVersion = "1.1.0";
      this.repo = repo;
      this.Name=string.Format("Install SIF {0}",sifVersion);
      this.ExecutionOrder = int.MinValue;
      this.ShouldRun = true;
    }

    public override string GetScript()
    {
      return this.scriptTemaplate.Replace("$sifVersion", this.sifVersion).Replace("$repoAddress", this.repo).Replace("$fundamentalsVersion", this.fundamentalsVersion);
    }
  }
}
