using SIM.Adapters.WebServer;
using SIM.Tool.Base.Wizards;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SIM.Tool.Windows.UserControls.Import
{
    /// <summary>
    /// Interaction logic for SetWebsiteBindings.xaml
    /// </summary>
    public partial class SetWebsiteBindings : UserControl, IWizardStep
    {
        //
         private List<BindingsItem> BindingsItems = new List<BindingsItem>();
        //
        public SetWebsiteBindings()
        {
            InitializeComponent();
        }
                
        public void InitializeStep(WizardArgs wizardArgs)
        {
            ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
            //TODO: Parse bindings in (ImportWizardArgs), Fill user control from bindings dictionary, Append all bindings in hosts
            this.siteBindings.DataContext = args.bindings;            
        }

        public bool SaveChanges(WizardArgs wizardArgs)
        {
            ImportWizardArgs args = (ImportWizardArgs)wizardArgs;
            List<string> usedBindings = new List<string>();
            foreach (var binding in args.bindings.Where(x => x.IsChecked == true))
            {
                if (BindingUsing(binding.hostName))
                {
                    usedBindings.Add(binding.hostName);
                }
            }
            if (usedBindings.Count > 0)
            {
                string usedBindingsMessage = "";
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
        //
        bool BindingUsing(string hostname)
        {
            return WebServerManager.HostBindingExists(hostname);
        }

    }
    //    
}
