namespace SIM.Core.Common
{
  using System.IO;
  using System.Xml.Serialization;
  using Sitecore.Diagnostics.Base.Annotations;

  public class Profile : IProfile
  {
    [NotNull]
    private static readonly string ProfileFilePath = Path.Combine(ApplicationManager.ProfilesFolder, "profile.xml");

    public string ConnectionString { get; set; }

    public string InstancesFolder { get; set; }

    public string License { get; set; }

    public string LocalRepository { get; set; }

    [NotNull]
    public static IProfile Read()
    {
      var deserializer = new XmlSerializer(typeof(Profile));
      using (var textReader = new StreamReader(ProfileFilePath))
      {
        return (IProfile)deserializer.Deserialize(textReader);
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