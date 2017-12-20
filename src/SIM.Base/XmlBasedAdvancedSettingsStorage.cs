namespace SIM
{
  using System.IO;
  using SIM.Extensions;

  public class XmlBasedAdvancedSettingsStorage : IAdvancedSettingsStorage
  {
    #region Fields

    protected string XPathPrefix { get; } = "/settings/";

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
      if (!FileSystem.FileSystem.Local.File.Exists(FilePath))
      {
        FileSystem.FileSystem.Local.File.WriteAllText(FilePath, @"<settings version=""1.4"" />");
      }

      UnderlyingDocument = XmlDocumentEx.LoadFile(FilePath);
    }

    public virtual string ReadSetting(string key, string defaultValue)
    {
      var normalizedXPathKey = NormalizeSettingKey(key);
      var xmlValue = UnderlyingDocument.SelectSingleElement(normalizedXPathKey).With(element => element.InnerText);
      return xmlValue.IsNullOrEmpty() ? defaultValue : xmlValue;
    }

    public virtual void WriteSetting(string key, string value)
    {
      var normalizedXPathKey = NormalizeSettingKey(key);
      if (value.IsNullOrEmpty())
      {
        var settingElement = UnderlyingDocument.SelectSingleElement(normalizedXPathKey);
        if (settingElement != null)
        {
          settingElement.ParentNode.RemoveChild(settingElement);
        }
      }
      else
      {
        UnderlyingDocument.SetElementValue(normalizedXPathKey, value);
      }

      UnderlyingDocument.Save();
    }

    #endregion

    #region Methods

    protected virtual string NormalizeSettingKey(string originalKey)
    {
      return XPathPrefix + originalKey;
    }

    #endregion
  }
}