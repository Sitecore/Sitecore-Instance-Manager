using SIM.Tool.Base.Wizards;
using System.Windows;

namespace SIM.Tool.Windows.UserControls.Backup
{
	public partial class BackupSettings : IWizardStep, IFlowControl
	{
    private bool _databases;
    private bool _mongoDatabases;
		private bool _files;
		private bool _excludeClient;

		
		public BackupSettings()
		{
			InitializeComponent();
		}

		void IWizardStep.InitializeStep(WizardArgs wizardArgs)
		{
			BackupName.Text = ((BackupSettingsWizardArgs) wizardArgs).BackupName;
		}

		bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
		{
			var args = (BackupSettingsWizardArgs)wizardArgs;
			
			if (!string.IsNullOrEmpty(BackupName.Text)) 
				args.BackupName = BackupName.Text;

			args.Files = _files;
			args.Databases = _databases;
		  args.MongoDatabases = _mongoDatabases;
			args.ExcludeClient = !_excludeClient;

			return true;
		}

		private void OnUnchanged(object sender, RoutedEventArgs e)
		{
			var name = ((System.Windows.Controls.CheckBox)e.Source).Name;

			switch (name)
			{
				case ("Databases"):
					_databases = false;
					break;

        case ("MongoDatabases"):
          _mongoDatabases = false;
          break;
				
				case ("Files"):
					_files = false;
					_excludeClient = false;
					ExcludeClient.IsChecked = false;
					break;

				case ("ExcludeClient"):
					_excludeClient = false;
					break;
			}
		}

		private void OnChanged(object sender, RoutedEventArgs e)
		{
			var name = ((System.Windows.Controls.CheckBox) e.Source).Name;

			switch (name)
			{
				case ("Databases"):
					_databases = true;
					break;

        case ("MongoDatabases"):
          _mongoDatabases = true;
          break;

				case ("Files"):
					_files = true;
					break;

				case ("ExcludeClient"):
					_excludeClient = true;
					_files = true;
					Files.IsChecked = true;
					break;
			}
		}

	  public bool OnMovingNext(WizardArgs wizardArgs)
	  {
	    var args = (BackupSettingsWizardArgs)wizardArgs;

	    if (args.Databases || args.MongoDatabases || args.Files) return true;
      MessageBox.Show("You haven't chosen any backup option");
	    return false;
	  }

	  public bool OnMovingBack(WizardArgs wizardArgs)
	  {
	    return true;
	  }
	}
}
