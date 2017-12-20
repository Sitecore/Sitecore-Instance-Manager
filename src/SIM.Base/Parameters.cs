namespace SIM
{
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public class Parameters
  {
    [NotNull]
    private readonly string[] _Array;

    private Parameters([NotNull] string paramString)
    {
      Assert.ArgumentNotNull(paramString, nameof(paramString));

      _Array = paramString.Split(":|;".ToCharArray());
    }

    [NotNull]
    public string this[int number]
    {
      get
      {
        if (_Array.Length > number)
        {
          return _Array[number] ?? string.Empty;
        }

        return string.Empty;
      }
    }

    [NotNull]
    public static Parameters Parse([NotNull] string paramString)
    {
      Assert.ArgumentNotNull(paramString, nameof(paramString));

      return new Parameters(paramString);
    }

    [NotNull]
    public string[] Skip(int count)
    {
      if (_Array.Length > count)
      {
        return _Array.Skip(count).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
      }

      return new string[0];
    }
  }
}
