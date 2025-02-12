namespace SIM.Pipelines.ClearBackups
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #region
  
  [UsedImplicitly]
  public class RemoveSelectedBackups : ClearBackupsProcessor
  {
    #region Methods

    protected override void Process([NotNull] ClearBackupsArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      foreach (var _folderPath in args.SelectedBackups)
      {
        FileSystem.FileSystem.Local.Directory.DeleteIfExists(_folderPath);
      }
    }

    #endregion
  } 
  #endregion
}