using System.Collections.Generic;

namespace SIM.ContainerInstaller.Repositories.TagRepository.Models
{
  public class SitecoreTagsEntity
  {
    public string Name { get; set; }

    public string Namespace { get; set; }

    public IEnumerable<TagEntity> Tags { get; set; }
  }
}