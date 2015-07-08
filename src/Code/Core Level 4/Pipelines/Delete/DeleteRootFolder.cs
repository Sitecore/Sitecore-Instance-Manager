#region Usings

using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete root folder.
  /// </summary>
  [UsedImplicitly]
  public class DeleteRootFolder : DeleteProcessor
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

      string path = args.RootPath;
      if (!string.IsNullOrEmpty(path))
      {
        FileSystem.Local.Directory.DeleteIfExists(path);
      }
    }

    #endregion
  }
}