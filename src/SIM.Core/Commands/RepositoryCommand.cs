namespace SIM.Core.Commands
{
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Products;

  public class RepositoryCommand : AbstractCommand<RepositoryCommandResult>
  {
    protected override void DoExecute(CommandResult<RepositoryCommandResult> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var profile = Profile.Read();
      var repository = profile.LocalRepository;
      Ensure.IsNotNullOrEmpty(repository, "Profile.LocalRepository is not specified");

      ProductManager.Initialize(repository);
      var data = new RepositoryCommandResult
      {
        Standalone = ProductManager.StandaloneProducts.Select(x => x.ToString()).ToArray(),
        Modules = ProductManager.Modules.Select(x => x.ToString()).ToArray()
      };
      
      result.Data = data;
    }
  }
}