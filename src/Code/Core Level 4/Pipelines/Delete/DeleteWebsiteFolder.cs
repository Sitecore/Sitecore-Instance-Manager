#region Usings

using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  


  #region

  using System.Linq;
  using SIM.Instances;

  #endregion

  /// <summary>
  ///   The delete website folder.
  /// </summary>
  [UsedImplicitly]
  public class DeleteWebsiteFolder : DeleteProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var cachedInstance = InstanceManager.PartiallyCachedInstances.SingleOrDefault(x => x.ID == args.InstanceID) as PartiallyCachedInstance;
      if (cachedInstance != null)
      {
        cachedInstance.Dispose();
      }

      FileSystem.Local.Directory.DeleteIfExists(args.WebRootPath);
    }

    #endregion
  }
}