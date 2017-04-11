namespace SIM
{
  #region

  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #endregion

  public class XmlElementEx
  {
    #region Fields

    [NotNull]
    public XmlDocumentEx Document { get; }

    [NotNull]
    public XmlElement Element { get; }

    #endregion

    #region Constructors

    public XmlElementEx([NotNull] XmlElement element, [NotNull] XmlDocumentEx document)
    {
      Assert.ArgumentNotNull(element, nameof(element));
      Assert.ArgumentNotNull(document, nameof(document));

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
      Assert.ArgumentNotNull(element, nameof(element));

      this.Element.AppendChild(element);
    }

    [NotNull]
    public XmlAttribute CreateAttribute([NotNull] string name, [CanBeNull] string value = null)
    {
      Assert.ArgumentNotNull(name, nameof(name));

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
      Assert.ArgumentNotNull(elementName, nameof(elementName));

      return this.Document.CreateElement(string.Empty, elementName, string.Empty);
    }

    public void Save()
    {
      this.Document.Save();
    }

    #endregion
  }
}