using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using SIM.Sitecore9Installer.Tasks;
using SIM.SitecoreEnvironments;

namespace SIM.Pipelines.Install
{
  public class GenerateSitecoreEnvironmentData : Processor
  {
    private const string SiteName = "SiteName";
    private const string SqlDbPrefix = "SqlDbPrefix";

    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      if (arguments.Tasker.UnInstall || arguments.ScriptsOnly)
      {
        this.Skip();
        return;
      }

      arguments.Tasker.GlobalParams.Evaluate();
      InstallParam sqlDbPrefixParam = arguments.Tasker.GlobalParams.FirstOrDefault(p => p.Name == SqlDbPrefix);
      if (sqlDbPrefixParam != null && !string.IsNullOrEmpty(sqlDbPrefixParam.Value))
      {
        SitecoreEnvironmentHelper.AddSitecoreEnvironment(this.CreateSitecoreEnvironment(arguments.Tasker, sqlDbPrefixParam.Value,arguments.UnInstallDataPath));
      }
    }

    private SitecoreEnvironment CreateSitecoreEnvironment(Tasker tasker, string sqlDbPrefix, string uninstallDataPath)
    {
      SitecoreEnvironment sitecoreEnvironment = new SitecoreEnvironment(sqlDbPrefix);
      sitecoreEnvironment.UnInstallDataPath = uninstallDataPath;
      sitecoreEnvironment.Members = new List<SitecoreEnvironmentMember>();

      foreach (Task powerShellTask in tasker.Tasks)
      {
        InstallParam installParam = powerShellTask.LocalParams.FirstOrDefault(x => x.Name == SiteName);
        if (installParam != null && !string.IsNullOrEmpty(installParam.Value))
        {
          powerShellTask.LocalParams.Evaluate();
          sitecoreEnvironment.Members.Add(new SitecoreEnvironmentMember(installParam.Value,
              SitecoreEnvironmentMember.Types.Site.ToString()));
        }
      }

      sitecoreEnvironment.Members = sitecoreEnvironment.Members.Distinct(new SitecoreEnvironmentMemberComparer()).ToList();

      return sitecoreEnvironment;
    }
  }
}