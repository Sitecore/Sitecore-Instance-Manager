using SIM.Pipelines;
using SIM.Pipelines.Processors;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
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

namespace SIM.Tool.Windows.UserControls.Install
{
  /// <summary>
  /// Interaction logic for Instance9TweakInstallParams.xaml
  /// </summary>
  public partial class Instance9TweakInstallParams : IWizardStep, IFlowControl
  {
    public Instance9TweakInstallParams()
    {
      InitializeComponent();
    }

    private Tasker tasker;

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      Install9WizardArgs args = (Install9WizardArgs)wizardArgs;
      this.tasker = args.Takser;
      List<TasksModel> model = new List<TasksModel>();
      model.Add(new TasksModel("Global", args.Takser.GlobalParams));
      foreach (SitecoreTask task in args.Takser.Tasks)
      {
        model.Add(new TasksModel(task.Name, task.LocalParams));
      }

      this.InstallationParameters.DataContext = model;
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
     
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    private class TasksModel
    {
      public TasksModel(string Name, List<InstallParam> Params)
      {
        this.Name = Name;
        this.Params = Params;
      }

      public string Name { get; }
      public List<InstallParam> Params { get; }
    }

    
  }
}
