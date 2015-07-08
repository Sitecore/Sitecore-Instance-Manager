namespace SIM.Tool.Windows.Pipelines.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Base;
  using SIM.Pipelines.Processors;

  public class Download8Args : ProcessorArgs
  {
    [NotNull]
    public readonly string Cookies;

    [NotNull]
    public readonly ReadOnlyCollection<Uri> Links;

    [NotNull]
    public readonly string LocalRepository;

    [NotNull]
    public readonly UriBasedCollection<string> FileNames;

    public Download8Args([NotNull] string cookies, [NotNull] ReadOnlyCollection<Uri> links, [NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(cookies, "cookies");
      Assert.ArgumentNotNull(links, "links");
      Assert.ArgumentNotNull(localRepository, "localRepository");
      this.LocalRepository = localRepository;
      this.Links = links;
      this.Cookies = cookies;
      this.FileNames = new UriBasedCollection<string>(links.ToDictionary(x => x, x => WebRequestHelper.GetFileName(x, cookies)));
    }
  }                         

  public class UriBasedCollection<T> : Dictionary<Uri, T>
  {
    public UriBasedCollection(IDictionary<Uri, T> dictionary) : base(dictionary)
    {
    }

    public new T this [Uri index]
    {
      get { return base[index]; }
      set
      {
        if (!this.ContainsKey(index))
        {
          this.Add(index, value);
        }
        else
        {
          base[index] = value;
        }
      }
    }
  }
}