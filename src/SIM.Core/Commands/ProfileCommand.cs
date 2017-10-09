namespace SIM.Core.Commands
{
  using System;
  using System.Data;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core.Common;

  public class ProfileCommand : AbstractCommand<IProfile>
  {
    public ProfileCommand([NotNull] IO.IFileSystem fileSystem)
      : base(fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));   
    }

    [CanBeNull]
    public virtual string ConnectionString { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string InstancesRoot { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string License { get; [UsedImplicitly] set; }

    [CanBeNull]
    public virtual string Repository { get; [UsedImplicitly] set; }

    protected override void DoExecute(CommandResult<IProfile> result)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      var profile = Profile.Read(FileSystem);

      var changes = 0;
      var connectionString = ConnectionString;
      if (!string.IsNullOrEmpty(connectionString))
      {
        profile.ConnectionString = connectionString;
        changes += 1;
      }

      var instancesRoot = InstancesRoot;
      if (!string.IsNullOrEmpty(instancesRoot))
      {
        profile.InstancesFolder = instancesRoot;
        changes += 1;
      }

      var license = License;
      if (!string.IsNullOrEmpty(license))
      {
        profile.License = license;
        changes += 1;
      }

      var repository = Repository;
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
        result.Data = Profile.Read(FileSystem);
      }
      catch (Exception ex)
      {
        throw new DataException("Profile file is corrupted", ex);
      }
    }
  }
}