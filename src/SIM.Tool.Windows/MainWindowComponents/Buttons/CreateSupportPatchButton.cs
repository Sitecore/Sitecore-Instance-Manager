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
        // Check if instance is Sitecore XP 9.0 or earlier
        if (instance.Product.Release.Version.MajorMinorInt <= 90)
        {
          return true;
        }
      }

      return false;
    }

    #endregion
  }
}
