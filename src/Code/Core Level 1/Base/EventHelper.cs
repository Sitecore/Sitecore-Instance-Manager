using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Base
{
  public static class EventHelper
  {
    public static void RaiseEvent<TEventArgs>(EventHandler<TEventArgs> subscriptionHandler, object sender, TEventArgs args) where TEventArgs : EventArgs
    {
      if (subscriptionHandler != null)
        subscriptionHandler(sender, args);
    }

    public static void RaiseEvent(EventHandler subscriptionHandler, object sender)
    {
      if (subscriptionHandler != null)
        subscriptionHandler(sender, EventArgs.Empty);
    }
  }
}
