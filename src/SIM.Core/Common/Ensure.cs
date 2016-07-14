namespace SIM.Core.Common
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public static class Ensure
  {
    [StringFormatMethod("message")]
    public static void IsNotNull(object obj, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");

      if (obj != null)
      {
        return;
      }

      if (args == null || args.Length <= 0)
      {
        throw new MessageException(message);
      }
      throw new MessageException(string.Format(message, args));
    }

    [StringFormatMethod("message")]
    public static void IsNotNullOrEmpty(string str, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");

      if (!string.IsNullOrEmpty(str))
      {
        return;
      }

      if (args == null || args.Length <= 0)
      {
        throw new MessageException(message);
      }
      throw new MessageException(string.Format(message, args));
    }

    [StringFormatMethod("message")]
    public static void IsTrue(bool condition, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");

      if (condition)
      {
        return;
      }

      if (args == null || args.Length <= 0)
      {
        throw new MessageException(message);
      }
      throw new MessageException(string.Format(message, args));
    }
  }
}