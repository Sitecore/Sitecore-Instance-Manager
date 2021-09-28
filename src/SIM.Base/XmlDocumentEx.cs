﻿namespace SIM
{
  #region

  using System;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #endregion

  public class XmlDocumentEx : XmlDocument
  {
    #region Constructors

    public XmlDocumentEx()
    {
    }

    public XmlDocumentEx([NotNull] string filePath)
    {
      Assert.ArgumentNotNull(filePath, nameof(filePath));

      FilePath = filePath;
      Load(filePath);
    }

    #endregion

    #region Properties

    public string FilePath { get; protected set; }

    #endregion

    #region Public Methods

    [CanBeNull]
    public new static XmlDocumentEx LoadXml(string xml)
    {
      try
      {
        XmlDocument doc = new XmlDocumentEx();
        doc.LoadXml(xml);
        return (XmlDocumentEx)doc;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot load xml: {xml}. {ex.Message}\r\n{Environment.StackTrace}");
        return null;
      }
    }
    
    [CanBeNull]
    public static XmlDocumentEx LoadStream(Stream stream)
    {
      try
      {
        XmlDocument doc = new XmlDocumentEx();
        doc.Load(stream);
        return (XmlDocumentEx)doc;
      }
      catch (Exception ex)
      {
        Log.Warn(ex, string.Format("Cannot load xml. {1}\r\n{2}", ex.Message, Environment.StackTrace));
        return null;
      }
    }

    public override sealed void Load([NotNull] string filename)
    {
      Assert.ArgumentNotNull(filename, nameof(filename));

      FilePath = filename;
      base.Load(filename);
    }

    public void Save()
    {
      Assert.IsNotNull(FilePath.EmptyToNull(), "FilePath is empty");

      Save(FilePath);
    }

    #endregion

    #region Public properties

    #endregion

    #region Public methods

    [NotNull]
    public static XmlDocumentEx LoadFile([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));
      if (!FileSystem.FileSystem.Local.File.Exists(path))
      {
        throw new FileIsMissingException("The " + path + " doesn't exists");
      }

      var document = new XmlDocumentEx
      {
        FilePath = path
      };
      document.Load(path);
      return document;
    }

    [CanBeNull]
    public static XmlDocumentEx LoadFileSafe([NotNull] string path)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      if (!FileSystem.FileSystem.Local.File.Exists(path))
      {
        return null;
      }

      var document = new XmlDocumentEx
      {
        FilePath = path
      };

      document.Load(path);
      return document;
    }

    public static string Normalize(string xml)
    {
      var doc = LoadXml(xml);
      var stringWriter = new StringWriter(new StringBuilder());
      var xmlTextWriter = new XmlTextWriter(stringWriter)
      {
        Formatting = Formatting.Indented
      };
      doc.Save(xmlTextWriter);
      return stringWriter.ToString();
    }

    // returns itself after applying changes
    public string GetElementAttributeValue(string xpath, string attributeName)
    {
      // Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
      XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));
      if (element != null && element.Attributes[attributeName] != null)
      {
        return element.Attributes[attributeName].Value;
      }

      return string.Empty;
    }

    public XmlDocumentEx Merge(XmlDocument target)
    {
      Assert.ArgumentNotNull(target, nameof(target));
      var root = DocumentElement.IsNotNull("The DocumentElement is missing");
      var importedRoot = target.DocumentElement.IsNotNull("The DocumentElement of imported xml is missing");
      Merge(root, importedRoot);

      return this;
    }

    public void SetElementAttributeValue(string xpath, string attributeName, string value)
    {
      // Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
      XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));
      if (element != null && element.Attributes[attributeName] != null)
      {
        element.Attributes[attributeName].Value = value;
      }
    }

    public void SetElementValue(string xpath, string value)
    {
      // Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
      XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));

      if (element != null)
      {
        if (value.IsNullOrEmpty()) return;

        element.InnerText = value;
        return;
      }

      var segments = xpath.Split('/').Where(w => !string.IsNullOrEmpty(w)).ToArray();

      var path = Prefix.TrimEnd("/");
      element = DocumentElement;
      for (int i = 1; i < segments.Length; ++i)
      {
        var segment = segments[i];
        path += "/" + segment;
        var newElement = this.SelectSingleElement(Prefix + path);
        if (newElement == null)
        {
          newElement = CreateElement(segment);
          element.AppendChild(newElement);
        }

        element = newElement;
      }

      if (value.IsNullOrEmpty()) return;

      element.InnerText = value;
    }

    public bool ElementAttributeContains(string xpath, string attributeName, string substring)
    {
      string value = this.GetElementAttributeValue(xpath, attributeName).ToLower();
      return !string.IsNullOrEmpty(value) && value.Contains(substring.ToLower());
    }

    #endregion

    #region Private methods

    private static void Merge(XmlElement source, XmlElement target, XmlElement[] elements = null)
    {
      foreach (var child in elements ?? target.ChildNodes.OfType<XmlElement>().ToArray())
      {
        var existingChild =
          source.ChildNodes.OfType<XmlElement>()
            .FirstOrDefault(
              c => child.LocalName == c.LocalName && child.NamespaceURI == c.NamespaceURI &&
                   child.Attributes.OfType<XmlAttribute>()
                     .All(attr => c.Attributes[attr.LocalName, attr.NamespaceURI].With(a => a.Value) == attr.Value));
        var newChild = existingChild ?? source.OwnerDocument.CreateNode(child.NodeType, child.Name, string.Empty) as XmlElement;
        if (existingChild == null)
        {
          foreach (XmlAttribute attribute in child.Attributes)
          {
            var newAttribute = source.OwnerDocument.CreateAttribute(attribute.Prefix, attribute.LocalName, attribute.NamespaceURI);
            newAttribute.Value = attribute.Value;
            newChild.Attributes.Append(newAttribute);
          }
        }

        // to avoid double enumerating
        var xmlElements = child.ChildNodes.OfType<XmlElement>().ToArray();
        if (xmlElements.Length == 0 && !string.IsNullOrEmpty(child.InnerText))
        {
          newChild.InnerText = child.InnerText;
        }
        else
        {
          Merge(newChild, child, xmlElements);
        }

        source.AppendChild(newChild);
      }
    }


    private string FixNotRootedXpathExpression(string xpathExpr)
    {
      if (xpathExpr[0] == '/')
      {
        return xpathExpr;
      }
      else
      {
        return @"/" + xpathExpr;
      }
    }

    #endregion

    #region Nested type: FileIsMissingException

    public class FileIsMissingException : Exception
    {
      #region Constructors

      public FileIsMissingException(string message)
        : base(message)
      {
      }

      #endregion
    }

    #endregion

    public string ToPrettyXmlString()
    {
      var sw = new StringWriter();
      var xml = new XmlTextWriter(sw);
      xml.Formatting = Formatting.Indented;
      xml.Indentation = 2;
      xml.IndentChar = ' ';
      Save(xml);

      return sw.ToString();
    }
    
  }
}