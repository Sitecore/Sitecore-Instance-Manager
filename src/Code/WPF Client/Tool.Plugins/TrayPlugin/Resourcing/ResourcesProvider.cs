using System.Text;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using SIM.Tool.Plugins.TrayPlugin.Resources;

namespace SIM.Tool.Plugins.TrayPlugin.Resourcing
{
  public class ResourcesProvider
  {
    public List<ResourceManager> Sources { get; set; }

    public ResourcesProvider()
    {
      Sources = new List<ResourceManager>();
      Initialize();
    }

    public Icon GetIconResource(string iconResourceName, Icon defaultValue)
    {
      foreach (ResourceManager resourceManager in Sources)
      {
        var extractedResource = resourceManager.GetObject(iconResourceName) as Icon;
        if (extractedResource != null)
          return extractedResource;
      }
      return defaultValue;
    }

    public string GetStringResource(string stringResourceName, string defaultValue)
    {
      foreach (ResourceManager source in Sources)
      {
        var extractedStr = source.GetString(stringResourceName);
        if (extractedStr != null)
          return extractedStr;
      }
      return defaultValue;
    }

    //Sorry that it isn't virtual - it's a bad practice to call virtual members from the constructor
    protected void Initialize()
    {
      Sources.Add(DefaultResources.ResourceManager);
    }
  }
}