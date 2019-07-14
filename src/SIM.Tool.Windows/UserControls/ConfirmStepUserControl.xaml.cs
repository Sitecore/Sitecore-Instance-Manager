using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System.IO;
using System.Linq;

namespace SIM.Tool.Windows.UserControls
{
  /// <summary>
  ///   The confirm step user control.
  /// </summary>
  public partial class ConfirmStepUserControl: IWizardStep, IFlowControl
  {
    #region Constructors

    public ConfirmStepUserControl(string param)
    {
      InitializeComponent();
      TextBlock.Text = param;
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      
    }

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      throw new System.NotImplementedException();
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
     
      var args = (ReinstallWizardArgs)wizardArgs;
      if (int.Parse(args.Instance.Product.ShortVersion) < 90)
      {
        return true;
      }

      string uninstallPath = string.Empty;
      foreach (string installName in Directory.GetDirectories(ApplicationManager.UnInstallParamsFolder))
      {
        if (args.Instance.Name.StartsWith(Path.GetFileName(installName)))
        {
          uninstallPath = installName;
          break;
        }
      }

      if (string.IsNullOrEmpty(uninstallPath))
      {
        WindowHelper.ShowMessage("UnInstall files not found.");
        return false;
      }

      Tasker tasker = new Tasker(uninstallPath);     
      tasker.UnInstall = true;
      args.Tasker = tasker;
      return true;
    }

    

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }



    #endregion
  }
}