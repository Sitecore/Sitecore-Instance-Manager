namespace SIM.Pipelines.Delete
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class DeleteMongoDatabases : DeleteProcessor
  {
    #region Fields

    private readonly List<string> _Done = new List<string>();

    #endregion

    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var detectedDatabases = args.MongoDatabases;

      DeleteDatabasesHelper.Process(detectedDatabases, _Done);
    }

    #endregion
  }
}