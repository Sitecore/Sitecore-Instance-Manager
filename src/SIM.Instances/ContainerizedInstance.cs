using JetBrains.Annotations;
using SIM.Products;
using System;

namespace SIM.Instances
{
  public class ContainerizedInstance : Instance
  {
    private string _name;

    public ContainerizedInstance()
    {
    }

    public ContainerizedInstance(string name)
      : base(Int32.MaxValue) // Containerized instances are not run on local IIS, we cannot resolve proper site ID, thus 'Int32.MaxValue' is set.
    {
      this._name = name;
    }

    public override string Name => _name;

    public override string DisplayName
    {
      get { return Name; }
    }

    public override string BindingsNames => string.Empty;

    [NotNull]
    public override Product Product
    {
      get { return Product.Undefined; }
    }

    public override string WebRootPath
    {
      get { return this.SitecoreEnvironment.UnInstallDataPath; }
    }
  }
}