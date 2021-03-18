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
  using JetBrains.Annotations;

  [UsedImplicitly]
  public class InstallModulesWizardArgs : WizardArgs
  {
    #region Fields

    public Instance Instance { get; }

    public readonly List<Product> _Modules = new List<Product>();

    private string _WebRootPath;

    public string Cookies { get; }

    public Dictionary<string, string> Headers { get; }

    #endregion

    #region Constructors

    public InstallModulesWizardArgs()
    { 
    }

    public InstallModulesWizardArgs(Instance instance, string cookies = null, Dictionary<string, string> headers = null)
    {
      Instance = instance;
      if (instance != null)
      {
        WebRootPath = instance.WebRootPath;
      }

      Cookies = cookies;
      Headers = headers;
    }

    #endregion

    #region Properties

    public Product ExtraPackage { get; set; }

    [UsedImplicitly]
    public string InstanceName
    {
      get
      {
        return Instance != null ? Instance.Name : string.Empty;
      }
    }

    [CanBeNull]
    public virtual Product Product
    {
      get
      {
        return Instance != null ? Instance.Product : null;
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
      var products = _Modules;
      var product = ExtraPackage;
      if (product != null)
      {
        products.Add(product);
      }

      return new InstallModulesArgs(Instance, products, connectionString, Cookies, Headers);
    }

    #endregion

    #region Public properties

    public string WebRootPath
    {
      get
      {
        return _WebRootPath ?? Instance.WebRootPath;
      }

      set
      {
        _WebRootPath = value;
      }
    }

    #endregion
  }
}