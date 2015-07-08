using SIM.Instances;
using SIM.Products;

namespace SIM.Pipelines.InstallModules
{
  public interface IPackageInstallActions
  {
    void Execute(Instance instance, Product module);
  }
}
