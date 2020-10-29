namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  using System.Windows;
  using JetBrains.Annotations;
  using SIM.Instances;

  [UsedImplicitly]
  public class CreateSupportPatchButton : CreateSupportHotfixButton
  {
    #region Public methods

    public CreateSupportPatchButton(string appArgsFilePath, string appUrl) : base(appArgsFilePath, appUrl)
    {
    }

    public override bool IsVisible(Window mainWindow, Instance instance)
    {
      if (base.IsVisible(mainWindow, instance))
      {
        if (base.IsSitecore90AndEarlier(instance))
        {
          return true;
        }
      }

      return false;
    }

    #endregion
  }
}
