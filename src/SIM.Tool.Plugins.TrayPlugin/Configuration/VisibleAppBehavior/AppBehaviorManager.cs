namespace SIM.Tool.Plugins.TrayPlugin.Configuration.VisibleAppBehavior
{
  public static class AppBehaviorManager
  {
    #region Public properties

    public static AppBehavior ActualBehavior { get; private set; }

    #endregion

    #region Public methods

    public static void ChangeBehavior(AppBehavior newBehavior)
    {
      if (ActualBehavior == newBehavior)
      {
        return;
      }

      if (ActualBehavior != null)
      {
        ActualBehavior.Detach();
      }

      ActualBehavior = newBehavior;
      ActualBehavior.Attach();
    }

    public static void Initialize()
    {
      ActualBehavior = new DefaultAppBehavior();
      ActualBehavior.Attach();
    }

    #endregion
  }
}