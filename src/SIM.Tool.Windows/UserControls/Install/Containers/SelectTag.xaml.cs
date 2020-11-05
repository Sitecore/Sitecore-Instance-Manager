using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using Sitecore.Diagnostics.Base;
using System.IO;
using System.Linq;
using System.Windows;
using TaskDialogInterop;

namespace SIM.Tool.Windows.UserControls.Install.Containers
{
  /// <summary>
  /// Interaction logic for Instance9SelectTasks.xaml
  /// </summary>
  public partial class SelectTag : IWizardStep, IFlowControl
  {
    private Window owner;
    public SelectTag()
    {
      InitializeComponent();
    }

    public void InitializeStep(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      this.owner = args.WizardWindow;
      string[] tags = Directory.GetDirectories(args.FilesRoot);
      this.Tags.DataContext = tags.Select(t=>new NameValueModel(Path.GetFileName(t),t));
      this.Tags.SelectedIndex = 0;
    }    

    public bool OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      Assert.ArgumentNotNull(wizardArgs, nameof(wizardArgs));
      InstallContainerWizardArgs args = (InstallContainerWizardArgs)wizardArgs;
      args.Tag = ((NameValueModel)this.Tags.SelectedValue).Value;
      args.DockerRoot = ((NameValueModel)this.Topoligies.SelectedItem).Value;
      return true;
    }

    public bool SaveChanges(WizardArgs wizardArgs)
    {
      return true;
    }

    private void Tags_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
      NameValueModel tag = (NameValueModel)this.Tags.SelectedValue;
      if (tag != null)
      {
        string[] topoligies = Directory.GetDirectories(tag.Value);
        this.Topoligies.DataContext = topoligies.Select(t => new NameValueModel(Path.GetFileName(t), t));
        this.Topoligies.SelectedIndex = 0;
      }
    }

    private class NameValueModel
    {
      public NameValueModel(string name, string value)
      {
        this.Name = name;
        this.Value = value;
      }

      public string Name { get; }
      public string Value { get; }
    }
  }

 
}
