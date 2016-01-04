namespace SIM
{
  #region

  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class XmlElementEx
  {
    #region Fields

    [NotNull]
    public readonly XmlDocumentEx Document;

    [NotNull]
    public readonly XmlElement Element;

    #endregion

    #region Constructors

    public XmlElementEx([NotNull] XmlElement element, [NotNull] XmlDocumentEx document)
    {
      Assert.ArgumentNotNull(element, "element");
      Assert.ArgumentNotNull(document, "document");

      this.Document = document;
      this.Element = element;
    }

    #endregion

    #region Properties

    [NotNull]
    public XmlAttributeCollection Attributes
    {
      get
      {
        return this.Element.Attributes;
      }
    }

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

    public void AppendChild([NotNull] XmlElement element)
    {
      Assert.ArgumentNotNull(element, "element");

      this.Element.AppendChild(element);
    }

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

    [NotNull]
    public XmlElement CreateElement([NotNull] string elementName)
    {
      Assert.ArgumentNotNull(elementName, "elementName");

      return this.Document.CreateElement(string.Empty, elementName, string.Empty);
    }

    public void Save()
    {
      this.Document.Save();
    }

    #endregion
  }
}