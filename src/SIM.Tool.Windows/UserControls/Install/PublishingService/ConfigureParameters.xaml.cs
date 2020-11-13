using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
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

namespace SIM.Tool.Windows.UserControls.Install.PublishingService
{
  /// <summary>
  /// Interaction logic for ConfigureParameters.xaml
  /// </summary>
  public partial class ConfigureParameters : IWizardStep, IFlowControl
  {
    public ConfigureParameters()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      InstallPublishingServiceWizardArgs args = (InstallPublishingServiceWizardArgs)wizardArgs;
      InstanceNameTextBlock.Text = args.InstanceName;
      InstancePathTextBlock.Text = args.Instance.RootPath;
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
  }
}
