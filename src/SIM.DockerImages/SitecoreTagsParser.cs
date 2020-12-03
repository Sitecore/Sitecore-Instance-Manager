using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SIM.DockerImages.Helpers;
using SIM.DockerImages.Models;

namespace SIM.DockerImages
{
  public class SitecoreTagsParser
  {
    private static IEnumerable<SitecoreTagsEntity> _sitecoreTagsEntities;

    private static IEnumerable<SitecoreTagsEntity> SitecoreTagsEntities =>_sitecoreTagsEntities ?? (_sitecoreTagsEntities =
                 JsonConvert.DeserializeObject<IEnumerable<SitecoreTagsEntity>>(new FileHelper().GetTagsData()));

    public SitecoreTagsParser()
    {
    }

    public IEnumerable<string> GetSitecoreTags()
    {
      return SitecoreTagsEntities?.Select(entity => entity.Tags)
        .SelectMany(tags => tags.Select(tag => tag.Tag));
    }

    public IEnumerable<string> GetSitecoreTags(string sitecoreVersionParam, string namespaceParam)
    {
      return SitecoreTagsEntities?.Where(entity => entity.Namespace == namespaceParam)
        .Select(entity => entity.Tags).SelectMany(tags => tags.Select(tag => tag.Tag))
        .Where(t => t.StartsWith(sitecoreVersionParam));
    }
  }
}
