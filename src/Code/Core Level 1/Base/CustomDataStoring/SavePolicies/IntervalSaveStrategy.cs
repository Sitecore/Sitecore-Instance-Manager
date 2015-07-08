using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;

namespace SIM.Base.CustomDataStoring.SavePolicies
{
  /// <summary>
  /// All the changes are saved in 5 second interval. Pay attention to the RubberDelaySaveStrategy, it might be more preferable
  /// </summary>
  public class IntervalSaveStrategy : ISaveStrategy
  {
    #region Static constants, fields, properties and methods

    private static volatile bool _changesArePresent = false;

    private static double _defaultInterval = 5000;
    private static Timer HeartBeat { get; set; }


    static IntervalSaveStrategy()
    {
      HeartBeat = new Timer(_defaultInterval);
      HeartBeat.AutoReset = true;
      HeartBeat.Elapsed += Beat;
    }

    protected static void Beat(object sender, ElapsedEventArgs e)
    {
      if (_changesArePresent)
      {
        _changesArePresent = false;
        PermanentDataManager.SaveNow();
      }
    }

    public static void SetNewInterval(double milliseconds)
    {
      HeartBeat.Interval = milliseconds;
    }

    #endregion

    #region ISaveStrategy Members

    public void HandleBoxChange()
    {
      _changesArePresent = true;
      if (!HeartBeat.Enabled)
        HeartBeat.Start();
    }

    #endregion
  }
}