namespace SIM.Tool.Windows
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using SIM.Instances;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public class CustomConverter : IValueConverter
  {
    #region Fields

    [NotNull]
    private readonly IMainWindowButton button;

    #endregion

    #region Constructors

    public CustomConverter([NotNull] IMainWindowButton mainWindowButton)
    {
      Assert.ArgumentNotNull(mainWindowButton, "mainWindowButton");

      this.button = mainWindowButton;
    }

    #endregion

    #region Public methods

    [CanBeNull]
    public object Convert([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      using (new ProfileSection("Checking if button is enabled", this))
      {
        ProfileSection.Argument("this.button", this.button.GetType().FullName);
        return this.button.IsEnabled(MainWindow.Instance, value as Instance);
      }
    }

    [CanBeNull]
    public object ConvertBack([CanBeNull] object value, [CanBeNull] Type targetType, [CanBeNull] object parameter, [CanBeNull] CultureInfo culture)
    {
      throw new NotSupportedException();
    }

    #endregion
  }
}