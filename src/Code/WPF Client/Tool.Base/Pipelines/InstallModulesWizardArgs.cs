#region Usings

using System;
using System.Collections.Generic;
using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.InstallModules;
using SIM.Pipelines.Processors;
using SIM.Products;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;

#endregion

namespace SIM.Tool.Base.Pipelines
{
  /// <summary>
  ///   The install modules wizard args.
  /// </summary>
  [UsedImplicitly]
  public class InstallModulesWizardArgs : WizardArgs
  {
    #region Fields

    /// <summary>
    ///   The instance.
    /// </summary>
    public readonly Instance Instance;

    /// <summary>
    ///   The modules.
    /// </summary>
    public readonly List<Product> Modules = new List<Product>();

    private string webRootPath;

    public string WebRootPath
    {
      get { return webRootPath ?? Instance.WebRootPath; }
      set { webRootPath = value; }
    }

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InstallModulesWizardArgs"/> class.
    /// </summary>
    /// <param name="instance">
    /// The instance. 
    /// </param>
    public InstallModulesWizardArgs(Instance instance = null)
    {
      this.Instance = instance;
      if (instance != null) this.WebRootPath = instance.WebRootPath;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the name of the instance.
    /// </summary>
    /// <value> The name of the instance. </value>
    [UsedImplicitly]
    public string InstanceName
    {
      get { return this.Instance != null ? this.Instance.Name : string.Empty; }
    }

    /// <summary>
    ///   Gets or sets the product.
    /// </summary>
    /// <value> The product. </value>
    /// <exception cref="NotImplementedException">
    ///   <c>NotImplementedException</c>
    /// </exception>
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

    public Product ExtraPackage { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Converts the <see cref="InstallModulesWizardArgs" /> to a <see cref="ProcessorArgs" /> .
    /// </summary>
    /// <returns> The <see cref="ProcessorArgs" /> . </returns>
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
  }
}