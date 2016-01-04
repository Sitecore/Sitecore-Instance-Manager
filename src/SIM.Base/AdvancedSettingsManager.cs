namespace SIM
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base;

  public static class AdvancedSettingsManager
  {
    #region Constructors

    static AdvancedSettingsManager()
    {
      SettingsStorage = new XmlBasedAdvancedSettingsStorage();
      SettingsStorage.Initialize();
      RegisteredSettings = new Dictionary<string, AdvancedPropertyBase>();
    }

    #endregion

    #region Public Properties

    public static Dictionary<string, AdvancedPropertyBase> RegisteredSettings { get; set; }
    public static IAdvancedSettingsStorage SettingsStorage { get; set; }

    #endregion

    #region Public Methods and Operators

    public static AdvancedProperty<bool> CreateAndRegisterBoolSetting(string xPathKey, bool defaultValue)
    {
      return CreateAndRegisterGenericSetting(xPathKey, defaultValue, bool.TryParse);
    }


    public static AdvancedProperty<double> CreateAndRegisterDoubleSetting(string xPathKey, double defaultValue)
    {
      return CreateAndRegisterGenericSetting(xPathKey, defaultValue, double.TryParse);
    }

    public static AdvancedProperty<T> CreateAndRegisterGenericSetting<T>(string xPathKey, T defaultValue, RawPropertyValueParser<T> valueParser)
    {
      var property = new AdvancedProperty<T>(xPathKey, defaultValue, SettingsStorage, valueParser);
      RegisterGenericSetting(property);
      return property;
    }

    public static AdvancedProperty<int> CreateAndRegisterIntSetting(string xPathKey, int defaultValue)
    {
      return CreateAndRegisterGenericSetting(xPathKey, defaultValue, int.TryParse);
    }

    public static AdvancedProperty<long> CreateAndRegisterLongSetting(string xPathKey, long defaultValue)
    {
      return CreateAndRegisterGenericSetting(xPathKey, defaultValue, long.TryParse);
    }

    public static AdvancedProperty<string> CreateAndRegisterStringSetting(string xPathKey, string defaultValue)
    {
      return CreateAndRegisterGenericSetting(xPathKey, defaultValue, delegate(string value, out string result)
      {
        result = value;
        return true;
      });
    }

    public static void RegisterGenericSetting(AdvancedPropertyBase propertyToRegister)
    {
      Assert.IsTrue(!RegisteredSettings.ContainsKey(propertyToRegister.XPathKey), "Property with '{0}' key is already registered. Duplicates are not allowed".FormatWith(propertyToRegister.XPathKey));
      RegisteredSettings[propertyToRegister.XPathKey] = propertyToRegister;
    }

    #endregion
  }
}