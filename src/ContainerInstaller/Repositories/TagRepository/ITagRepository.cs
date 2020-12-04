using System.Collections.Generic;

namespace ContainerInstaller.Repositories.TagRepository
{
  public interface ITagRepository
  {
    IEnumerable<string> GetTags();

    IEnumerable<string> GetSortedShortSitecoreTags(string sitecoreVersionParam, string namespaceParam);
  }
}