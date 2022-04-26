using System.Collections.Generic;

namespace SIM.ContainerInstaller.Repositories.TagRepository
{
  public interface ITagRepository
  {
    IEnumerable<string> GetTags();

    IEnumerable<string> GetSortedShortSitecoreTags(string sitecoreVersionParam, string namespaceParam);

    IEnumerable<string> GetToolsTags(string nameParam, string namespaceParam);

    IEnumerable<string> GetSpeOrSxaTags(IEnumerable<string> nameParams, string namespaceParam);
  }
}