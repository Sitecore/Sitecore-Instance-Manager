namespace SIM.Client
{
  using System;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public class DummyLogProvider : ILogProvider
  {
    public void Debug([CanBeNull] Type ownerType, [CanBeNull] string message)
    {
    }

    public void Debug([CanBeNull] Type ownerType, [CanBeNull] Exception ex, [CanBeNull] string message)
    {
    }

    public void Info([CanBeNull] Type ownerType, [CanBeNull] string message)
    {
    }

    public void Info([CanBeNull] Type ownerType, [CanBeNull] Exception ex, [CanBeNull] string message)
    {
    }

    public void Warn([CanBeNull] Type ownerType, [CanBeNull] string message)
    {
    }

    public void Warn([CanBeNull] Type ownerType, [CanBeNull] Exception ex, [CanBeNull] string message)
    {
    }

    public void Error([CanBeNull] Type ownerType, [CanBeNull] string message)
    {
    }

    public void Error([CanBeNull] Type ownerType, [CanBeNull] Exception ex, [CanBeNull] string message)
    {
    }

    public void Fatal([CanBeNull] Type ownerType, [CanBeNull] string message)
    {
    }

    public void Fatal([CanBeNull] Type ownerType, [CanBeNull] Exception ex, [CanBeNull] string message)
    {
    }
  }
}