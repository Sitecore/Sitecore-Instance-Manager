namespace SIM.Tool.Base.Profiles
{
  using System;
  using System.Data.SqlClient;
  using System.IO;
  using System.Xml.Serialization;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.IO;

  #region

  #endregion

  public static class ProfileManager
  {
    #region Fields

    [NotNull]
    private static string ProfileFilePath { get; } = GetProfilePath("profile.xml");

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

    public static void Initialize([NotNull] IFileSystem fileSystem)
    {
      Assert.ArgumentNotNull(fileSystem, nameof(fileSystem));

      var file = fileSystem.ParseFile(ProfileFilePath);
      if (file.Exists)
      {
        var profile = ReadProfile(file);

        Assert.IsNotNull(profile, nameof(profile));
        Profile = profile;
      }
    }

    public static Profile ReadProfile(IFile profileFile)
    {
      try
      {
        var deserializer = new XmlSerializer(typeof(Profile));
        using (TextReader textReader = new StreamReader(profileFile.FullName))
        {
          return (Profile)deserializer.Deserialize(textReader);
        }
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during reading profile");

        profileFile.TryDelete();
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
      var profileConnectionString = Profile.ConnectionString;
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