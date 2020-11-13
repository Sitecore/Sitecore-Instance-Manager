using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using SIM.DockerImages.Models;

namespace SIM.DockerImages
{
  public class SitecoreTagsParser
  {
    private const string SitecoreTagsPath = "https://raw.githubusercontent.com/Sitecore/docker-images/master/tags/sitecore-tags.json";
    
    private readonly IEnumerable<SitecoreTagsEntity> _sitecoreTagsEntities;

    private string SitecoreTagsFilePath
    {
      get { return Path.Combine(ApplicationManager.DockerImagesFolder, "sitecore-tags.json"); }
    }

    public SitecoreTagsParser()
    {
      string json = new WebClient().DownloadString(SitecoreTagsPath);

      if (!string.IsNullOrEmpty(json))
      {
        if (!File.Exists(this.SitecoreTagsFilePath))
        {
          File.WriteAllText(this.SitecoreTagsFilePath, json);
        }
        else if (json != File.ReadAllText(this.SitecoreTagsFilePath))
        {
          File.WriteAllText(this.SitecoreTagsFilePath, json);
        }

        this._sitecoreTagsEntities = JsonConvert.DeserializeObject<IEnumerable<SitecoreTagsEntity>>(json);
      }
    }

    public IEnumerable<SitecoreTagsEntity> GetSitecoreTags()
    {
      return _sitecoreTagsEntities;
    }

    public IEnumerable<SitecoreTagsEntity> GetSitecoreTags(string namespaceParam)
    {
      if (_sitecoreTagsEntities != null)
      {
        return _sitecoreTagsEntities.Where(s => s.Namespace == namespaceParam).ToList();
      }

      return null;
    }

    public IEnumerable<SitecoreTagsEntity> GetSitecoreTags(int sitecoreMajorVersionParam)
    {
      if (_sitecoreTagsEntities != null)
      {
        return _sitecoreTagsEntities.Where(s => s.Tags.Any() && s.Tags.Where(t => t.Tag.StartsWith(sitecoreMajorVersionParam.ToString())).Any()).ToList();
      }

      return null;
    }

    public IEnumerable<SitecoreTagsEntity> GetSitecoreTags(string namespaceParam, int sitecoreMajorVersionParam)
    {
      if (_sitecoreTagsEntities != null)
      {
        return _sitecoreTagsEntities.Where(s => s.Namespace == namespaceParam && s.Tags.Any() && s.Tags.Where(t => t.Tag.StartsWith(sitecoreMajorVersionParam.ToString())).Any()).ToList();
      }

      return null;
    }
  }
}
