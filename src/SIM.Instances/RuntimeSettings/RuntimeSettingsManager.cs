namespace SIM.Instances.RuntimeSettings
{
  public class RuntimeSettingsManager
  {
    #region Public methods

    public static RuntimeSettingsAccessor GetCachingSettingsAccessor(Instance instance)
    {
      return new RuntimeSettingsAccessor(instance);
    }

    public static RuntimeSettingsAccessor GetRealtimeSettingsAccessor(Instance instance)
    {
      return new RuntimeSettingsAccessor(instance);
    }

    #endregion
  }
}