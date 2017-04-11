namespace SIM.Tool.Windows.Pipelines.Download
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using SIM.Pipelines.Processors;

  public class DownloadArgs : ProcessorArgs
  {
    #region Fields

    public string Cookies { get; }
    public readonly UriBasedCollection<string> _FileNames = new UriBasedCollection<string>();
    public readonly ReadOnlyCollection<Uri> _Links;
    public string LocalRepository { get; }
    public readonly UriBasedCollection<long> _Sizes;

    #endregion

    #region Constructors

    public DownloadArgs(string cookies, ReadOnlyCollection<Uri> links, string localRepository, UriBasedCollection<long> sizes)
    {
      LocalRepository = localRepository;
      _Sizes = sizes;
      _Links = links;
      Cookies = cookies;
    }

    #endregion
  }

  public class UriBasedCollection<T> : Dictionary<Uri, T>
  {
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