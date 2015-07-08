using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using SIM.Base;

namespace SIM.Tool.Windows.Dialogs
{
  using SIM.Tool.Base;

  public partial class AboutDialog
	{
		public AboutDialog()
		{
			InitializeComponent();
			VersionNumber.Text = ApplicationManager.AppVersion;
			RevisionNumber.Text = ApplicationManager.AppRevision;

		  var label = ApplicationManager.AppLabel;
		  if (!string.IsNullOrEmpty(label))
				UpdateNumber.Text = label;
			else
				Update.Visibility = Visibility.Hidden;
		}

		private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
		{
			Assert.ArgumentNotNull(sender, "sender");
			Assert.ArgumentNotNull(e, "e");

			if (e.Key != Key.Escape) return;
			if (e.Handled) return;
			e.Handled = true;
			Close();
		}

		private void OnOkButtonClick(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void RequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			WindowHelper.RunApp(new ProcessStartInfo(e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}