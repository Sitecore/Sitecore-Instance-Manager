namespace SIM
{
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class Parameters
  {
    [NotNull]
    private readonly string[] array;

    private Parameters([NotNull] string paramString)
    {
      Assert.ArgumentNotNull(paramString, "paramString");

      this.array = paramString.Split(":|;".ToCharArray());
    }

    [NotNull]
    public string this[int number]
    {
      get
      {
        if (this.array.Length > number)
        {
          return this.array[number] ?? string.Empty;
        }

        return string.Empty;
      }
    }

    [NotNull]
    public static Parameters Parse([NotNull] string paramString)
    {
      Assert.ArgumentNotNull(paramString, "paramString");

      return new Parameters(paramString);
    }

    [NotNull]
    public string[] Skip(int count)
    {
      if (this.array.Length > count)
      {
        return this.array.Skip(count).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
      }

      return new string[0];
    }
  }
}
