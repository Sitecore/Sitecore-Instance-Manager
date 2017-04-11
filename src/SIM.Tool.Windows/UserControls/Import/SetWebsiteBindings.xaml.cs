namespace SIM.Tool.Windows.UserControls.Import
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using SIM.Adapters.WebServer;
  using SIM.Tool.Base.Wizards;

  public partial class SetWebsiteBindings : UserControl, IWizardStep
  {

    #region Fields

    private List<BindingsItem> _BindingsItems = new List<BindingsItem>();

    #endregion


    #region Constructors

    public SetWebsiteBindings()
    {
      this.InitializeComponent();
    }

    #endregion

    #region Public methods

    public void InitializeStep(WizardArgs wizardArgs)
    {
      ImportWizardArgs args = (ImportWizardArgs)wizardArgs;

      // TODO: Parse bindings in (ImportWizardArgs), Fill user control from bindings dictionary, Append all bindings in hosts
      this.siteBindings.DataContext = args._Bindings;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
      List<string> usedBindings = new List<string>();
      foreach (var binding in args._Bindings.Where(x => x.IsChecked == true))
      {
        if (this.BindingUsing(binding.HostName))
        {
          usedBindings.Add(binding.HostName);
        }
      }

      if (usedBindings.Count > 0)
      {
        var usedBindingsMessage = string.Empty;
        foreach (string binding in usedBindings)
        {
          usedBindingsMessage += binding + "\n";
        }

        MessageBox.Show("The following bindings are already used:\n" + usedBindingsMessage);
        return false;
      }
      else
      {
        return true;
      }
    }

    #endregion


    #region Private methods

    private bool BindingUsing(string hostname)
    {
      return WebServerManager.HostBindingExists(hostname);
    }

    #endregion
  }

}