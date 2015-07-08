namespace SIM.Base
{
  public static class AdvancedSettings
  {
    /// <summary>
    /// Creates and registers advanced setting stored in %AppData%/Sitecore/Sitecore Instance Manager/AdvancedSettings.xml file
    /// </summary>
    /// <typeparam name="T">Type of stored value of this setting</typeparam>
    /// <param name="xPathKey">Key of the setting in a form of xpath expression</param>
    /// <param name="defaultValue">Default value of the setting</param>
    /// <param name="valueParcer">Custom parser of the setting that deserializes string value into <typeparam name="T"/></param>
    /// <returns></returns>
    public static AdvancedProperty<T> Create<T>(string xPathKey, T defaultValue, RawPropertyValueParser<T> valueParcer)
    {
      return AdvancedSettingsManager.CreateAndRegisterGenericSetting(xPathKey, defaultValue, valueParcer);
    }

    /// <summary>
    /// Creates and registers String advanced setting stored in %AppData%/Sitecore/Sitecore Instance Manager/AdvancedSettings.xml file
    /// </summary>
    /// <param name="xPathKey">Key of the setting in a form of xpath expression</param>
    /// <param name="defaultValue">Default value of the setting</param>
    /// <returns></returns>
    public static AdvancedProperty<string> Create(string xPathKey, string defaultValue)
    {
      return AdvancedSettingsManager.CreateAndRegisterGenericSetting(xPathKey, defaultValue, AdvancedSettings.TryParse);
    }

    /// <summary>
    /// Creates and registers Boolean advanced setting stored in %AppData%/Sitecore/Sitecore Instance Manager/AdvancedSettings.xml file
    /// </summary>
    /// <param name="xPathKey">Key of the setting in a form of xpath expression</param>
    /// <param name="defaultValue">Default value of the setting</param>
    /// <returns></returns>
    public static AdvancedProperty<bool> Create(string xPathKey, bool defaultValue)
    {
      return AdvancedSettingsManager.CreateAndRegisterGenericSetting(xPathKey, defaultValue, bool.TryParse);
    }

    /// <summary>
    /// Creates and registers Boolean advanced setting stored in %AppData%/Sitecore/Sitecore Instance Manager/AdvancedSettings.xml file
    /// </summary>
    /// <param name="xPathKey">Key of the setting in a form of xpath expression</param>
    /// <param name="defaultValue">Default value of the setting</param>
    /// <returns></returns>
    public static AdvancedProperty<int> Create(string xPathKey, int defaultValue)
    {
      return AdvancedSettingsManager.CreateAndRegisterGenericSetting(xPathKey, defaultValue, int.TryParse);
    }

    private static bool TryParse(this string str, out string result)
    {
      result = str;
      return true;
    }
  }
}