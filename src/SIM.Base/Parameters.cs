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

      this._Array = paramString.Split(":|;".ToCharArray());
    }

    [NotNull]
    public string this[int number]
    {
      get
      {
        if (this._Array.Length > number)
        {
          return this._Array[number] ?? string.Empty;
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
      if (this._Array.Length > count)
      {
        return this._Array.Skip(count).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
      }

      return new string[0];
    }
  }
}
