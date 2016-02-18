using System;

namespace SIM
{
  using System.Collections.Generic;
  using System.Collections.Specialized;
  using System.IO;

  public static class Null
  {
    public static readonly string String = null;
    public static readonly DirectoryInfo DirectoryInfo = null;
    public static Dictionary<string,string> StringDictionary = null;

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
