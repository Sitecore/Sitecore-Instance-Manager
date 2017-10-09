namespace SIM.Pipelines.Delete
{
  #region

  using System;
  using Microsoft.Win32;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  #endregion

  [UsedImplicitly]
  public class DeleteRegistryKey : DeleteProcessor
  {
    #region Constants

    protected const string SitecoreNodePath = "SOFTWARE\\Wow6432Node\\Sitecore CMS";

    #endregion

    #region Methods

    protected override void Process(DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var localMachine = Registry.LocalMachine;
      Assert.IsNotNull(localMachine, nameof(localMachine));

      var sitecoreNode = localMachine.OpenSubKey(SitecoreNodePath, true);
      if (sitecoreNode == null)
      {
        return;
      }

      foreach (var subKeyName in sitecoreNode.GetSubKeyNames())
      {
        Assert.IsNotNull(subKeyName, nameof(subKeyName));

        var instanceNode = sitecoreNode.OpenSubKey(subKeyName);
        if (instanceNode == null)
        {
          continue;
        }

        var name = instanceNode.GetValue("IISSiteName") as string ?? string.Empty;
        var dir = (instanceNode.GetValue("InstanceDirectory") as string ?? string.Empty).TrimEnd('\\');
        if (name.Equals(args.InstanceName, StringComparison.OrdinalIgnoreCase) || dir.Equals(args.Instance.RootPath.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase))
        {
          Log.Info($"Deleting {SitecoreNodePath}\\{subKeyName} key from registry");
          sitecoreNode.DeleteSubKey(subKeyName);

          return;
        }
      }
    }

    #endregion
  }
}