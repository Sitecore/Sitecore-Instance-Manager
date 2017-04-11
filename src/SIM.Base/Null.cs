using System;

namespace SIM
{
  using System.Collections.Generic;
  using System.IO;

  public static class Null
  {
    public static string String { get; } = null;
    public static DirectoryInfo DirectoryInfo { get; } = null;
    public static Dictionary<string,string> _StringDictionary = null;

    public static T Safe<T>(Func<T> func) where T: class 
    {
      try
      {
        return func();
      }
      catch
      {
        return null;
      }
    }
  }
}
