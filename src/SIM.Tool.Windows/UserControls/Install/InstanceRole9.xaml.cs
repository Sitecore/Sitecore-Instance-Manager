﻿using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Navigation;
using Sitecore.Diagnostics.Logging;

namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public partial class InstanceRole9 : IWizardStep
  {
    public InstanceRole9()
    {
      InitializeComponent();
    }

    public void InitializeStep([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      RoleName.IsEnabled = false;

      var args = (InstallWizardArgs)wizardArgs;
      try
      {
        var ver = args.Product.TwoVersion.Replace(".", "");
        if (ver.Length <= 2)
        {
          ver += Safe.Call(() => $"{args.Product.Update}") ?? "0";
        }
       
        var txt = int.Parse(ver);

        if (txt >= 900)
        {
          RoleName.IsEnabled = true;
          if (string.IsNullOrEmpty(args.InstallRoles9))
          {
            Standalone.IsChecked = true;
          }
          else
          {
            var radio = (RadioButton) RoleName.FindName(args.InstallRoles9);
            Assert.IsNotNull(radio, $"{args.InstallRoles9} is not supported");

            radio.IsChecked = true;
          }
        }
        else
        {
          foreach (var radio in RoleName.Children.OfType<RadioButton>())
          {
            radio.IsChecked = false;
          }
        }
      }
      catch (Exception e)
      {
        Log.Error(e, "Something is wrong");
      }
    }

    public bool SaveChanges([NotNull] WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));

      var args = (InstallWizardArgs)wizardArgs;
      if (RoleName.IsEnabled != true)
      {
        args.InstallRoles9 = ""; 
        InstallWizardArgs.SaveLastTimeOption(nameof(args.InstallRoles9), args.InstallRoles9);

        return true;
      }

      var role = RoleName.Children.OfType<RadioButton>().FirstOrDefault(x => x.IsChecked == true).IsNotNull("role").Name;

      args.InstallRoles9 = role;
      InstallWizardArgs.SaveLastTimeOption(nameof(args.InstallRoles9), args.InstallRoles9);

      return true;
    }

    private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }
  }
}
