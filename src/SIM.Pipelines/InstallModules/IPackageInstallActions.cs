namespace SIM.Pipelines.InstallModules
{
  using SIM.Instances;
  using SIM.Products;

  public interface IPackageInstallActions
  {
    #region Public methods

    void Execute(Instance instance, Product module);

    #endregion
  }
}