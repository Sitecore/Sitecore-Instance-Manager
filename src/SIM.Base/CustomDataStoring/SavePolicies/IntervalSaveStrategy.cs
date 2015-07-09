using System.Timers;

namespace SIM.CustomDataStoring.SavePolicies
{
  /// <summary>
  ///   All the changes are saved in 5 second interval. Pay attention to the RubberDelaySaveStrategy, it might be more
  ///   preferable
  /// </summary>
  public class IntervalSaveStrategy : ISaveStrategy
  {
    #region Static constants, fields, properties and methods

    #region Fields

    private static volatile bool _changesArePresent = false;

    private static double _defaultInterval = 5000;

    #endregion

    #region Constructors

    static IntervalSaveStrategy()
    {
      HeartBeat = new Timer(_defaultInterval);
      HeartBeat.AutoReset = true;
      HeartBeat.Elapsed += Beat;
    }

    #endregion

    #region Private properties

    private static Timer HeartBeat { get; set; }

    #endregion

    #region Public methods

    public static void SetNewInterval(double milliseconds)
    {
      HeartBeat.Interval = milliseconds;
    }

    #endregion

    #region Protected methods

    protected static void Beat(object sender, ElapsedEventArgs e)
    {
      if (_changesArePresent)
      {
        _changesArePresent = false;
        PermanentDataManager.SaveNow();
      }
    }

    #endregion

    #endregion

    #region ISaveStrategy Members

    public void HandleBoxChange()
    {
      _changesArePresent = true;
      if (!HeartBeat.Enabled)
      {
        HeartBeat.Start();
      }
    }

    #endregion
  }
}