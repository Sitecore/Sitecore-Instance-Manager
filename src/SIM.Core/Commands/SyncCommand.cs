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
  using SIM.IO;

  public class SyncCommand : AbstractMultiInstanceActionCommand
  {
    public SyncCommand([NotNull] IFileSystem fileSystem) : base(fileSystem)
    {
    }

    [CanBeNull]
    public virtual string Targets { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Ignore { get; [UsedImplicitly] set; }

    [NotNull]
    private HandlerBase OnChanged { get; } = new ChangedHandler();

    [NotNull]
    private HandlerBase OnDeleted { get; } = new DeletedHandler();

    [NotNull]
    private HandlerBase OnRenamed { get; } = new RenamedHandler();

    protected override void DoExecute([NotNull] IReadOnlyList<Instance> instances, CommandResult result)
    {
      Assert.ArgumentNotNull(instances, nameof(instances));
      Assert.ArgumentNotNullOrEmpty(Targets, nameof(Targets));

      var ignore = Ignore?.Replace("/", "\\").Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
      var targetRoots = GetInstances(Targets)
        .Select(x => x.WebRootPath)
        .ToArray();

      var websiteFolders = instances.Select(x => x.WebRootPath).ToArray();
      var watchers = new List<FileSystemWatcher>();
      foreach (var websiteFolder in websiteFolders)
      {
        var watcher = new FileSystemWatcher
        {
          Path = websiteFolder,
          Filter = "*.*",
          IncludeSubdirectories = true,
          InternalBufferSize = int.MaxValue / 2
        };

        watcher.Error += (sender, args) => Console.WriteLine($"{DateTime.Now:HH:mm:ss} ERRROR: {args.GetException()?.Message}");

        watcher.Created += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
        watcher.Changed += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
        watcher.Renamed += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);
        watcher.Deleted += (sender, args) => Update(args, websiteFolder, targetRoots, ignore);

        watcher.EnableRaisingEvents = true;
      }

      Console.WriteLine("Waiting for file system changes...");
      Console.WriteLine();
      Console.WriteLine("[Press any key to abort]");
      Console.ReadKey();
      
      watchers.ForEach(x => x?.Dispose());
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

      var fileName = Path.GetFileName(sourcePath);
      if (!fileName.Contains('.'))
      {
        // do not support files without dot
        return;
      }

      if (ignore.Any(x => sourcePath.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0))
      {
        return;
      }

      switch (args.ChangeType)
      {
        case WatcherChangeTypes.Deleted:
          OnDeleted.Process(args, sourceRoot, targetRoots);
          return;

        case WatcherChangeTypes.Renamed:
          OnRenamed.Process(args, sourceRoot, targetRoots);
          return;

        default:
          OnChanged.Process(args, sourceRoot, targetRoots);
          return;
      }      
    }

    private class ChangedHandler : HandlerBase
    {
      protected override bool PreProcess(FileSystemEventArgs args)
      {
        Assert.ArgumentNotNull(args, nameof(args));

        var sourcePath = args.FullPath;
        Assert.IsNotNull(sourcePath, nameof(sourcePath));         

        Console.WriteLine($"{DateTime.Now:HH:mm:ss} CHANGED: {sourcePath}");

        return true;
      }

      protected override void TryProcess(FileSystemEventArgs args, string sourceRoot, string targetRoot)
      {
        var relativePath = args.Name;
        Assert.IsNotNull(relativePath, nameof(relativePath));

        var targetPath = Path.Combine(targetRoot, relativePath);
        Assert.IsNotNull(relativePath, nameof(relativePath));

        var targetDir = Path.GetDirectoryName(targetPath);
        Directory.CreateDirectory(targetDir);

        var sourcePath = args.FullPath;
        Assert.IsNotNullOrEmpty(sourcePath, nameof(sourcePath));

        File.Copy(sourcePath, targetPath, true);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} +COPIED: {targetPath}");
      }
    }

    private class RenamedHandler : HandlerBase
    {
      protected override bool PreProcess(FileSystemEventArgs args)
      {
        Assert.ArgumentNotNull(args, nameof(args));

        var sourcePath = args.FullPath;
        Assert.IsNotNull(sourcePath, nameof(sourcePath));        

        Console.WriteLine($"{DateTime.Now:HH:mm:ss} RENAMED: {sourcePath}");

        return true;
      }

      protected override void TryProcess(FileSystemEventArgs args, string sourceRoot, string targetRoot)
      {
        Assert.ArgumentNotNull(args, nameof(args));
        Assert.ArgumentNotNullOrEmpty(sourceRoot, nameof(sourceRoot));
        Assert.ArgumentNotNullOrEmpty(targetRoot, nameof(targetRoot));

        var relatviePath = args.Name;
        Assert.IsNotNull(relatviePath, nameof(relatviePath));

        var targetPath = Path.Combine(targetRoot, relatviePath);
        Assert.IsNotNull(targetPath, nameof(targetPath));

        var targetDir = Path.GetDirectoryName(targetPath);
        Directory.CreateDirectory(targetDir);

        var rArgs = (RenamedEventArgs)args;

        var oldRelativePath = rArgs.OldName;
        Assert.IsNotNull(oldRelativePath, nameof(oldRelativePath));

        var oldTargetPath = Path.Combine(targetRoot, oldRelativePath);

        // delete old file in targetRoot
        if (File.Exists(oldTargetPath))
        {
          File.Delete(oldTargetPath);
          Console.WriteLine($"{DateTime.Now:HH:mm:ss} -KILLED: {oldTargetPath}");
        }

        // copy from sourcePath to targetPath
        File.Copy(args.FullPath, targetPath);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} +COPIED: {targetPath}");
      }
    }

    private class DeletedHandler : HandlerBase
    {
      protected override bool PreProcess(FileSystemEventArgs args)
      {
        Assert.ArgumentNotNull(args, nameof(args));

        var sourcePath = args.FullPath;
        Assert.IsNotNull(sourcePath, nameof(sourcePath));

        // file exists, but must be absent
        if (File.Exists(sourcePath))
        {
          return false;
        }

        Console.WriteLine($"{DateTime.Now:HH:mm:ss} DELETED: {sourcePath}");

        return true;
      }

      protected override void TryProcess(FileSystemEventArgs args, string sourceRoot, string targetRoot)
      {
        Assert.ArgumentNotNull(args, nameof(args));
        Assert.ArgumentNotNullOrEmpty(sourceRoot, nameof(sourceRoot));
        Assert.ArgumentNotNullOrEmpty(targetRoot, nameof(targetRoot));

        var relatviePath = args.Name;
        Assert.IsNotNull(relatviePath, nameof(relatviePath));

        var targetPath = Path.Combine(targetRoot, relatviePath);
        Assert.IsNotNull(targetPath, nameof(targetPath));

        File.Delete(targetPath);
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} -KILLED: {targetPath}");
      }
    }

    private abstract class HandlerBase
    {     
      protected abstract bool PreProcess([NotNull] FileSystemEventArgs args);

      protected abstract void TryProcess([NotNull] FileSystemEventArgs args, [NotNull] string sourceRoot, [NotNull] string targetPath);

      public void Process([NotNull] FileSystemEventArgs args, [NotNull] string sourceRoot, [NotNull] string[] targetRoots)
      {
        Assert.ArgumentNotNull(args, nameof(args));
        Assert.ArgumentNotNullOrEmpty(sourceRoot, nameof(sourceRoot));
        Assert.ArgumentNotNull(targetRoots, nameof(targetRoots));

        if (!PreProcess(args))
        {
          return;
        }
          
        var relativePath = args.Name;
        Assert.IsNotNull(relativePath, nameof(relativePath));

        var retryList = new List<string>();

        foreach (var targetRoot in targetRoots)
        {
          Assert.IsNotNullOrEmpty(targetRoot, nameof(targetRoot));

          if (sourceRoot.Equals(targetRoot, StringComparison.OrdinalIgnoreCase))
          {
            // skip B => B when A, B => B, C
            continue;
          }

          try
          {
            TryProcess(args, sourceRoot, targetRoot);
          }
          catch
          {
            retryList.Add(targetRoot);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} ?FAILED: {targetRoot}\\{relativePath}");
          }
        }       

        for (var i = 0; i < 10 && retryList.Any(); ++i)
        {
          Thread.Sleep(1000);

          var retryTargetRoots = retryList.ToArray();
          foreach (var targetRoot in retryTargetRoots)
          {
            try
            {
              TryProcess(args, sourceRoot, targetRoot);
              retryList.Remove(targetRoot);
            }
            catch
            {
            }
          }
        }

        foreach (var targetRoot in retryList)
        {
          Console.WriteLine($"{DateTime.Now:HH:mm:ss} ! FATAL: {targetRoot}\\{relativePath}");
        }

        Console.WriteLine();
      }
    }
  }
}