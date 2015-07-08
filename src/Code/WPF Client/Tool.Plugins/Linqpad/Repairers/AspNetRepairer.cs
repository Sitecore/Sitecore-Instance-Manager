using System.Xml;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public class AspNetRepairer : IRepairer
  {
    public void Repair(XmlDocument doc, Instance instance)
    {
      Assert.ArgumentNotNull(doc, "doc");

      RemoveNode("/configuration/system.webServer", doc);
      RemoveNode("/configuration/system.web", doc);
    }

    protected virtual void RemoveNode(string xpath, XmlDocument doc)
    {
      var node = doc.SelectSingleNode(xpath);
      if (node != null && node.ParentNode != null)
      {
        node.ParentNode.RemoveChild(node);
      }
    }
  }
}
