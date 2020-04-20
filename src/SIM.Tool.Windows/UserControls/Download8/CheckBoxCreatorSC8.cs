using JetBrains.Annotations;
using SIM.Extensions;
using SIM.Products;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.InfoService.Client.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIM.Tool.Windows.UserControls.Download8
{
  public class CheckBoxCreatorSC8 : ICheckBoxCreator
  {
    protected string name;
    protected string version;
    protected string revision;

    public IEnumerable<IProductDownloadCheckBox> Create(IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));

      IDistribution distribution = release.DefaultDistribution;
      Assert.IsNotNull(distribution, nameof(distribution));

      Uri uri = new Uri(distribution.Downloads.First(x => x.StartsWith("http")));

      IProductDownloadCheckBox checkBox = GetCheckBoxInstance(release, uri);

      yield return checkBox;
    }

    protected virtual IProductDownloadCheckBox GetCheckBoxInstance(IRelease release, Uri uri)
    {
      name = "Sitecore CMS";
      version = release.Version.MajorMinor;
      revision = release.Revision;
      string label = release.Label;
      bool isEnabled = !ProductManager.Products.Any(CheckProduct);

      return new ProductDownloadCheckbox(isEnabled, name, uri, label, revision, version);
    }

    protected bool CheckProduct([CanBeNull] Products.Product product)
    {
      if (product == null)
      {
        return false;
      }

      if (!product.Name.EqualsIgnoreCase(name) && !product.OriginalName.EqualsIgnoreCase(name))
      {
        return false;
      }

      if (product.TwoVersion != version)
      {
        return false;
      }

      return product.Revision == revision;
    }
  }
}