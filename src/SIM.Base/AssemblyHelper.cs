using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics.Base;
using Sitecore.Diagnostics.Base.Annotations;

namespace SIM
{
  public static class AssemblyHelper
  {
    /// <summary>
    /// Load assembly from file path.  FileSteam is disposed, which 
    /// prevents SIM from maintaining a lock on the DLL, which
    /// can prevent deleting Sitecore intances.
    /// 
    /// Adapted from http://stackoverflow.com/a/20080196/402949
    /// </summary>
    [NotNull]
    public static Assembly GetFromFilePath(string dllPath)
    {
      Assert.IsNotNull(dllPath, nameof(dllPath));

      FileStream stream = File.OpenRead(dllPath);

      Assert.IsNotNull(stream, nameof(stream));

      using (stream)
      {
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        return Assembly.Load(data);
      }
    }
  }
}
