namespace SIM.Pipelines.ClearBackups
{
  #region

  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #endregion

  public class ClearBackupsArgs : ProcessorArgs
  {
    #region Fields

    public Instance Instance { get; }
    public IEnumerable<string> SelectedBackups { get; }
    #endregion

    #region Constructors

    public ClearBackupsArgs([NotNull] Instance instance, IEnumerable<string> backups=null)
    {
      SelectedBackups = backups;
      Instance = instance;
    }


    public void Initialize()
    {
      InstanceID = Instance.ID;
      InstanceBackupsFolder = Instance.BackupsFolder;
    }

    #endregion

    #region Properties
    public string InstanceBackupsFolder { get; set; }
   
    #endregion

    #region Public properties

    public string InstanceName { get; private set; }

    public long InstanceID { get; private set; }

    #endregion
  }
}