using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using JetBrains.Annotations;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class TelerikKeyGenerator : ITelerikKeyGenerator
  {
    public string Generate()
    {
      string scriptFile = Path.Combine(Directory.GetCurrentDirectory(), "ContainerFiles/scripts/TelerikKeyGeneratorScript.ps1");
      PSFileExecutor ps = new PSFileExecutor(scriptFile, Directory.GetCurrentDirectory());
      Collection<PSObject> results = ps.Execute();

      if (results.Count < 1)
      {
        throw new InvalidOperationException("Error in: TelerikKeyGenerator. PS script has returned less than 1 result.");
      }

      return results[0].ToString();
    }
  }
}