namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Pipelines.Processors;
  using SIM.Products;
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
      this.UserName = username;
      this.Password = password;
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
        return this._Products;
      }
    }

    [CanBeNull]
    public IRelease[] Releases
    {
      get
      {
        return Extensions.With(SIM.Products.Product.Service.GetVersions("Sitecore CMS")
            .With(x => x.Where(z => z.MajorMinor.StartsWith("8"))), x => x.SelectMany(y => y.Releases.Values).ToArray());
      }
    }

    public string UserName { get; set; }

    #endregion

    #region Public methods

    [NotNull]
    public override ProcessorArgs ToProcessorArgs()
    {
      return new Download8Args(this.Cookies, this.Links, ProfileManager.Profile.LocalRepository);
    }

    #endregion
  }
}