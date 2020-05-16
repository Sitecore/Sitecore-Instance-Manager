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
using SIM.Tool.Windows.UserControls.Install.Validation;

namespace SIM.Tool.Windows.UserControls.Install
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class Instance9Validation : IWizardStep, IFlowControl, ICustomButton
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
      if (args.Validate)
      {
        WindowHelper.LongRunningTask(() => this.RunValidation(), "Running validation", owner);
      }
      else
      {
        this.Caption.Text = "Validation skipped by user.";
      }

    }

    public IEnumerable<Sitecore9Installer.Validation.ValidationResult> Messages
    {
      get;set;
    }

    public string CustomButtonText => "Details...";

    private ValidationStatsItem GetStatsItemForLevel(Sitecore9Installer.Validation.ValidatorState level, Brush brush)
    {
      int count = this.Messages.Count(m => m.State == level);
      if (count > 0)
      {
        ValidationStatsItem item = new ValidationStatsItem();
        item.Color = brush;
        item.Text = $"{level}: {count}";
        return item;
      }

      return null;
    }
    private void RunValidation()
    {
      IEnumerable<Sitecore9Installer.Validation.ValidationResult> results = this.tasker.GetValidationResults();
      Dispatcher.BeginInvoke(new Action(() =>
        {
          this.Messages = results.OrderBy(r => r.State);
          if (this.Messages.Any())
          {            
            this.Caption.Text = "Validation results:";
            List<ValidationStatsItem> items = new List<ValidationStatsItem>();
            ValidationStatsItem errors = this.GetStatsItemForLevel(Sitecore9Installer.Validation.ValidatorState.Error, Brushes.Red);
            if (errors != null)
            {
              items.Add(errors);
            }

            ValidationStatsItem warns = this.GetStatsItemForLevel(Sitecore9Installer.Validation.ValidatorState.Warning, Brushes.Orange);
            if (warns != null)
            {
              items.Add(warns);
            }

            ValidationStatsItem success = this.GetStatsItemForLevel(Sitecore9Installer.Validation.ValidatorState.Success, Brushes.Green);
            if (success != null)
            {
              items.Add(success);
            }

            this.Stats.DataContext = items;
          }
          else
          {
            this.Caption.Text = "No validation results.";
          }         
        }), DispatcherPriority.Background).Wait();
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      if (this.Messages.Any(m => m.State != Sitecore9Installer.Validation.ValidatorState.Success))
      {
        if(MessageBox.Show("There are validation errors/warnings do you want to run install anyway?", "Validation", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
        {
          return true;
        }

        return false;
      }

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<ValidationDetails>(this.Messages, this.owner);
    }
  }
}
