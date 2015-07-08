namespace SIM.Tool.Plugins.SupportWorkaround
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using SIM.Base;
  using SIM.Instances;

  public static class VsHelper
  {
    public const string ProjectPrefix = "Sitecore.Support.";
    
    public static string[] GetWorkaroundProjectFiles(Instance instance)
    {
      using (new ProfileSection("Get workaround project files", typeof(VsHelper)))
      {
        ProfileSection.Argument("instance", instance);

        var result = FileSystem.Local.Directory.GetFiles(instance.WebRootPath, ProjectPrefix + "*.csproj");
        
        return ProfileSection.Result(result);
      }
    }
    
    public static List<string> FindFilesToCommit(string projectFilePath)
    {
      using (new ProfileSection("Find files to commit", typeof(VsHelper)))
      {
        ProfileSection.Argument("projectFilePath", projectFilePath);

        string slnFilePath = projectFilePath.ReplaceExtension(".sln");
        var list = new List<string>();
        if (!String.IsNullOrEmpty(slnFilePath))
        {
          list.Add(slnFilePath);
        }

        list.AddRange(FindFilesInProject(projectFilePath));

        return ProfileSection.Result(list); 
      }
    }

    private static IEnumerable<string> FindFilesInProject(string csprojFile)
    {
      string root = Path.GetDirectoryName(csprojFile);
      var doc = XmlDocumentEx.LoadFile(csprojFile);
      var itemGroups = doc.DocumentElement.ChildNodes;
      foreach (var itemGroup in itemGroups.OfType<XmlElement>().Where(el => el.Name.EqualsIgnoreCase("ItemGroup")))
      {
        foreach (var item in itemGroup.ChildNodes.OfType<XmlElement>())
        {
          if (item.Name.EqualsIgnoreCase("Reference"))
          {
            continue;
          }

          if (item.Name.EqualsIgnoreCase("Folder"))
          {
            continue;
          }

          var include = item.GetAttribute("Include");
          if (!String.IsNullOrEmpty(include) && !include.EndsWith("web.config", StringComparison.OrdinalIgnoreCase))
          {
            yield return Path.Combine(root, include);
          }
        }
      }

      yield return csprojFile;
    }
  }
}
