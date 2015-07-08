using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SIM.Base;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public class NonAsciiRepairer : IRepairer
  {
    public void Repair(XmlDocument doc, Instance instance)
    {
      Assert.ArgumentNotNull(doc, "doc");

      //sample characters that are replaced: ™ “ ’ ” ’
      var builder = ToStringBuilder(doc);
      var repaired = Regex.Replace(builder.ToString(), @"[^\u0000-\u007F]", string.Empty);
      doc.LoadXml(repaired);
    }

    protected virtual StringBuilder ToStringBuilder(XmlDocument doc)
    {
      using (var stringWriter = new StringWriter())
      {
        using (var xmlWriter = new XmlTextWriter(stringWriter))
        {
          xmlWriter.Formatting = Formatting.Indented;
          doc.WriteTo(xmlWriter);
          xmlWriter.Flush();
          return stringWriter.GetStringBuilder();
        }
      }
    }
  }
}