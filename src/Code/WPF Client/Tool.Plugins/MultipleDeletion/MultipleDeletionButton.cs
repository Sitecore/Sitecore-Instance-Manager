using System;
using System.Collections.Generic;
using System.Windows;
using SIM.Instances;
using SIM.Pipelines.MultipleDeletion;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Windows;
using SIM.Tool.Wizards;

namespace SIM.Tool.Plugins.MultipleDeletion
{
  public class MultipleDeletionButton : IMainWindowButton
  {
    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      WizardPipelineManager.Start("multipleDeletion", mainWindow, new MultipleDeletionArgs(new List<string>()), null, OnWizardCompleted);
    }

    private static void OnWizardCompleted()
    {
      MainWindowHelper.SoftlyRefreshInstances();
    }
  }

}
