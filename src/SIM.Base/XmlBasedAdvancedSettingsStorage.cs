namespace SIM
{
  using System.IO;

  public class XmlBasedAdvancedSettingsStorage : IAdvancedSettingsStorage
  {
    #region Fields

    protected readonly string XPathPrefix = "/settings/";

    #endregion

    #region Properties

    protected string FilePath
    {
      get
      {
        return Path.Combine(ApplicationManager.DataFolder, "AdvancedSettings.xml");
      }
    }

    protected XmlDocumentEx UnderlyingDocument { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void Initialize()
    {
      if (!FileSystem.FileSystem.Local.File.Exists(this.FilePath))
      {
        FileSystem.FileSystem.Local.File.WriteAllText(this.FilePath, @"<settings version=""1.4"" />");
      }

      this.UnderlyingDocument = XmlDocumentEx.LoadFile(this.FilePath);
    }

    public virtual string ReadSetting(string key, string defaultValue)
    {
      string normalizedXPathKey = this.NormalizeSettingKey(key);
      var xmlValue = this.UnderlyingDocument.SelectSingleElement(normalizedXPathKey).With(element => element.InnerText);
      return xmlValue.IsNullOrEmpty() ? defaultValue : xmlValue;
    }

    public virtual void WriteSetting(string key, string value)
    {
      string normalizedXPathKey = this.NormalizeSettingKey(key);
      if (value.IsNullOrEmpty())
      {
        var settingElement = this.UnderlyingDocument.SelectSingleElement(normalizedXPathKey);
        if (settingElement != null)
        {
          settingElement.ParentNode.RemoveChild(settingElement);
        }
      }
      else
      {
        this.UnderlyingDocument.SetElementValue(normalizedXPathKey, value);
      }

      this.UnderlyingDocument.Save();
    }

    #endregion

    #region Methods

    protected virtual string NormalizeSettingKey(string originalKey)
    {
      return this.XPathPrefix + originalKey;
    }

    #endregion
  }
}