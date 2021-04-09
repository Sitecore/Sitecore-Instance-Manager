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
        int version;

        if (instance.Product.Release != null)
        {
          version = instance.Product.Release.Version.MajorMinorInt;
        }
        else
        {
          int.TryParse(instance.Product.ShortVersion, out version);
        }

        if (version != default(int) && version <= 90)
        {
          return true;
        }
      }

      return false;
    }

    #endregion
  }
}
