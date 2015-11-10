namespace SIM.Pipelines.Delete
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class DeleteMongoDatabases : DeleteProcessor
  {
    #region Fields

    private readonly List<string> done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var detectedDatabases = args.MongoDatabases;

      DeleteDatabasesHelper.Process(detectedDatabases, this.done);
    }

    #endregion
  }
}