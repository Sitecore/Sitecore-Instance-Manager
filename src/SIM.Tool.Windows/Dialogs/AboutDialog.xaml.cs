namespace SIM.Tool.Windows.Dialogs
{
  using System.Diagnostics;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Navigation;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;
  using System;

  public partial class AboutDialog
  {
    #region Constructors

    public AboutDialog()
    {
      InitializeComponent();
      VersionNumber.Text = ApplicationManager.AppVersion;
      RevisionNumber.Text = ApplicationManager.AppRevision;

      var label = ApplicationManager.AppLabel;
      if (!string.IsNullOrEmpty(label))
      {
        UpdateNumber.Text = label;
      }
      else
      {
        Update.Visibility = Visibility.Hidden;
      }

      CopyrightInformation.Text = "Sitecore(R) is a registered trademark. " +
                                  "All other brand and product names are the property of their respective holders. " +
                                  "The contents of this document are the property of Sitecore.\r\n" +
                                  $"Copyright (C) 2001-{DateTime.UtcNow.Year} Sitecore. All rights reserved.";
    }

    #endregion

    #region Private methods

    private void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      CoreApp.RunApp(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (e.Key != Key.Escape)
      {
        return;
      }

      if (e.Handled)
      {
        return;
      }

      e.Handled = true;
      Close();
    }

    #endregion
  }
}