using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIM.Base
{
  public class XmlBasedAdvancedSettingsStorage : IAdvancedSettingsStorage
  {
    #region Fields

    protected readonly string XPathPrefix = "/settings/";

    #endregion

    #region Properties

    protected string FilePath
    {
      get { return Path.Combine(ApplicationManager.DataFolder, "AdvancedSettings.xml"); }
    }

    protected XmlDocumentEx UnderlyingDocument { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void Initialize()
    {
      if (!FileSystem.Local.File.Exists(FilePath))
      {
        FileSystem.Local.File.WriteAllText(FilePath, @"<settings version=""1.3"" />");
      }

      UnderlyingDocument = XmlDocumentEx.LoadFile(FilePath);
    }

    public virtual string ReadSetting(string key, string defaultValue)
    {
      string normalizedXPathKey = this.NormalizeSettingKey(key);
      var xmlValue = UnderlyingDocument.SelectSingleElement(normalizedXPathKey).With(element => element.InnerText);
      return xmlValue.IsNullOrEmpty() ? defaultValue : xmlValue;
    }

    public virtual void WriteSetting(string key, string value)
    {
      string normalizedXPathKey = this.NormalizeSettingKey(key);
      if (value.IsNullOrEmpty())
      {
        var settingElement = UnderlyingDocument.SelectSingleElement(normalizedXPathKey);
        if (settingElement != null)
          settingElement.ParentNode.RemoveChild(settingElement);
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