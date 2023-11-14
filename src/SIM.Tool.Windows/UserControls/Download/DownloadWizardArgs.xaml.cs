namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Download;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.InfoService.Client.Model;

  public class DownloadWizardArgs : WizardArgs
  {
    #region Fields

    [NotNull]
    private readonly List<IProductDownloadCheckBox> _Products = new List<IProductDownloadCheckBox>();

    #endregion

    #region Constructors

    [UsedImplicitly]
    public DownloadWizardArgs()
    {
    }

    #endregion

    #region Public properties

    public string Cookies { get; set; }

    public ReadOnlyCollection<Uri> Links { get; set; }

    [NotNull]
    public List<IProductDownloadCheckBox> Products
    {
      get
      {
        return _Products;
      }
    }

    [CanBeNull]
    public IRelease[] Releases { get; } = SIM.Products.Product.Service.GetVersions("Sitecore CMS").Where(x => x.Version.Major >= 8).ToArray();

    #endregion

    #region Public methods

    [NotNull]
    public override ProcessorArgs ToProcessorArgs()
    {
      return new DownloadArgs(Cookies, Links, ProfileManager.Profile.LocalRepository);
    }

    #endregion
  }
}