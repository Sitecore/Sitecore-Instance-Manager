using System.Xml;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public class ConfigStorePathRepairer : IRepairer
  {
    public void Repair(XmlDocument doc, Instance instance)
    {
      Assert.ArgumentNotNull(doc, "doc");
      Assert.ArgumentNotNull(instance, "instance");
      
      var stores = doc.SelectSingleNode("/configuration/sitecore/configStores");
      if (stores != null && stores.ChildNodes.Count > 0)
      {
        var path = instance.WebRootPath + @"\App_Config\Security\";
        foreach (XmlNode node in stores.ChildNodes)
        {
          if (node.Attributes == null)
          {
            continue;
          }
          var attr = node.Attributes["arg0"];
          if (attr == null)
          {
            continue;
          }
          if (!attr.Value.StartsWith("/App_Config/Security/"))
          {
            continue;
          }
          var newValue = attr.Value.Replace("/App_Config/Security/", path);
          attr.Value = newValue;
        }
      }
    }
  }
}