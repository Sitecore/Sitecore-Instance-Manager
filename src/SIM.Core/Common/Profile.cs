namespace SIM.Core.Common
{
  using System.IO;
  using System.Xml.Serialization;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using SIM.IO;

  public class Profile : IProfile
  {
    [NotNull]
    private static string ProfileFilePath { get; } = Path.Combine(ApplicationManager.ProfilesFolder, "profile.xml");

    public string ConnectionString { get; set; }

    public string InstancesFolder { get; set; }

    public string License { get; set; }

    public string LocalRepository { get; set; }

    protected IO.IFileSystem FileSystem { get; private set; }

    public void Save()
    {
      var deserializer = new XmlSerializer(typeof(Profile));
      using (var textWriter = new StreamWriter(ProfileFilePath))
      {
        deserializer.Serialize(textWriter, this);
      }
    }

    [NotNull]
    public static IProfile Read([NotNull] IO.IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      var profileFile = fileSystem.ParseFile(ProfileFilePath);  
      var deserializer = new XmlSerializer(typeof(Profile));
      using (var textReader = new StreamReader(profileFile.OpenRead()))
      {
        var profile = (Profile)deserializer.Deserialize(textReader);
        profile.FileSystem = fileSystem;

        return profile;
      }
    }
  }
}