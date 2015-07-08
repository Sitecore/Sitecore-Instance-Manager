using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SIM.Pipelines.Processors;
using SIM.Base;
using SIM.Products;
using SIM.Tool.Base;

namespace SIM.Tool.Windows.Pipelines.Download
{
  public class DownloadProcessor : Processor
  {
    public override long EvaluateStepsCount(ProcessorArgs args)
    {  
      return ((DownloadArgs)args).Sizes.Sum(size => size.Value);
    }

    protected override void Process(ProcessorArgs args)
    {
      var download = (DownloadArgs)args;
      var cookies = download.Cookies;
      var localRepository = download.LocalRepository;
      var fileNames = download.FileNames;
      var links = download.Links;
      var fileSizes = download.Sizes;

      int parallelDownloadsNumber = WindowsSettings.AppDownloaderParallelThreads.Value;

      var cancellation = new CancellationTokenSource();
      var urls = links.Where(link => RequireDownloading(fileNames[link], fileSizes[link], localRepository)).ToArray();
      for (int i = 0; i < urls.Length; i += parallelDownloadsNumber)
      {
        int remains = urls.Length - i;
        var tasks = urls
          .Skip(i)
          .Take(Math.Min(parallelDownloadsNumber, remains))
          .Select(url => Task.Factory.StartNew(() => DownloadFile(url, fileNames[url], fileSizes[url], localRepository, cookies, cancellation.Token), cancellation.Token))
          .ToArray();
        
        try
        {
          Task.WaitAll(tasks, WindowsSettings.AppDownloaderTotalTimeout.Value * WebRequestHelper.Hour);
        }
        catch (Exception ex)
        {
          Log.Warn("An error occurred during downloading files", this, ex);

          cancellation.Cancel();
          throw;
        }
      }
    }

    private bool RequireDownloading(string fileName, long fileSize, string localRepository)
    {
      string filePath1 = ProductManager.Products.Select(product => product.PackagePath).FirstOrDefault(packagePath => Path.GetFileName(packagePath).EqualsIgnoreCase(fileName));      
      if (!string.IsNullOrEmpty(filePath1))
      {
        if (FileSystem.Local.File.Exists(filePath1))
        {
          if (FileSystem.Local.File.GetFileLength(filePath1) == fileSize)
          {
            Log.Info("Downloading is skipped, the {0} file already exists".FormatWith(filePath1), this);
            return false;
          }

          Log.Info("The {0} file already exists, but its size is incorrect - file will be removed".FormatWith(filePath1), this);
          FileSystem.Local.File.Delete(filePath1);
        }
      }

      var filePath2 = Path.Combine(localRepository, fileName);
      if (FileSystem.Local.File.Exists(filePath2))
      {
        if (FileSystem.Local.File.GetFileLength(filePath2) == fileSize)
        {
          Log.Info("Downloading is skipped, the {0} file already exists".FormatWith(filePath2), this);
          return false;
        }

        Log.Info("The {0} file already exists, but its size is incorrect - file will be removed".FormatWith(filePath2), this);
        FileSystem.Local.File.Delete(filePath2);
      }                    

      return true;
    }

    public void DownloadFile(Uri url, string fileName, long fileSize, string localRepository, string cookies, CancellationToken token)
    {      
      using (var response = WebRequestHelper.RequestAndGetResponse(url, null, null, cookies))
      {
        string destFileName = Path.Combine(localRepository, fileName);
        Assert.IsTrue(!FileSystem.Local.File.Exists(destFileName), "The {0} file already exists".FormatWith(destFileName));

        if (TryCopyFromExternalRepository(fileName, destFileName))
        {
          this.Controller.IncrementProgress(fileSize);
          return;
        }

        WebRequestHelper.DownloadFile(url, destFileName, response, token, count => this.Controller.IncrementProgress(count));
      }
    }

    private bool TryCopyFromExternalRepository(string fileName, string destFileName)
    {
      var externalRepositories = WindowsSettings.AppDownloaderExternalRepository.Value;
      if (!string.IsNullOrEmpty(externalRepositories))
      {
        try
        {
          foreach (var repository in externalRepositories.Split('|').Reverse())
          {
            var files = FileSystem.Local.Directory.GetFiles(repository, fileName, SearchOption.AllDirectories);
            var externalRepositoryFilePath = files.FirstOrDefault();
            if (!string.IsNullOrEmpty(externalRepositoryFilePath) && FileSystem.Local.File.Exists(externalRepositoryFilePath))
            {
              using (new ProfileSection("Copying file from remote repository", this))
              {
                ProfileSection.Argument("fileName", fileName);
                ProfileSection.Argument("externalRepositoryFilePath", externalRepositoryFilePath);

                WindowHelper.CopyFileUI(externalRepositoryFilePath, destFileName, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);                
              }

              Log.Info("Copying the {0} file has completed".FormatWith(fileName), this);
              return true;
            }
          }
        }
        catch (Exception ex)
        {
          Log.Warn("Unable to copy the {0} file from external repository".FormatWith(fileName), this, ex);
        }
      }

      return false;
    }
  }
}
