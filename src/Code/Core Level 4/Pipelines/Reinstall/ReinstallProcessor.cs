#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Reinstall
{
  #region

  

  #endregion

  /// <summary>
  ///   The reinstall processor.
  /// </summary>
  public abstract class ReinstallProcessor : Processor
  {
    #region Public Methods

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    public override sealed bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((ReinstallArgs)args);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected virtual bool IsRequireProcessing([NotNull] ReinstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((ReinstallArgs)args);
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected abstract void Process([NotNull] ReinstallArgs args);

    #endregion
  }
}