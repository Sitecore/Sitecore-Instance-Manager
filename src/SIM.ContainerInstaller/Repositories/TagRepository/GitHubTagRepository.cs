using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SIM.ContainerInstaller.Repositories.TagRepository.Models;
using SIM.ContainerInstaller.Repositories.TagRepository.Parsers;

namespace SIM.ContainerInstaller.Repositories.TagRepository
{
  public class GitHubTagRepository : ITagRepository
  {
    private readonly string _shortTagPattern = @"^([0-9]+.[0-9]+.[0-9]+|[0-9]+.[0-9]+)-[^-]+$"; // tag examples: "10.0.0-ltsc2019", "10.0-2004"

    private IEnumerable<SitecoreTagsEntity> _sitecoreTagsEntities;

    private IEnumerable<SitecoreTagsEntity> SitecoreTagsEntities => 
      _sitecoreTagsEntities ?? (_sitecoreTagsEntities =
        new SitecoreTagsParser().GetTagsEntities());

    private static GitHubTagRepository _instance;

    public static GitHubTagRepository GetInstance()
    {
      return _instance ?? (_instance = new GitHubTagRepository());
    }

    public IEnumerable<string> GetTags()
    {
      return SitecoreTagsEntities?.Select(entity => entity.Tags)
        .SelectMany(tags => tags.Select(tag => tag.Tag));
    }

    private IEnumerable<string> GetSitecoreTags(string sitecoreVersionParam, string namespaceParam)
    {
      return SitecoreTagsEntities?.Where(entity => entity.Namespace == namespaceParam)
        .Select(entity => entity.Tags).SelectMany(tags => tags.Select(tag => tag.Tag))
        .Where(t => t.StartsWith(sitecoreVersionParam));
    }

    public IEnumerable<string> GetSortedShortSitecoreTags(string sitecoreVersionParam, string namespaceParam)
    {
      IEnumerable<string> tags = this.GetSitecoreTags(sitecoreVersionParam, namespaceParam);
      if (tags != null)
      {
        return this.FilterTagsByShortTagPattern(tags);
      }

      return new List<string>();
    }

    public IEnumerable<string> GetSortedShortTags(string nameParam, string namespaceParam)
    {
      IEnumerable<string> sitecoreTags = SitecoreTagsEntities?.Where(entity => entity.Name == nameParam && entity.Namespace == namespaceParam)
        .Select(entity => entity.Tags).SelectMany(tags => tags.Select(tag => tag.Tag));
      if (sitecoreTags != null)
      {
        return this.FilterTagsByShortTagPattern(sitecoreTags);
      }

      return new List<string>();
    }

    private IEnumerable<string> FilterTagsByShortTagPattern(IEnumerable<string> sitecoreTags)
    {
      Regex regex = new Regex(_shortTagPattern);
      return sitecoreTags.Where(tag => regex.IsMatch(tag)).Distinct().OrderBy(tag => tag);
    }
  }
}