namespace SIM.Tool.Plugins.TrayPlugin.Resourcing
{
  using System.Collections.Generic;
  using System.Drawing;
  using System.Resources;
  using SIM.Tool.Plugins.TrayPlugin.Resources;

  public class ResourcesProvider
  {
    #region Constructors

    public ResourcesProvider()
    {
      this.Sources = new List<ResourceManager>();
      this.Initialize();
    }

    #endregion

    #region Public properties

    public List<ResourceManager> Sources { get; set; }

    #endregion

    #region Public methods

    public Icon GetIconResource(string iconResourceName, Icon defaultValue)
    {
      foreach (ResourceManager resourceManager in this.Sources)
      {
        var extractedResource = resourceManager.GetObject(iconResourceName) as Icon;
        if (extractedResource != null)
        {
          return extractedResource;
        }
      }

      return defaultValue;
    }

    public string GetStringResource(string stringResourceName, string defaultValue)
    {
      foreach (ResourceManager source in this.Sources)
      {
        var extractedStr = source.GetString(stringResourceName);
        if (extractedStr != null)
        {
          return extractedStr;
        }
      }

      return defaultValue;
    }

    #endregion

    // Sorry that it isn't virtual - it's a bad practice to call virtual members from the constructor
    #region Protected methods

    protected void Initialize()
    {
      this.Sources.Add(DefaultResources.ResourceManager);
    }

    #endregion
  }
}