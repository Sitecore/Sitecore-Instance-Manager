using SIM.Pipelines.PublishingService.Uninstall;

namespace SIM.Tool.Base.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.IO;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Pipelines.Install;
  using SIM.Pipelines.Processors;
  using SIM.Services;
  using SIM.Tool.Base.Converters;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  public class UninstallSPSWizardArgs : WizardArgs
  {
    public UninstallSPSWizardArgs(Instance instance)
    {
      this.Instance = instance;
      this.InstanceName = instance.Name;
    }

    #region Properties

    public Instance Instance { get; private set; }
    public string InstanceName { get; private set; }
    public string SPSInstanceFolder { get; } = ProfileManager.Profile.InstancesFolder;

    public string SPSSiteName { get; set; }
    public string SPSAppPoolName { get; set; }
    public string SPSWebroot { get; set; }
    public bool SkipSPSSite { get; set; }
    public bool SkipSPSAppPool { get; set; }
    public bool SkipSPSWebroot { get; set; }

    public override ProcessorArgs ToProcessorArgs()
    {
      return new UninstallSPSProcessorArgs()
      {
        Instance = this.Instance,
        SPSSiteName = this.SPSSiteName,
        SPSAppPoolName = this.SPSAppPoolName,
        SPSWebroot = this.SPSWebroot,
        SkipSPSSite = this.SkipSPSSite,
        SkipSPSAppPool = this.SkipSPSAppPool,
        SkipSPSWebroot = this.SkipSPSWebroot
      };
    }

    #endregion
  }
}