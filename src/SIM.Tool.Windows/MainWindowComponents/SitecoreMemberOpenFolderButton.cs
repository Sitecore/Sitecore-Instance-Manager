namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
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

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      if (instance != null && MainWindowHelper.IsSitecoreMember(instance))
      {
        return false;
      }

      return true;
    }

    #endregion
  }
}