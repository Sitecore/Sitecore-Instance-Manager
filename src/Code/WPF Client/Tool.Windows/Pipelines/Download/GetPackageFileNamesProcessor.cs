using SIM.Base;
using SIM.Pipelines.Processors;

namespace SIM.Tool.Windows.Pipelines.Download
{
  public class GetPackageFileNamesProcessor : Processor
  {
    protected override void Process(ProcessorArgs args)
    {
      var download = (DownloadArgs)args;

      foreach (var link in download.Links)
      {
        download.FileNames[link] = WebRequestHelper.GetFileName(link, download.Cookies);
      }
    }    
  }
}
