using System;
using System.Threading;

namespace SIM.CustomDataStoring.SavePolicies
{
  /// <summary>
  ///   This strategy saves changes after 5 seconds (by default) from the last change.
  /// </summary>
  public class RubberDelaySaveStrategy : ISaveStrategy
  {
    #region Static constants, fields, properties and methods

    #region Constructors

    static RubberDelaySaveStrategy()
    {
      Delay = TimeSpan.FromSeconds(10);
      CountDown = new Timer(CountdownHandler);
    }

    #endregion

    #region Public properties

    public static TimeSpan Delay { get; set; }

    #endregion

    #region Private properties

    private static Timer CountDown { get; set; }

    #endregion

    #region Private methods

    private static void CountdownHandler(object state)
    {
      PermanentDataManager.SaveNow();
    }

    #endregion

    #endregion

    #region ISaveStrategy Members

    public void HandleBoxChange()
    {
      CountDown.Change(Delay, TimeSpan.FromMilliseconds(-1));
    }

    #endregion
  }
}