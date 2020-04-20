using System;
using System.Collections.Generic;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.InfoService.Client.Model;
using System.Linq;
using SIM.Products;

namespace SIM.Tool.Windows.UserControls.Download8
{
  public class CheckBoxCreatorSC9 : CheckBoxCreatorSC8, ICheckBoxCreator
  {
    public new IEnumerable<IProductDownloadCheckBox> Create(IRelease release)
    {
      Assert.ArgumentNotNull(release, nameof(release));
      string distPrefix = "topology-";

      IEnumerable<KeyValuePair<string, IDistribution>> distributions = release.Distributions?.Where(d => d.Key.StartsWith(distPrefix));
      if (distributions == null || distributions.Count() == 0)
      {
        yield break;
      }

      foreach (KeyValuePair<string, IDistribution> dist in distributions)
      {
        string topology = dist.Key.Substring(distPrefix.Length).ToUpper();

        Uri url = new Uri(dist.Value.Downloads.First(x => x.StartsWith("http")));

        IProductDownloadCheckBox checkBox = GetCheckBoxInstance(release, url, topology);

        yield return checkBox;
      }
    }

    protected virtual IProductDownloadCheckBox GetCheckBoxInstance(IRelease release, Uri uri, string topology)
    {
      name = "Sitecore CMS";
      version = release.Version.MajorMinor;
      revision = $"{release.Revision} (WDP {topology} packages)";
      string label = release.Label;
      bool isEnabled = !ProductManager.Products.Any(CheckProduct);

      return new ProductDownloadCheckbox(isEnabled, name, uri, label, revision, version);
    }
  }
}