#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Restore
{
  #region

  

  #endregion

  /// <summary>
  ///   The restore processor.
  /// </summary>
  public abstract class RestoreProcessor : Processor
  {
    #region Methods

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    public override sealed long EvaluateStepsCount(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.EvaluateStepsCount((RestoreArgs)args);
    }

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    protected virtual long EvaluateStepsCount([NotNull] RestoreArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1;
    }

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    public override bool IsRequireProcessing(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return this.IsRequireProcessing((RestoreArgs)args);
    }

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    protected virtual bool IsRequireProcessing([NotNull] RestoreArgs args)
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
    protected override void Process(ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((RestoreArgs)args);
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected abstract void Process([NotNull] RestoreArgs args);

    #endregion
  }
}