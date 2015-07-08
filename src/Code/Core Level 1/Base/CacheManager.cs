using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SIM.Base
{
  public static class CacheManager
  {
    private static readonly Dictionary<string, string> EncodeReplacements = new Dictionary<string, string>() { { "|", ".!.#.:." }, { Environment.NewLine, ".:.#.!." } };

    private class Cache : SortedDictionary<string, string>
    {
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
        FileSystem.Local.File.AppendAllText(GetFilePath(cacheName), "{0}|{1}{2}".FormatWith(EncodeDecodeValue(key, true), EncodeDecodeValue(value, true), Environment.NewLine));
      }
    }

    private static string GetFilePath(string cacheName)
    {
      return Path.Combine(ApplicationManager.CachesFolder, cacheName + ".txt");
    }


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

    private static bool isReady;
    private readonly static object getEntryLock = new object();
    private readonly static Dictionary<string, object> caches = new Dictionary<string, object>();
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

    private static string[] GetCacheFiles()
    {
      return FileSystem.Local.Directory.GetFiles(ApplicationManager.CachesFolder, "*.txt");
    }


    private static Cache LoadCache(string path)
    {
      var cache = new Cache();
      if (FileSystem.Local.File.Exists(path))
      {
        try
        {
          foreach (var line in FileSystem.Local.File.ReadAllLines(path).Where(line => !string.IsNullOrEmpty(line.Trim())))
          {
            var arr = line.Split('|');
            cache[EncodeDecodeValue(arr[0], false)] = EncodeDecodeValue(arr[1], false);
          }
        }
        catch (Exception ex)
        {
          Log.Warn("The {0} cache is corrupted and will be deleted".FormatWith(path), typeof(CacheManager), ex);
          FileSystem.Local.File.Delete(path);
        }
      }
      return cache;
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

    private static readonly object GetCacheLock = new object();

    public static void ClearAll()
    {
      lock (caches)
      {
        caches.Clear();
        foreach (var path in GetCacheFiles())
        {
          FileSystem.Local.File.Delete(path);
        }
      }
    }
  }
}
