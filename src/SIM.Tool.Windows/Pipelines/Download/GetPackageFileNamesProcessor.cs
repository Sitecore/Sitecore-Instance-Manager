namespace SIM.Tool.Windows.Pipelines.Download
{
  using SIM.Pipelines.Processors;

  public class GetPackageFileNamesProcessor : Processor
  {
    #region Protected methods

    protected override void Process(ProcessorArgs args)
    {
      var download = (DownloadArgs)args;

      foreach (var link in download._Links)
      {
        download._FileNames[link] = WebRequestHelper.GetFileName(link, download.Cookies);
      }
    }

    #endregion
  }
}