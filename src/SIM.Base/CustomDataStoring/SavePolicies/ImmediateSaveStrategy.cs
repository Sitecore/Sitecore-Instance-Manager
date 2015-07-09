namespace SIM.CustomDataStoring.SavePolicies
{
  /// <summary>
  ///   All the changes which are performed inside box are saved immediately. Can cause delays in case of frequent changes.
  /// </summary>
  public class ImmediateSaveStrategy : ISaveStrategy
  {
    #region ISaveStrategy Members

    public virtual void HandleBoxChange()
    {
      PermanentDataManager.SaveNow();
    }

    #endregion
  }
}