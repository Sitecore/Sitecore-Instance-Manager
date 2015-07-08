using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Base.CustomDataStoring.SavePolicies
{
  /// <summary>
  /// All the changes which are performed inside box are saved immediately. Can cause delays in case of frequent changes.
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