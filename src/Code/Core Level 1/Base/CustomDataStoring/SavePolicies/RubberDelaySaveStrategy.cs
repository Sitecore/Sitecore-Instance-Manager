using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SIM.Base.CustomDataStoring.SavePolicies
{
  /// <summary>
  /// This strategy saves changes after 5 seconds (by default) from the last change.
  /// </summary>
  public class RubberDelaySaveStrategy : ISaveStrategy
  {
    #region Static constants, fields, properties and methods

    private static Timer CountDown { get; set; }
    public static TimeSpan Delay { get; set; }

    static RubberDelaySaveStrategy()
    {
      Delay = TimeSpan.FromSeconds(10);
      CountDown = new Timer(CountdownHandler);
    }

    private static void CountdownHandler(object state)
    {
      PermanentDataManager.SaveNow();
    }

    #endregion

    #region ISaveStrategy Members

    public void HandleBoxChange()
    {
      CountDown.Change(Delay, TimeSpan.FromMilliseconds(-1));
    }

    #endregion
  }
}