namespace SIM.Core.Models
{
  using System.Collections.Generic;
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class InstanceInfo
  {
    public InstanceInfo(long id, [NotNull] string name, [NotNull] string state, [NotNull] string webRootPath)
    {
      Assert.ArgumentNotNull(name, "name");
      Assert.ArgumentNotNull(state, "state");
      Assert.ArgumentNotNull(webRootPath, "webRootPath");

      this.Id = id;
      this.Name = name;
      this.State = state;
      this.WebRootPath = webRootPath;
    }
    
    /// <summary>
    /// The IIS site ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// The IIS site Name.
    /// </summary>
    [NotNull]
    public string Name { get; set; }

    /// <summary>
    /// The IIS site and application pool state.
    /// </summary>
    [NotNull]
    public string State { get; set; }

    /// <summary>
    /// The physical folder that IIS site points to.
    /// </summary>
    [NotNull]
    public string WebRootPath { get; set; }

    /// <summary>
    /// The physical folder that is specified as "dataFolder" in Sitecore configuration files.
    /// </summary>
    [CanBeNull]
    public DirectoryInfo DataFolder { get; set; }

    /// <summary>
    /// The physical folder that is a logical root folder.
    /// </summary>
    [CanBeNull]
    public DirectoryInfo RootFolder { get; set; }

    /// <summary>
    /// The name, version and revision of the Sitecore product.
    /// </summary>
    [CanBeNull]
    public string ProductName { get; set; }

    /// <summary>
    /// The SQL databases specified in Sitecore configuration files.
    /// </summary>
    [CanBeNull]
    public IDictionary<string, string> Databases { get; set; }

    /// <summary>
    /// The w3wp processes IDs.
    /// </summary>
    [CanBeNull]
    public IEnumerable<int> ProcessIds { get; set; }
  }
}