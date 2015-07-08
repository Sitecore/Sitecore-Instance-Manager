#region Usings

using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines.Delete
{
  #region

  

  #endregion

  /// <summary>
  ///   The delete processor.
  /// </summary>
  public abstract class DeleteProcessor : Processor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override sealed void Process([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      this.Process((DeleteArgs)args);
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected abstract void Process([NotNull] DeleteArgs args);

    #endregion
  }
}