namespace SIM.Core.Commands
{
  using System;
  using System.Data;
  using SIM.Core.Common;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ProfileCommand : AbstractCommand<IProfile>
  {
    protected override void DoExecute(CommandResult<IProfile> result)
    {
      Assert.ArgumentNotNull(result, "result");

      var profile = Profile.Read();

      var changes = 0;
      var connectionString = this.ConnectionString;
      if (!string.IsNullOrEmpty(connectionString))
      {
        profile.ConnectionString = connectionString;
        changes += 1;
      }

      var instancesRoot = this.InstancesRoot;
      if (!string.IsNullOrEmpty(instancesRoot))
      {
        profile.InstancesFolder = instancesRoot;
        changes += 1;
      }

      var license = this.License;
      if (!string.IsNullOrEmpty(license))
      {
        profile.License = license;
        changes += 1;
      }

      var repository = this.Repository;
      if (!string.IsNullOrEmpty(repository))
      {
        profile.LocalRepository = repository;
        changes += 1;
      }

      if (changes > 0)
      {
        profile.Save();
      }

      try
      {
        result.Data = Profile.Read();
      }
      catch (Exception ex)
      {
        throw new DataException("Profile file is corrupted", ex);
      }
    }

    [CanBeNull]
    public virtual string ConnectionString { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string InstancesRoot { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string License { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Repository { get; [UsedImplicitly]  set; }
  }
}