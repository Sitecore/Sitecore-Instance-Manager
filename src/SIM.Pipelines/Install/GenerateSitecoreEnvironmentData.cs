using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
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
      if (arguments.Tasker.UnInstall)// || arguments.ScriptsOnly)
      {
        this.Skip();
        return;
      }

      InstallParam sqlDbPrefixParam = arguments.Tasker.GlobalParams.FirstOrDefault(p => p.Name == SqlDbPrefix);
      if (sqlDbPrefixParam != null && !string.IsNullOrEmpty(sqlDbPrefixParam.Value))
      {
        this.AddSitecoreEnvironment(this.CreateSitecoreEnvironment(arguments.Tasker, sqlDbPrefixParam.Value));
      }
    }

    private SitecoreEnvironment CreateSitecoreEnvironment(Tasker tasker, string sqlDbPrefix)
    {
      SitecoreEnvironment sitecoreEnvironment = new SitecoreEnvironment(sqlDbPrefix);
      sitecoreEnvironment.Members = new List<SitecoreEnvironmentMember>();

      foreach (PowerShellTask powerShellTask in tasker.Tasks)
      {
        InstallParam installParam = powerShellTask.LocalParams.FirstOrDefault(x => x.Name == SiteName);
        if (installParam != null && !string.IsNullOrEmpty(installParam.Value))
        {
          Hashtable evaluatedLocalParams = tasker.EvaluateLocalParams(powerShellTask.LocalParams, tasker.GlobalParams);
          if (evaluatedLocalParams != null && evaluatedLocalParams[SiteName] != null)
          {
            SitecoreEnvironmentMember sitecoreEnvironmentMember =
              new SitecoreEnvironmentMember(evaluatedLocalParams[SiteName].ToString(),
                SitecoreEnvironmentMember.Types.Site.ToString());
            if (!sitecoreEnvironment.Members.Contains(sitecoreEnvironmentMember))
            {
              sitecoreEnvironment.Members.Add(sitecoreEnvironmentMember);
            }
          }
        }
      }

      return sitecoreEnvironment;
    }

    private void AddSitecoreEnvironment(SitecoreEnvironment sitecoreEnvironment)
    {
      List<SitecoreEnvironment> sitecoreEnvironments = SitecoreEnvironmentHelper.GetSitecoreEnvironmentData();
      if (sitecoreEnvironments == null)
      {
        sitecoreEnvironments = new List<SitecoreEnvironment>();
      }

      // Do not add new Sitecore environment if its name already exists in the Environments.json file
      foreach (SitecoreEnvironment se in sitecoreEnvironments)
      {
        if (se.Name == sitecoreEnvironment.Name)
        {
          return;
        }
      }

      sitecoreEnvironments.Add(sitecoreEnvironment);
      SitecoreEnvironmentHelper.SaveSitecoreEnvironmentData(sitecoreEnvironments);
    }
  }
}