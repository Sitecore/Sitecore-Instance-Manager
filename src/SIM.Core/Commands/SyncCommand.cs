namespace SIM.Core.Commands
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class SyncCommand : AbstractMultiInstanceActionCommand
  {
    [CanBeNull]
    public virtual string Targets { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Ignore { get; [UsedImplicitly] set; }

    protected override void DoExecute([NotNull] IReadOnlyList<Instance> instances, CommandResult result)
    {
      Assert.ArgumentNotNull(instances, nameof(instances));
      Assert.ArgumentNotNullOrEmpty(Targets, nameof(Targets));

      var ignore = Ignore?.Replace("/", "\\").Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
      var targetRoots = GetInstances(Targets)
        .Select(x => x.WebRootPath)
        .ToArray();

      var websiteFolders = instances.Select(x => x.WebRootPath).ToArray();
      foreach (var websiteFolder in websiteFolders)
      {
        var watcher = new FileSystemWatcher
        {
          Path = websiteFolder,
          Filter = "*.*",
          EnableRaisingEvents = true,
          NotifyFilter = NotifyFilters.LastWrite,
          IncludeSubdirectories = true
        };

        watcher.Created += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
        watcher.Changed += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
        watcher.Deleted += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
      }

      Console.WriteLine("Waiting for file system changes...");
      Console.WriteLine();
      Console.WriteLine("[Press any key to abort]");
      Console.ReadKey();
    }

    private void Update([CanBeNull] FileSystemEventArgs args, [NotNull] string sourceRoot, [NotNull] string[] targetRoots, [NotNull] string[] ignore)
    {
      Assert.ArgumentNotNullOrEmpty(sourceRoot, nameof(sourceRoot));
      Assert.ArgumentNotNull(targetRoots, nameof(targetRoots));
      Assert.ArgumentNotNull(ignore, nameof(ignore));

      var sourcePath = args?.FullPath;
      Assert.IsNotNull(sourcePath, nameof(sourcePath));

      if (sourcePath.Contains("\\.git\\") || sourcePath.EndsWith("\\.git"))
      {
        return;
      }

      if (ignore.Any(x => sourcePath.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0))
      {
        return;
      }

      if (!File.Exists(sourcePath))
      {
        return;
      }

      var isDeleted = args.ChangeType == WatcherChangeTypes.Deleted;
      Console.WriteLine(isDeleted ? $"{DateTime.Now:HH:mm:ss} DELETED: {sourcePath}" : $"{DateTime.Now:HH:mm:ss} CHANGED: {sourcePath}");

      var retryList = new List<string>();
      var virtualPath = sourcePath.Substring((sourceRoot.TrimEnd('\\') + "\\").Length);
      foreach (var targetRoot in targetRoots)
      {
        var targetPath = Path.Combine(targetRoot, virtualPath);
        if (Path.GetFullPath(targetRoot).Equals(sourcePath))
        {
          // this can be when syncing A;B => B;C
          continue;
        }

        try
        {
          File.Copy(sourcePath, targetPath, true);
          Console.WriteLine($"{DateTime.Now:HH:mm:ss} +COPIED: {targetPath}");
        }
        catch
        {
          retryList.Add(targetPath);
          Console.WriteLine($"{DateTime.Now:HH:mm:ss} ?FAILED: {targetPath}");
        }
      }

      for (var i = 0; i < 10 && retryList.Any(); ++i)
      {
        Thread.Sleep(1000);
        var targetPaths = retryList.ToArray();
        foreach (var targetPath in targetPaths)
        {
          try
          {
            if (isDeleted)
            {
              File.Delete(targetPath);
              Console.WriteLine($"{DateTime.Now:HH:mm:ss} -KILLED: {targetPath}");
            }
            else
            {
              File.Copy(sourcePath, targetPath, true);
              Console.WriteLine($"{DateTime.Now:HH:mm:ss} +COPIED: {targetPath}");
            }
            retryList.Remove(targetPath);
          }
          catch
          {
          }
        }
      }

      foreach (var targetPath in retryList)
      {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} ! FATAL: {targetPath}");
      }

      Console.WriteLine();
    }
  }
}