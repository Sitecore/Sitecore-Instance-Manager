namespace SIM
{
  public abstract class AdvancedPropertyBase
  {
    #region Constructors

    protected AdvancedPropertyBase(string xPathKey, IAdvancedSettingsStorage settingsStorage)
    {
      XPathKey = xPathKey;
      SettingsStorage = settingsStorage;
    }

    #endregion

    #region Public Properties

    public abstract bool IsRawUserValueValid { get; }

    public string Name
    {
      get
      {
        return XPathKey;
      }
    }

    public abstract string RawDefaultValue { get; }

    public abstract string RawUserValue { get; set; }

    public string XPathKey { get; set; }

    public bool HasUserValue
    {
      get
      {
        return RawUserValue != null;
      }
    }

    #endregion

    #region Properties

    protected IAdvancedSettingsStorage SettingsStorage { get; set; }

    #endregion

    #region Methods

    protected virtual string ReadSetting()
    {
      return SettingsStorage.ReadSetting(XPathKey, null);
    }

    protected virtual void WriteSetting(string newValue)
    {
      SettingsStorage.WriteSetting(XPathKey, newValue);
    }

    #endregion
  }
}