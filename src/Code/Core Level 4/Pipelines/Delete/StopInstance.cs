#region Usings

using System;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The stop instance.
  /// </summary>
  [UsedImplicitly]
  public class StopInstance : DeleteProcessor
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

      try
      {
        args.InstanceStop();
      }
      catch (Exception ex)
      {
        SIM.Base.Log.Warn("Cannot stop instance {0}. {1}".FormatWith(args.InstanceName, ex.Message), this.GetType(), ex);
      }
    }

    #endregion
  }
}