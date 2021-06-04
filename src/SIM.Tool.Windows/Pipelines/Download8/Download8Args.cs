namespace SIM.Tool.Windows.Pipelines.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Linq;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  public class Download8Args : ProcessorArgs
  {
    #region Fields

    [CanBeNull]
    public string Cookies { get; }

    [NotNull]
    public readonly UriBasedCollection<string> _FileNames;

    [NotNull]
    public readonly ReadOnlyCollection<Uri> _Links;

    [NotNull]
    public string LocalRepository { get; }

    #endregion

    #region Constructors

    public Download8Args([CanBeNull] string cookies, [NotNull] ReadOnlyCollection<Uri> links, [NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(links, nameof(links));
      Assert.ArgumentNotNull(localRepository, nameof(localRepository));
      LocalRepository = localRepository;
      _Links = links;
      Cookies = cookies;
      _FileNames = new UriBasedCollection<string>(links.ToDictionary(x => x, x => WebRequestHelper.GetFileName(x, cookies)));
    }

    #endregion
  }

  public class UriBasedCollection<T> : Dictionary<Uri, T>
  {
    #region Constructors

    public UriBasedCollection(IDictionary<Uri, T> dictionary) : base(dictionary)
    {
    }

    #endregion

    #region Public properties

    public new T this[Uri index]
    {
      get
      {
        return base[index];
      }

      set
      {
        if (!ContainsKey(index))
        {
          Add(index, value);
        }
        else
        {
          base[index] = value;
        }
      }
    }

    #endregion
  }
}