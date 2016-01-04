namespace SIM
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;

  public static class CacheManager
  {
    #region Fields

    private static readonly Dictionary<string, string> EncodeReplacements = new Dictionary<string, string>()
    {
      {
        "|", ".!.#.:."
      }, 
      {
        Environment.NewLine, ".:.#.!."
      }
    };

    private static readonly object GetCacheLock = new object();
    private static readonly Dictionary<string, object> caches = new Dictionary<string, object>();
    private static readonly object getEntryLock = new object();
    private static bool isReady;

    #endregion

    #region Public methods

    public static void ClearAll()
    {
      lock (caches)
      {
        caches.Clear();
        foreach (var path in GetCacheFiles())
        {
          FileSystem.FileSystem.Local.File.Delete(path);
        }
      }
    }

    public static string GetEntry(string cacheName, string key)
    {
      key = key.ToLowerInvariant();
      if (!isReady)
      {
        lock (getEntryLock)
        {
          if (!isReady)
          {
            LoadCaches();
          }
        }
      }

      var cache = GetCache(cacheName);
      return cache.ContainsKey(key) ? cache[key] : null;
    }

    public static void SetEntry(string cacheName, string key, string value)
    {
      key = key.ToLowerInvariant();
      var cache = GetCache(cacheName);
      lock (cache)
      {
        cache[key] = value;
        FileSystem.FileSystem.Local.File.AppendAllText(GetFilePath(cacheName), "{0}|{1}{2}".FormatWith(EncodeDecodeValue(key, true), EncodeDecodeValue(value, true), Environment.NewLine));
      }
    }

    #endregion

    #region Private methods

    private static string EncodeDecodeValue(string input, bool encode)
    {
      var result = input;
      foreach (KeyValuePair<string, string> replacement in EncodeReplacements)
      {
        if (encode)
        {
          result = result.Replace(replacement.Key, replacement.Value);
        }
        else
        {
          result = result.Replace(replacement.Value, replacement.Key);
        }
      }

      return result;
    }

    private static Cache GetCache(string cacheName)
    {
      if (!caches.ContainsKey(cacheName))
      {
        lock (GetCacheLock)
        {
          if (!caches.ContainsKey(cacheName))
          {
            var cache = new Cache();
            caches.Add(cacheName, cache);
            return cache;
          }
        }
      }

      return (Cache)caches[cacheName];
    }

    private static string[] GetCacheFiles()
    {
      return FileSystem.FileSystem.Local.Directory.GetFiles(ApplicationManager.CachesFolder, "*.txt");
    }

    private static string GetFilePath(string cacheName)
    {
      return Path.Combine(ApplicationManager.CachesFolder, cacheName + ".txt");
    }


    private static Cache LoadCache(string path)
    {
      var cache = new Cache();
      if (FileSystem.FileSystem.Local.File.Exists(path))
      {
        try
        {
          foreach (var line in FileSystem.FileSystem.Local.File.ReadAllLines(path).Where(line => !string.IsNullOrEmpty(line.Trim())))
          {
            var arr = line.Split('|');
            cache[EncodeDecodeValue(arr[0], false)] = EncodeDecodeValue(arr[1], false);
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "The {0} cache is corrupted and will be deleted", path);
          FileSystem.FileSystem.Local.File.Delete(path);
        }
      }

      return cache;
    }

    private static void LoadCaches()
    {
      Assert.IsTrue(!isReady, "The LoadCaches() method must be executed only once");
      foreach (var path in GetCacheFiles())
      {
        var fileName = Path.GetFileNameWithoutExtension(path).Split('.');
        var name = fileName[0];
        Assert.IsTrue(!caches.ContainsKey(name), "The {0} cache is already created".FormatWith(fileName));
        caches.Add(name, LoadCache(path));
      }

      isReady = true;
    }

    #endregion

    #region Nested type: Cache

    private class Cache : SortedDictionary<string, string>
    {
    }

    #endregion
  }
}