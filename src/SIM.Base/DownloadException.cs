namespace SIM
{
  using System;

  public class DownloadException : Exception
  {
    internal DownloadException(string url, Exception ex) : base("Failed to download data from: " + url, ex)
    {
    }
  }
}