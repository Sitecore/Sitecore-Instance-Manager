using System.Collections.Generic;
using System.Xml;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public abstract class MakeNodeCommentRepairerProcessor : IRepairer
  {
    protected abstract IEnumerable<string> GetPaths(XmlDocument doc, Instance instance);

    public void Repair(XmlDocument doc, Instance instance)
    {
      Assert.ArgumentNotNull(instance, "instance");

      var paths = GetPaths(doc, instance);
      foreach (var path in paths)
      {
        var nodes = doc.SelectNodes(path);
        if (nodes == null || nodes.Count == 0)
        {
          continue;
        }
        foreach (XmlNode node in nodes)
        {
          MakeComment(node);
        }
      }
    }

    protected virtual void MakeComment(XmlNode node)
    {
      Assert.ArgumentNotNull(node, "node");
      RemoveComments(node);
      var comment = node.OwnerDocument.CreateComment(node.OuterXml);
      node.ParentNode.ReplaceChild(comment, node);
    }

    protected virtual void RemoveComments(XmlNode node)
    {
      Assert.ArgumentNotNull(node, "node");

      var comments = node.SelectNodes(".//comment()");
      if (comments != null && comments.Count > 0)
      {
        foreach (XmlNode comment in comments)
        {
          comment.ParentNode.RemoveChild(comment);
        }
      }
    }
  }
}