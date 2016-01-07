namespace SIM.Client
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public static class NullExtensions
  {
    [NotNull]
    public static T NotNull<T>([CanBeNull] this T obj, [CanBeNull] string message = null) where T : class
    {
      Assert.IsNotNull(obj, message ?? string.Format("The given instance of {0} is not expected to be null", typeof(T).FullName));

      return obj;
    }
  }
}