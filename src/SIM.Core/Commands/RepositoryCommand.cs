namespace SIM.Core.Commands
{
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Products;

  public class RepositoryCommand : AbstractCommand<RepositoryCommandResult>
  {
    protected override void DoExecute(CommandResultBase<RepositoryCommandResult> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var profile = Profile.Read();
      var repository = profile.LocalRepository;
      Assert.IsNotNullOrEmpty(repository, "Profile.LocalRepository is null or empty");

      ProductManager.Initialize(repository);
      var data = new RepositoryCommandResult
      {
        Standalone = ProductManager.StandaloneProducts.Select(x => x.ToString()).ToArray(),
        Modules = ProductManager.Modules.Select(x => x.ToString()).ToArray()
      };

      result.Success = true;
      result.Message = "done";
      result.Data = data;
    }
  }
}