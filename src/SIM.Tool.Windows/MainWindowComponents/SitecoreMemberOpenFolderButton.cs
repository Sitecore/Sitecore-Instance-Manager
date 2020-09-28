namespace SIM.Tool.Windows.MainWindowComponents
{
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class SitecoreMemberOpenFolderButton : OpenFolderButton
  {
    #region Constructors

    public SitecoreMemberOpenFolderButton(string folder) : base(folder)
    {
    }

    #endregion

    #region Public methods

    /*public override bool IsVisible(Window mainWindow, Instance instance)
    {
      return MainWindowHelper.IsEnabledOrVisibleButtonForSitecoreMember(instance);
    }*/

    #endregion
  }
}