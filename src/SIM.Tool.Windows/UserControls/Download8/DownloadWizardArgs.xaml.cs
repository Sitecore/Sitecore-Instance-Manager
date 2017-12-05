namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Download8;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;
  using SIM.Extensions;

  public class DownloadWizardArgs : WizardArgs
  {
    #region Fields

    [NotNull]
    private readonly List<ProductDownload8InCheckbox> _Products = new List<ProductDownload8InCheckbox>();

    #endregion

    #region Constructors

    [UsedImplicitly]
    public DownloadWizardArgs()
    {
    }

    public DownloadWizardArgs([NotNull] string username, [NotNull] string password)
    {
      Assert.ArgumentNotNull(username, nameof(username));
      Assert.ArgumentNotNull(password, nameof(password));
      UserName = username;
      Password = password;
    }

    #endregion

    #region Public properties

    public string Cookies { get; set; }

    public ReadOnlyCollection<Uri> Links { get; set; }

    public string Password { get; set; }

    [NotNull]
    public List<ProductDownload8InCheckbox> Products
    {
      get
      {
        return _Products;
      }
    }

    [CanBeNull]
    public IRelease[] Releases { get; } = SIM.Products.Product.Service.GetVersions("Sitecore CMS").ToArray();

    public string UserName { get; set; }

    #endregion

    #region Public methods

    [NotNull]
    public override ProcessorArgs ToProcessorArgs()
    {
      return new Download8Args(Cookies, Links, ProfileManager.Profile.LocalRepository);
    }

    #endregion
  }
}