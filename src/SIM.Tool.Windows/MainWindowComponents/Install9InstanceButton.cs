using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Wizards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SIM.Tool.Windows.MainWindowComponents
{
  class Install9InstanceButton : IMainWindowButton
  {
    public bool IsEnabled([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      return true;
    }

    public bool IsVisible([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      return true;
    }

    public void OnClick([NotNull] Window mainWindow, [CanBeNull] Instance instance)
    {
      WizardPipelineManager.Start("install9", mainWindow, null, null, (args) =>
      {
        if (args == null)
        {
          return;
        }

        var install = (InstallWizardArgs)args;
        var product = install.Product;

        if (install.HasInstallationBeenCompleted)
        {
          MainWindowHelper.AddNewInstance(install.InstanceName, false);
        }
        
      }, () => new Install9WizardArgs());
    }
  }
}
