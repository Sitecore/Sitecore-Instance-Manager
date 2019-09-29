namespace SIM.Tool.Windows.UserControls.Helpers
{
  using Newtonsoft.Json.Linq;
  using SIM.Tool.Base.Pipelines;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using System;
  using System.Collections.Generic;
  using System.IO;

  public enum Topology { Undefined, XP0, XP1, XM1 }
  public static class UninstallTasksHelper
  {
    public static void AddUninstallTasks(Install9WizardArgs args)
    {
      // Uninstallation tasks should be added only to SC90x parameter files (*.json).      
      var versions = new List<int>() { 900, 901, 902 };

      var scVersionInt = args.InstanceProduct.Release.Version.MajorMinorUpdateInt;
      Log.Debug($"AddUnInstallationTasks: scVersionInt '{scVersionInt}'");

      if (!versions.Contains(scVersionInt))
      {
        // Starting from SC910 the uninstall tasks are included OOB.
        return;
      }

      Topology topology = Topology.Undefined;

      if (args.Product.Revision.Contains(Topology.XP0.ToString())) { topology = Topology.XP0; }
      else if (args.Product.Revision.Contains(Topology.XP1.ToString())) { topology = Topology.XP1; }
      else if (args.Product.Revision.Contains(Topology.XM1.ToString())) { topology = Topology.XM1; }

      if (topology == Topology.Undefined)
      {
        Log.Warn($"SC topology was not recognized. Revision: {args.Product.Revision}");
        return;
      }

      // Try to load Uninstall Tasks files
      var uninstallTasksFolder = "SC90UninstallTasks";
      var path = Path.Combine(Directory.GetCurrentDirectory(), uninstallTasksFolder, topology.ToString());
      var fileNames = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);

      if (fileNames.Length == 0)
      {
        Log.Warn($"Uninstall tasks patch files were not found. Path: {path}");
        return;
      }

      var uninstallTasksPatches = new Dictionary<string, JObject>();
      ReadUninstallTasksToDict(fileNames, uninstallTasksPatches);

      var listConfigFilesToPatch = GetConfigFilesToPatch(args);

      var result = PatchFilesWithUninstallTasks(listConfigFilesToPatch, uninstallTasksPatches);
    }

    private static bool PatchFilesWithUninstallTasks(string[] listFilesToPatch, Dictionary<string, JObject> uninstallPatches)
    {
      Assert.ArgumentNotNull(listFilesToPatch, "listFilesToPatch");
      Assert.ArgumentNotNull(uninstallPatches, "uninstallPatches");

      var success = true;
      var fail = !success;

      if (listFilesToPatch.Length <= 0)
      {
        Log.Debug($"PatchFilesWithUninstallTasks: list files to patch is empty.");
        return fail;
      }

      try
      {
        foreach (var fileToPatch in listFilesToPatch)
        {
          var key = Path.GetFileName(fileToPatch).ToLower();

          if (uninstallPatches.ContainsKey(key))
          {
            var sourceJObject = JObject.Parse(File.ReadAllText(fileToPatch));

            var deltaObject = uninstallPatches[key];

            sourceJObject.Merge(deltaObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Union });

            File.WriteAllText(fileToPatch, sourceJObject.ToString());

            Log.Debug($"File '{fileToPatch}' was patched with the uninstall tasks successfully.");
          }
        }
      }
      catch (Exception ex)
      {
        Log.Error($"Fail to patch OOB config files with uninstall tasks. \r\n{ex.Source}\r\n{ex.Message}");

        return fail;
      }
      return success;
    }

    private static string[] GetConfigFilesToPatch(Install9WizardArgs args)
    {
      return Directory.GetFiles(args.ScriptRoot, "*.json");
    }

    private static void ReadUninstallTasksToDict(string[] fileNames, Dictionary<string, JObject> uninstallPatches)
    {
      Assert.ArgumentNotNull(uninstallPatches, "uninstallPatches");
      Assert.ArgumentNotNull(fileNames, "fileNames");

      foreach (var fileName in fileNames)
      {
        var uninstallTask = JObject.Parse(File.ReadAllText(fileName));
        var key = Path.GetFileName(fileName).ToLower();
        uninstallPatches.Add(key, uninstallTask);
      }
    }
  }
}
