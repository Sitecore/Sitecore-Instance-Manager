using JetBrains.Annotations;
using SIM.Extensions;
using Sitecore.Diagnostics.Base;
using System.Xml;

namespace SIM.Pipelines.InstallSearchIndexes
{
  public class UpdateManagedSchemaAction : InstallSearchIndexesProcessor
  {
    protected override void Process([NotNull] InstallSearchIndexesArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      foreach (var index in args._AvailableSearchIndexesDictionary)
      {
        string newCorePath = args.SolrFolder.EnsureEnd(@"\") + index.Value;

        UpdateManagedSchemaFile(newCorePath);
      }
    }

    private void UpdateManagedSchemaFile(string newCorePath)
    {
      XmlDocument doc = new XmlDocument();
      doc.Load(newCorePath.EnsureEnd(@"\") + @"conf\managed-schema");
      XmlElement newField = doc.CreateElement("field");
      newField.SetAttribute("name", "_uniqueid");
      newField.SetAttribute("type", "string");
      newField.SetAttribute("indexed", "true");
      newField.SetAttribute("required", "true");
      newField.SetAttribute("stored", "true");
      XmlNode schemaNode = doc.SelectSingleNode("/schema");

      if (schemaNode != null)
      {
        schemaNode.AppendChild(newField);
      }

      XmlNode uniqueKeyNode = doc.SelectSingleNode("/schema/uniqueKey");
      if (uniqueKeyNode != null)

      {
        uniqueKeyNode.InnerText = "_uniqueid";
      }

      doc.Save(newCorePath.EnsureEnd(@"\") + @"conf\managed-schema");
    }
  }
}
