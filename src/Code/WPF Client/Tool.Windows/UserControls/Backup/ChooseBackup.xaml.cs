#region Usings

using System.Collections.Generic;
using SIM.Instances;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.UserControls.Install.Modules;

#endregion

namespace SIM.Tool.Windows.UserControls.Backup
{
  #region

  

  #endregion

  /// <summary>
  ///   Interaction logic for FilePackages.xaml
  /// </summary>
  public partial class ChooseBackup : IWizardStep
  {
    #region Fields

    /// <summary>
    ///   The check box items.
    /// </summary>
    private readonly List<InstanceBackup> checkBoxItems = new List<InstanceBackup>();

    #endregion

    #region Constructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="FilePackages" /> class.
    /// </summary>
    public ChooseBackup()
    {
      this.InitializeComponent();
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {                       
      var args = (RestoreWizardArgs)wizardArgs;
      this.checkBoxItems.Clear();

      this.checkBoxItems.AddRange(args.Instance.Backups);
      

      this.backups.DataContext = this.checkBoxItems;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {       
      var args = (RestoreWizardArgs)wizardArgs;
      args.Backup = this.backups.SelectedItem as InstanceBackup;

      return true;
    }

    #endregion
  }
}