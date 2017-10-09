namespace SIM
{
  using System;

  public static class EventHelper
  {
    #region Public methods

    public static void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> subscriptionHandler, object sender, TEventArgs args) where TEventArgs : EventArgs
    {
      if (subscriptionHandler != null)
      {
        subscriptionHandler(sender, args);
      }
    }

    #endregion
  }
}