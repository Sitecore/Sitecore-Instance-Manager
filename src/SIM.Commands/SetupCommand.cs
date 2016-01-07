namespace SIM.Commands
{
  using System.IO;
  using System.Xml.Serialization;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ProfileCommand : AbstractCommand
  {
    protected override void DoExecute(CommandResult result)
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
        profile.ConnectionString = license;
        changes += 1;
      }

      var repository = this.Repository;
      if (!string.IsNullOrEmpty(repository))
      {
        profile.ConnectionString = repository;
        changes += 1;
      }

      if (changes > 0)
      {
        profile.Save();
      }

      try
      {
        result.Success = true;
        result.Data = Profile.Read();
      }
      catch
      {
        result.Success = false;
        result.Data = "profile is corrupted";
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

    [CanBeNull]
    public virtual string Plugins { get; [UsedImplicitly] set; }

    public class Profile : IProfile
    {
      [NotNull]
      private static readonly string ProfileFilePath = Path.Combine(ApplicationManager.ProfilesFolder, "profile.xml");
        
      public string ConnectionString { get; set; }

      public string InstancesFolder { get; set; }

      public string License { get; set; }

      public string LocalRepository { get; set; }

      public string Plugins { get; set; }

      [NotNull]
      public static Profile Read()
      {
        var deserializer = new XmlSerializer(typeof(Profile));
        using (var textReader = new StreamReader(ProfileFilePath))
        {
          return (Profile)deserializer.Deserialize(textReader);
        }
      }

      public void Save()
      {
        var deserializer = new XmlSerializer(typeof(Profile));
        using (var textWriter = new StreamWriter(ProfileFilePath))
        {
          deserializer.Serialize(textWriter, this);
        }
      }
    }
  }
}