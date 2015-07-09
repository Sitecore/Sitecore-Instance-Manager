namespace SIM.Tool.Windows.Pipelines.Download
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using SIM.Pipelines.Processors;

  public class DownloadArgs : ProcessorArgs
  {
    #region Fields

    public readonly string Cookies;
    public readonly UriBasedCollection<string> FileNames = new UriBasedCollection<string>();
    public readonly ReadOnlyCollection<Uri> Links;
    public readonly string LocalRepository;
    public readonly UriBasedCollection<long> Sizes;

    #endregion

    #region Constructors

    public DownloadArgs(string cookies, ReadOnlyCollection<Uri> links, string localRepository, UriBasedCollection<long> sizes)
    {
      this.LocalRepository = localRepository;
      this.Sizes = sizes;
      this.Links = links;
      this.Cookies = cookies;
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

    #endregion
  }
}