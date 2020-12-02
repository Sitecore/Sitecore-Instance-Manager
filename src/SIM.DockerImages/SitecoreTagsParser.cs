using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using SIM.DockerImages.Helpers;
using SIM.DockerImages.Models;

namespace SIM.DockerImages
{
  public class SitecoreTagsParser
  {
    private readonly IEnumerable<SitecoreTagsEntity> _sitecoreTagsEntities;

    public SitecoreTagsParser()
    {
      this._sitecoreTagsEntities = JsonConvert.DeserializeObject<IEnumerable<SitecoreTagsEntity>>(new FileHelper().GetTagsData());
    }

    public IEnumerable<string> GetSitecoreTags()
    {
      return _sitecoreTagsEntities?.Select(entity => entity.Tags)
        .SelectMany(tags => tags.Select(tag => tag.Tag));
    }

    public IEnumerable<string> GetSitecoreTags(string sitecoreVersionParam, string namespaceParam)
    {
      return _sitecoreTagsEntities?.Where(entity => entity.Namespace == namespaceParam)
        .Select(entity => entity.Tags).SelectMany(tags => tags.Select(tag => tag.Tag))
        .Where(t => t.StartsWith(sitecoreVersionParam));
    }
  }
}
