﻿namespace SIM.Pipelines.Delete
{
  #region

  using System.Linq;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;

  #endregion

  [UsedImplicitly]
  public class DeleteWebsiteFolder : DeleteProcessor
  {
    #region Methods

    protected override void Process([NotNull] DeleteArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var cachedInstance = InstanceManager.Default.PartiallyCachedInstances?.Values.SingleOrDefault(x => x.ID == args.InstanceID) as PartiallyCachedInstance;
      if (cachedInstance != null)
      {
        cachedInstance.Dispose();
      }

      FileSystem.FileSystem.Local.Directory.DeleteIfExists(args.WebRootPath);
    }

    #endregion
  }
}