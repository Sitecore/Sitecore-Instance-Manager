using System.Net;
using JetBrains.Annotations;
using Sitecore.Diagnostics.Base;
using SIM.IO;
using SIM.IO.Real;

namespace SIM.Pipelines.Install
{
  [UsedImplicitly]
  public class InstallRoles : InstallProcessor
  {
    [NotNull]
    private IFileSystem FileSystem { get; } = new RealFileSystem();

    #region Methods

    protected override void Process(InstallArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var role = args.InstallRoles;
      if (string.IsNullOrEmpty(role))
      {
        return;
      }

      var websiteDir = FileSystem.ParseFolder(args.WebRootPath);
      var product = args.Product;
      var version = $"{product.Version}.{Safe.Call(() => $"{product.Update}") ?? "0"}";
      InstallRolesCommandHelper.Install(websiteDir, version, role);
    } 

    #endregion
  }
}