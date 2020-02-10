using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using SIM.IO.Real;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SIM.Tool.Windows.UserControls.Install.ParametersEditor;
using System.Windows.Threading;

namespace SIM.Tool.Windows.UserControls.Install
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class Instance9Validation : IWizardStep, IFlowControl
  {
    private Window owner;
    private Tasker tasker;
    public Instance9Validation()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.tasker = args.Tasker;
      WindowHelper.LongRunningTask(() => this.RunValidation(), "Running validation",owner);
    }

    public IEnumerable<Sitecore9Installer.Validation.ValidationResult> Messages
    {
      get { return this.MessagesList.DataContext as IEnumerable<Sitecore9Installer.Validation.ValidationResult>; }
      set { this.MessagesList.DataContext = value; }
    }

    private void RunValidation()
    {
      IEnumerable<Sitecore9Installer.Validation.ValidationResult> results = this.tasker.GetValidationErrors();
      Dispatcher.BeginInvoke(new Action(() =>
        {
          if (results.Any())
          {
            this.Caption.Text = "Validation results:";
            this.MessagesList.Visibility = Visibility.Visible;
          }
          else
          {
            this.Caption.Text = "Validation is successful.";
            this.MessagesList.Visibility = Visibility.Hidden;
          }

          this.Messages = results;
        }), DispatcherPriority.Background);
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      if (this.Messages.Any())
      {
        return WindowHelper.ShowMessage("There are validation errors. Do you want to run install anyway?",
                 MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes;
      }

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }
  }
}
