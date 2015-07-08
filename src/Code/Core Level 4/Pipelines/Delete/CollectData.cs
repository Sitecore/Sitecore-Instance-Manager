#region Usings

using System;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The collect data.
  /// </summary>
  [UsedImplicitly]
  public class CollectData : DeleteProcessor
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
      args.WebRootPath = args.Instance.WebRootPath;
      args.RootPath = args.Instance.RootPath;
    }

    #endregion
  }
}