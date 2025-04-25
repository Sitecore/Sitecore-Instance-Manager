using JetBrains.Annotations;
using SIM.Extensions;
using SIM.Instances;
using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public class PopulateNewIndexAction : InstallSearchIndexesProcessor
  {
    protected override void Process([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      foreach (var index in args._AvailableSearchIndexesDictionary)
      {
        string newCorePath = args.SolrFolder.EnsureEnd(@"\") + index.Value;
        PopulateNewIndex(index.Key, args.Instance, args.Headers, args.AuthCookies);
      }
    }

    private void PopulateNewIndex(string coreID, Instance instance, IDictionary<string, string> headers, string authCookie)
    {
      var popUrl = instance.GetUrl(@"/sitecore/admin/PopulateManagedSchema.aspx?indexes=" + coreID);

      try
      {
        int sitecoreVersion;

        int.TryParse(instance.Product.ShortVersion, out sitecoreVersion);

        if (sitecoreVersion >= 91)
        {
          if (headers == null)
          {
            return;
          }
          var result = WebRequestHelper.DownloadString(popUrl, headers: headers).Trim();

        }
        else if (sitecoreVersion == 90)
        {
          string instanceUri = instance.GetUrl();

          if (string.IsNullOrEmpty(authCookie))
          {
            return;
          }
          var result = WebRequestHelper.DownloadString(popUrl, cookies: authCookie).Trim();
        }
      }
      catch (Exception ex)
      {
        return;
      }
    }

  }
}
