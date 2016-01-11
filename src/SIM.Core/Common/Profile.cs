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