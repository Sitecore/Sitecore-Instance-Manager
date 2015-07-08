#region Usings

using System.Collections.Generic;
using System.Linq;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Restore
{
  #region

  

  #endregion

  /// <summary>
  ///   The restore args.
  /// </summary>
  public class RestoreArgs : ProcessorArgs
  {
    #region Fields

    /// <summary>
    ///   The backup.
    /// </summary>
    public readonly InstanceBackup Backup;

    public readonly Instance Instance;
    private readonly string instanceName;
    public string DataFolder;
    public string WebRootPath;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RestoreArgs"/> class.
    /// </summary>
    /// <param name="instance">
    /// The instance. 
    /// </param>
    /// <param name="backup">
    /// The backup. 
    /// </param>
    public RestoreArgs([NotNull] Instance instance, InstanceBackup backup = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      //if(backup == null)
      //{
      //  IEnumerable<InstanceBackup> bs = instance.GetBackups();
      //  Assert.IsNotNull(bs, "There isn't any available backup", false);
      //  backup = bs.OrderBy(b => b.Date).FirstOrDefault();
      //  Assert.IsNotNull(backup, "There isn't any available backup", false);
      //}

      this.Backup = backup;
      this.Instance = instance;
      this.WebRootPath = instance.WebRootPath; ;
      this.DataFolder = instance.DataFolderPath;
      this.instanceName = instance.Name;
    }

    public string InstanceName
    {
      get { return this.instanceName; }
    }

    #endregion
  }
}