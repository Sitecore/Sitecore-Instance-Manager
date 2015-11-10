namespace SIM.Tool.Windows.Pipelines.Download8
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using System.Threading;
  using Alienlab.NetExtensions;
  using SIM.Pipelines.Processors;
  using SIM.Products;
  using SIM.Tool.Base;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  [UsedImplicitly]
  public class Download8Processor : Processor
  {
    #region Constants

    private const int scale = 100;

    #endregion

    #region Public methods

    public override long EvaluateStepsCount(ProcessorArgs args)
    {
      var download = (Download8Args)args;
      var count = download.FileNames.Count(x => this.RequireDownloading(x.Value, download.LocalRepository));
      return count * scale;
    }

    #endregion

    #region Protected methods

    protected override void Process(ProcessorArgs args)
    {
      var download = (Download8Args)args;
      var cookies = download.Cookies;
      var localRepository = download.LocalRepository;
      var fileNames = download.FileNames;
      var links = download.Links;

      var cancellation = new CancellationTokenSource();
      var urls = links.Where(link => this.RequireDownloading(fileNames[link], localRepository)).ToArray();
      var n = urls.Length;
      for (int index = 0; index < n; index++)
      {
        var url = urls[index];
        try
        {
          var fileName = fileNames[url];
          fileName = FixFileName(fileName);

          var destFileName = Path.Combine(localRepository, fileName);
          Assert.IsTrue(!FileSystem.FileSystem.Local.File.Exists(destFileName), "The {0} file already exists".FormatWith(destFileName));
          Log.Info("Downloading {0}",  destFileName);

          if (this.TryCopyFromExternalRepository(fileName, destFileName))
          {
            this.Controller.SetProgress(index * scale + scale);
            return;
          }

          var downloadOptions = new DownloadOptions
          {
            Uri = url, 
            BlockOptions = new BlocksCount(WindowsSettings.AppDownloader8ParallelThreads.Value), 
            CancellationToken = cancellation.Token, 
            Cookies = cookies, 
            FilePath = destFileName, 
            RequestTimeout = new TimeSpan(0, WindowsSettings.AppDownloaderTotalTimeout.Value, 0, 0)
          };

          var done = false;
          Exception exception = null;
          var context = new DownloadContext(downloadOptions);
          context.OnProgressChanged += (x, y, z) => this.Controller.SetProgress(index * scale + z);
          context.OnDownloadCompleted += () => done = true;
          context.OnErrorOccurred += ex => exception = ex;
          ApplicationManager.AttemptToClose += (x, y) => cancellation.Cancel(true);
          context.DownloadAsync();
          while (!done && exception == null)
          {
            Thread.Sleep(1000);
          }

          if (exception != null)
          {
            cancellation.Cancel();
            throw new InvalidOperationException("Downloading file '{0}' failed with exception".FormatWith(url), exception);
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred during downloading files");

          cancellation.Cancel();
          throw new InvalidOperationException("An unhandled exception happened during downloading '{0}' file".FormatWith(url));
        }
      }
    }

    #endregion

    #region Private methods

    private static string FixFileName(string fileName)
    {
      var replacements = new[]
      {
        new[]
        {
          @"(\s\d)(\d\s)", "$1.$2"
        }, 
        new[]
        {
          @"rev\s+", "rev. "
        }
      };
      foreach (var replacement in replacements)
      {
        fileName = new Regex(replacement[0]).Replace(fileName, replacement[1]);
      }

      return fileName;
    }

    private bool RequireDownloading([NotNull] string fileName, [NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(fileName, "fileName");
      Assert.ArgumentNotNull(localRepository, "localRepository");
      fileName = FixFileName(fileName);
      string filePath1 = ProductManager.Products.Select(product => product.PackagePath).FirstOrDefault(packagePath => Path.GetFileName(packagePath).EqualsIgnoreCase(fileName));
      if (!string.IsNullOrEmpty(filePath1))
      {
        if (FileSystem.FileSystem.Local.File.Exists(filePath1))
        {
          Log.Info("Downloading is skipped, the {0} file already exists", filePath1);

          return false;
        }
      }

      return true;
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
            var files = FileSystem.FileSystem.Local.Directory.GetFiles(repository, fileName, SearchOption.AllDirectories);
            var externalRepositoryFilePath = files.FirstOrDefault();
            if (!string.IsNullOrEmpty(externalRepositoryFilePath) && FileSystem.FileSystem.Local.File.Exists(externalRepositoryFilePath))
            {
              using (new ProfileSection("Copying file from remote repository", this))
              {
                ProfileSection.Argument("fileName", fileName);
                ProfileSection.Argument("externalRepositoryFilePath", externalRepositoryFilePath);

                WindowHelper.CopyFileUI(externalRepositoryFilePath, destFileName, Microsoft.VisualBasic.FileIO.UIOption.AllDialogs, Microsoft.VisualBasic.FileIO.UICancelOption.ThrowException);
              }

              Log.Info("Copying the {0} file has completed", fileName);
              return true;
            }
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "Unable to copy the {0} file from external repository", fileName);
        }
      }

      return false;
    }

    #endregion
  }
}