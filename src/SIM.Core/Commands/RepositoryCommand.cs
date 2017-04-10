namespace SIM.Core.Commands
{
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.IO;
  using SIM.Products;

  public class RepositoryCommand : AbstractCommand<RepositoryCommandResult>
  {
    protected override void DoExecute(CommandResult<RepositoryCommandResult> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var profile = Profile.Read(FileSystem);
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

    public RepositoryCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }
  }
}