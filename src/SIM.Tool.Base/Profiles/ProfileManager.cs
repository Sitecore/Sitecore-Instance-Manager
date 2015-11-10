namespace SIM.Tool.Base.Profiles
{
  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.Xml.Serialization;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class ProfileManager
  {
    #region Fields

    [NotNull]
    private static readonly string ProfileFilePath = GetProfilePath("profile.xml");

    #endregion

    #region Properties

    public static bool FileExists
    {
      get
      {
        return FileSystem.FileSystem.Local.File.Exists(ProfileFilePath);
      }
    }

    public static bool IsValid
    {
      get
      {
        try
        {
          Profile.Validate();
          return true;
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "The profile is invalid");

          return false;
        }
      }
    }

    [CanBeNull]
    public static Profile Profile { get; private set; }

    #endregion

    #region Public Methods

    public static void Initialize()
    {
      if (FileExists)
      {
        var profileFilePath = ProfileFilePath;
        var profile = ReadProfile(profileFilePath);

        Assert.IsNotNull(profile, "profile");
        Profile = profile;
      }
    }

    public static Profile ReadProfile(string profileFilePath)
    {
      try
      {
        var deserializer = new XmlSerializer(typeof(Profile));
        using (TextReader textReader = new StreamReader(profileFilePath))
        {
          return (Profile)deserializer.Deserialize(textReader);
        }
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during reading profile");

        FileSystem.FileSystem.Local.Directory.TryDelete(profileFilePath);
        return null;
      }
    }

    public static void SaveChanges([CanBeNull] Profile profile = null)
    {
      if (profile != null)
      {
        Profile = profile;
      }

      XmlSerializer serializer = new XmlSerializer(typeof(Profile));
      using (TextWriter textWriter = new StreamWriter(ProfileFilePath))
      {
        serializer.Serialize(textWriter, Profile);
      }
    }

    #endregion

    #region Public methods

    public static SqlConnectionStringBuilder GetConnectionString()
    {
      string profileConnectionString = ProfileManager.Profile.ConnectionString;
      Assert.IsNotNull(profileConnectionString, "Connection String Not Set");
      var connectionString = new SqlConnectionStringBuilder(profileConnectionString);
      return connectionString;
    }

    public static string GetProfilePath(string profileName)
    {
      return Path.Combine(ApplicationManager.ProfilesFolder, profileName);
    }

    #endregion
  }
}