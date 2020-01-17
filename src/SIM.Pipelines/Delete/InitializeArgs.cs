namespace SIM.Pipelines.Delete
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region

  #endregion

  [UsedImplicitly]
  public class InitializeArgs : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      args.Initialize();
    }

    #endregion
  }
}