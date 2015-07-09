namespace SIM
{
  public interface IAdvancedSettingsStorage
  {
    #region Public methods

    void Initialize();
    string ReadSetting(string key, string defaultValue);
    void WriteSetting(string key, string value);

    #endregion
  }
}