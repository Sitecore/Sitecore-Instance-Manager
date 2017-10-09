namespace SIM.Instances.RuntimeSettings
{
  public class RuntimeSettingsManager
  {
    #region Public methods

    public static RuntimeSettingsAccessor GetRealtimeSettingsAccessor(Instance instance)
    {
      return new RuntimeSettingsAccessor(instance);
    }

    #endregion
  }
}