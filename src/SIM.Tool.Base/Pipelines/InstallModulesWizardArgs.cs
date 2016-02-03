namespace SIM.Tool.Base.Pipelines
{
  using System;
  using System.Collections.Generic;
  using SIM.Instances;
  using SIM.Pipelines.InstallModules;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class InstallModulesWizardArgs : WizardArgs
  {
    #region Fields

    public readonly Instance Instance;

    public readonly List<Product> Modules = new List<Product>();

    private string webRootPath;

    #endregion

    #region Constructors

    public InstallModulesWizardArgs(Instance instance = null)
    {
      this.Instance = instance;
      if (instance != null)
      {
        this.WebRootPath = instance.WebRootPath;
      }
    }

    #endregion

    #region Properties

    public Product ExtraPackage { get; set; }

    [UsedImplicitly]
    public string InstanceName
    {
      get
      {
        return this.Instance != null ? this.Instance.Name : string.Empty;
      }
    }

    [CanBeNull]
    public virtual Product Product
    {
      get
      {
        return this.Instance != null ? this.Instance.Product : null;
      }

      set
      {
        throw new NotImplementedException();
      }
    }

    #endregion

    #region Public Methods

    public override ProcessorArgs ToProcessorArgs()
    {
      var connectionString = ProfileManager.GetConnectionString();
      var products = this.Modules;
      var product = this.ExtraPackage;
      if (product != null)
      {
        products.Add(product);
      }

      return new InstallModulesArgs(this.Instance, products, connectionString);
    }

    #endregion

    #region Public properties

    public string WebRootPath
    {
      get
      {
        return this.webRootPath ?? this.Instance.WebRootPath;
      }

      set
      {
        this.webRootPath = value;
      }
    }

    public bool? SkipDictionaries { get; set; }
    public bool? SkipRadControls { get; set; }
    public bool? ServerSideRedirect { get; set; }
    public bool? IncreaseExecutionTimeout { get; set; }

    #endregion
  }
}