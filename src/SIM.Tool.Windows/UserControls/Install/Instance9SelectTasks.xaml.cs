﻿using JetBrains.Annotations;
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

namespace SIM.Tool.Windows.UserControls.Install
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class Instance9SelectTasks : IWizardStep, IFlowControl, ICustomButton
  {
    private Window owner;
    private Tasker tasker;
    public Instance9SelectTasks()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      this.tasker = args.Tasker;
      this.TasksList.DataContext = args.Tasker.Tasks.Where(t=>(t.SupportsUninstall()&&t.UnInstall)||!t.UnInstall);
    }    

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      if (!args.Tasker.Tasks.Any(t => t.ShouldRun))
      {
        MessageBox.Show("At least one task must be selected");
        return false;
      }

      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    public string CustomButtonText { get=>"Advanced..."; }
    public void CustomButtonClick()
    {
      WindowHelper.ShowDialog<Install9ParametersEditor>(this.tasker, this.owner);
    }
  }
}
