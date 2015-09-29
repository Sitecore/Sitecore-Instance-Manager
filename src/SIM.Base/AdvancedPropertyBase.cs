namespace SIM
{
  public abstract class AdvancedPropertyBase
  {
    #region Constructors

    protected AdvancedPropertyBase(string xPathKey, IAdvancedSettingsStorage settingsStorage)
    {
      this.XPathKey = xPathKey;
      this.SettingsStorage = settingsStorage;
    }

    #endregion

    #region Public Properties

    public abstract bool IsRawUserValueValid { get; }

    public string Name
    {
      get
      {
        return this.XPathKey;
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
      return this.SettingsStorage.ReadSetting(this.XPathKey, null);
    }

    protected virtual void WriteSetting(string newValue)
    {
      this.SettingsStorage.WriteSetting(this.XPathKey, newValue);
    }

    #endregion
  }
}