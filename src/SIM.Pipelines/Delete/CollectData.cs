namespace SIM.Pipelines.Delete
{
  using Sitecore.Diagnostics;
  using Sitecore.Diagnostics.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class CollectData : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      args.WebRootPath = args.Instance.WebRootPath;
      args.RootPath = args.Instance.RootPath;
    }

    #endregion
  }
}