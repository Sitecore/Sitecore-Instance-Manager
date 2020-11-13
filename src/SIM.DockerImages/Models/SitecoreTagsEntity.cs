using System.Collections.Generic;

namespace SIM.DockerImages.Models
{
  public class SitecoreTagsEntity
  {
    public string Name { get; set; }

    public string Namespace { get; set; }

    public IEnumerable<Tags> Tags { get; set; }
  }
}
