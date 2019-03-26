using SIM.Core.Logging;
using SIM.Sitecore9Installer.Validation.Factory;

namespace SIM.Tool.Windows.UserControls.Install
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Controls.Primitives;
  using System.Xml;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Pipelines;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;
  using SIM.IO.Real;
  using SIM.Sitecore9Installer;
  using SitecoreInstaller.Validation.Factory;

  #region

  #endregion

  [UsedImplicitly]
  public partial class Instance9Validate : IWizardStep, IFlowControl
  {
    #region Fields

    [NotNull]
    private readonly ICollection<string> _AllFrameworkVersions = Environment.Is64BitOperatingSystem ? new[]
    {
      "v2.0", "v2.0 32bit", "v4.0", "v4.0 32bit"
    } : new[]
    {
      "v2.0", "v4.0"
    };

    private InstallWizardArgs _InstallParameters = null;
    private IEnumerable<Product> _StandaloneProducts;

    #endregion

    #region Constructors

    public Instance9Validate()
    {
      InitializeComponent();
    }

    #endregion

    #region Public properties

    public static bool InstallEverywhere
    {
      get
      {
        return WindowsSettings.AppInstallEverywhere.Value;
      }
    }

    #endregion

    #region Public Methods

    public bool OnMovingBack(WizardArgs wizardArg)
    {
      return true;
    }

    public bool OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (Install9WizardArgs)wizardArgs;
      var tasker = args.Takser;
      return true;
    }

   

    #endregion

    #region Methods

    #region Protected methods

    protected void Alert([NotNull] string message, [NotNull] params object[] args)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(args, nameof(args));

      WindowHelper.ShowMessage(message.FormatWith(args), "Conflict is found", MessageBoxButton.OK, MessageBoxImage.Stop);
    }

    #endregion

    #region Private methods

    private void Init()
    {
      using (new ProfileSection("Initializing InstanceDetails", this))
      {
        DataContext = new Model();
        _StandaloneProducts = ProductManager.StandaloneProducts.Where(p=>int.Parse(p.ShortVersion)>=90&&!p.PackagePath.EndsWith(".scwdp.zip"));
      }
    }

    private void PickLocationFolder([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
     // WindowHelper.PickFolder("Choose location folder", this.solrRoot, null);
    }    

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      
    }

    #endregion

    #endregion

    #region Nested type: Model

    public class Model
    {
      #region Fields

      [CanBeNull]
      [UsedImplicitly]
      public readonly Product[] _Products = ProductManager.StandaloneProducts.ToArray();

      [NotNull]
      private string _Name;

      #endregion

      #region Properties

      [NotNull]
      [UsedImplicitly]
      public string Name
      {
        get
        {
          return _Name;
        }

        set
        {
          Assert.IsNotNull(value.EmptyToNull(), "Name must not be empty");
          _Name = value;
        }
      }

      [UsedImplicitly]
      public IGrouping<string, Product> SelectedProductGroup1 { get; set; }

      #endregion
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      Init();
      var args = (Install9WizardArgs)wizardArgs;
      var tasker = args.Takser;
      Dictionary<string, string> installParams = new Dictionary<string, string>(tasker.GlobalParams.ToDictionary(x => x.Name, x => x.Value));
      foreach (var task in tasker.Tasks)
      {
        foreach (var localParam in task.LocalParams)
        {
          if (!installParams.ContainsKey(localParam.Name))
          {
            installParams.Add(localParam.Name, localParam.Value);
          }
        }

        var validators = ValidatorFactory.Instance.GetValidators(installParams);
        foreach (var validator in validators)
        {
          validator.Validate(installParams);
        }
        this.ValidatorGrid.ItemsSource = validators;
        

      }

    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {

      return true;
    }

    #endregion
  }
}
