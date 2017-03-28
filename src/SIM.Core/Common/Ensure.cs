namespace SIM.Core.Common
{
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public static class Ensure
  {
    [ContractAnnotation("obj:null=>stop")]
    [StringFormatMethod("message")]
    public static void IsNotNull(object obj, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

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

    [ContractAnnotation("str:null=>stop")]
    [StringFormatMethod("message")]
    public static void IsNotNullOrEmpty(string str, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

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

    [ContractAnnotation("condition:false=>stop")]
    [StringFormatMethod("message")]
    public static void IsTrue(bool condition, string message, params object[] args)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

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