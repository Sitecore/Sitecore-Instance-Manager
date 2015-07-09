namespace SIM.CustomDataStoring
{
  /// <summary>
  ///   Entry point for permanent data storages. Use it to get root boxes and force saving procedure.
  /// </summary>
  public static class PermanentDataManager
  {
    #region Public Properties

    public static IDataBox PluginsData
    {
      get
      {
        return PermanentInfoStorage.GetRootDataBox().GetOrCreateSubBox("Plugins");
      }
    }

    public static IDataBox SIMCoreData
    {
      get
      {
        return PermanentInfoStorage.GetRootDataBox().GetOrCreateSubBox("SIM.Core");
      }
    }

    #endregion

    #region Public Methods and Operators

    public static bool SaveNow()
    {
      return PermanentInfoStorage.ForceSave();
    }

    #endregion
  }
}