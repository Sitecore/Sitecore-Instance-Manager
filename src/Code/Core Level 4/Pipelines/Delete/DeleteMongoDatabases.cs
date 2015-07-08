namespace SIM.Pipelines.Delete
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using SIM.Base;
  using SIM.Pipelines.Processors;

  [UsedImplicitly]
  public class DeleteMongoDatabases : DeleteProcessor
  {
    private readonly List<string> done = new List<string>();

    #region Methods

    #region Protected methods

    /// <summary>
    /// Processes the processor.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Can't detect valid connection string
    /// </exception>
    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var detectedDatabases = args.MongoDatabases;

      DeleteDatabasesHelper.Process(detectedDatabases, this.done);
    }

    #endregion

    #endregion
  }
}