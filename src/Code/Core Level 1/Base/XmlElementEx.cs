#region Usings

using System.Xml;

#endregion

namespace SIM.Base
{
  #region

  

  #endregion

  /// <summary>
  ///   The xml element ex.
  /// </summary>
  public class XmlElementEx
  {
    #region Fields

    /// <summary>
    ///   The document.
    /// </summary>
    [NotNull]
    public readonly XmlDocumentEx Document;

    /// <summary>
    ///   The element.
    /// </summary>
    [NotNull]
    public readonly XmlElement Element;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlElementEx"/> class.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    /// <param name="document">
    /// The document. 
    /// </param>
    public XmlElementEx([NotNull] XmlElement element, [NotNull] XmlDocumentEx document)
    {
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNull(document, "document");

      this.Document = document;
      this.Element = element;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Attributes.
    /// </summary>
    [NotNull]
    public XmlAttributeCollection Attributes
    {
      get
      {
        return this.Element.Attributes;
      }
    }

    /// <summary>
    ///   Gets Name.
    /// </summary>
    [NotNull]
    public string Name
    {
      get
      {
        return this.Element.Name;
      }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The append child.
    /// </summary>
    /// <param name="element">
    /// The element. 
    /// </param>
    public void AppendChild([NotNull] XmlElement element)
    {
      Assert.ArgumentNotNull(element, "element");

      this.Element.AppendChild(element);
    }

    /// <summary>
    /// The create attribute.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public XmlAttribute CreateAttribute([NotNull] string name, [CanBeNull] string value = null)
    {
      Assert.ArgumentNotNull(name, "name");

      XmlAttribute attribute = this.Document.CreateAttribute(name);
      if (value != null)
      {
        attribute.Value = value;
      }

      return attribute;
    }

    /// <summary>
    /// The create element.
    /// </summary>
    /// <param name="elementName">
    /// The element name. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public XmlElement CreateElement([NotNull] string elementName)
    {
      Assert.ArgumentNotNull(elementName, "elementName");

      return this.Document.CreateElement(string.Empty, elementName, string.Empty);
    }

    /// <summary>
    ///   The save.
    /// </summary>
    public void Save()
    {
      this.Document.Save();
    }

    #endregion
  }
}