using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SIM.Pipelines.Processors;

namespace SIM.Tool.Windows.Pipelines.Download
{
  public class DownloadArgs : ProcessorArgs
  {
    public readonly string Cookies;
    public readonly ReadOnlyCollection<Uri> Links;
    public readonly string LocalRepository;
    public readonly UriBasedCollection<long> Sizes;
    public readonly UriBasedCollection<string > FileNames = new UriBasedCollection<string>();

    public DownloadArgs(string cookies, ReadOnlyCollection<Uri> links, string localRepository, UriBasedCollection<long> sizes)
    {
      LocalRepository = localRepository;
      Sizes = sizes;
      Links = links;
      Cookies = cookies;
    }
  }                         

  public class UriBasedCollection<T> : Dictionary<Uri, T>
  {
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