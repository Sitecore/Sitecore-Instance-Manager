using System;
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
    protected override void Process([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      if (arguments.Tasker.UnInstall)
      {

      }
      else
      {
        InstallParam sqlDbPrefixParam = arguments.Tasker.GlobalParams.FirstOrDefault(p => p.Name == "SqlDbPrefix");
        if (sqlDbPrefixParam != null && !string.IsNullOrEmpty(sqlDbPrefixParam.Value))
        {
          SitecoreEnvironment sitecoreEnvironment = SitecoreEnvironmentHelper.CreateSitecoreEnvironment(sqlDbPrefixParam.Value, arguments.Tasker.Tasks);
          List<SitecoreEnvironment> sitecoreEnvironments = SitecoreEnvironmentHelper.GetSitecoreEnvironmentData();
          sitecoreEnvironments.Add(sitecoreEnvironment);
          SitecoreEnvironmentHelper.SaveSitecoreEnvironmentData(sitecoreEnvironments);
        }
      }
    }
  }
}