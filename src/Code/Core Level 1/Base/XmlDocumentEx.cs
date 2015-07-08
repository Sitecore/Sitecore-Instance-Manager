#region Usings

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;

#endregion

namespace SIM.Base
{
  #region



  #endregion

  /// <summary>
  ///   The xml document ex.
  /// </summary>
  public class XmlDocumentEx : XmlDocument
  {
    public XmlDocumentEx()
    {
    }

    public XmlDocumentEx([NotNull] string filePath)
    {
      Assert.ArgumentNotNull(filePath, "filePath");

      this.FilePath = filePath;
      this.Load(filePath);
    }

    #region Properties

    /// <summary>
    ///   Gets or sets FilePath.
    /// </summary>
    public string FilePath { get; protected set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The load.
    /// </summary>
    /// <param name="filename">
    /// The filename. 
    /// </param>
    public override sealed void Load([NotNull] string filename)
    {
      Assert.ArgumentNotNull(filename, "filename");

      this.FilePath = filename;
      base.Load(filename);
    }

    /// <summary>
    ///   The save.
    /// </summary>
    public void Save()
    {
      Assert.IsNotNull(this.FilePath.EmptyToNull(), "FilePath is empty");

      this.Save(this.FilePath);
    }

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
        Log.Warn("Cannot load xml: {0}. {1}\r\n{2}".FormatWith(xml, ex.Message, Environment.StackTrace), typeof(XmlDocumentEx), ex);
        return null;
      }
    }

    #endregion

    public bool Exists
    {
      get
      {
        return FileSystem.Local.File.Exists(this.FilePath);
      }
    }

    #region Nested type: FileIsMissingException

    /// <summary>
    ///   The file is missing exception.
    /// </summary>
    public class FileIsMissingException : Exception
    {
      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="FileIsMissingException"/> class.
      /// </summary>
      /// <param name="message">
      /// The message. 
      /// </param>
      public FileIsMissingException(string message)
        : base(message)
      {
      }

      #endregion
    }

    #endregion

    public static XmlDocumentEx LoadFile(string path)
    {
      Assert.ArgumentNotNull(path, "path");
      if (!FileSystem.Local.File.Exists(path))
      {
        throw new FileIsMissingException("The " + path + " doesn't exists");
      }

      var document = new XmlDocumentEx { FilePath = path };
      document.Load(path);
      return document;
    }
    
    // returns itself after applying changes
    public XmlDocumentEx Merge(XmlDocument target)
    {
      Assert.ArgumentNotNull(target, "target");
      var root = this.DocumentElement.IsNotNull("The DocumentElement is missing");
      var importedRoot = target.DocumentElement.IsNotNull("The DocumentElement of imported xml is missing");
      Merge(root, importedRoot);

      return this;
    }

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
          foreach (XmlAttribute attribute in child.Attributes)
          {
            var newAttribute = source.OwnerDocument.CreateAttribute(attribute.Prefix, attribute.LocalName, attribute.NamespaceURI);
            newAttribute.Value = attribute.Value;
            newChild.Attributes.Append(newAttribute);
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

    public void SetElementValue(string xpath, string value)
    {
      //Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
      XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));
      
      if (element != null)
      {
        element.InnerText = value;
        return;
      }

      var segments = xpath.Split('/').Where(w => !string.IsNullOrEmpty(w)).ToArray();
      
      string path = Prefix.TrimEnd("/");
      element = this.DocumentElement;
      for (int i = 1; i < segments.Length; ++i)
      {
        var segment = segments[i];
        path += "/" + segment;
        var newElement = this.SelectSingleElement(Prefix + path);
        if (newElement == null)
        {
          newElement = this.CreateElement(segment);
          element.AppendChild(newElement);
        }

        element = newElement;
      }

      element.InnerText = value;
    }

    public string GetElementAttributeValue(string xpath, string attributeName)
    {
        //Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
        XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));
        if (element != null && element.Attributes[attributeName] != null)
        {            
                return element.Attributes[attributeName].Value;
        }
        return "";
    }

    public void SetElementAttributeValue(string xpath, string attributeName, string value)
    {
        //Assert.IsTrue(xpath[0] == '/', "The xpath expression must be rooted");
        XmlElement element = this.SelectSingleElement(FixNotRootedXpathExpression(xpath));
        if(element != null && element.Attributes[attributeName] != null)
            element.Attributes[attributeName].Value = value;
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

    public static string Normalize(string xml)
    {
      var doc = XmlDocumentEx.LoadXml(xml);
      var stringWriter = new StringWriter(new StringBuilder());
      var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };
      doc.Save(xmlTextWriter);
      return stringWriter.ToString();
    }

  }
}