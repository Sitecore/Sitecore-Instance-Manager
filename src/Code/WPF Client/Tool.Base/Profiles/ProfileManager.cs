#region Usings

using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using SIM.Adapters;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Profiles
{
  #region

  

  #endregion

  /// <summary>
  ///   The profile manager.
  /// </summary>
  public static class ProfileManager
  {
    #region Fields

    /// <summary>
    ///   The profile file path.
    /// </summary>
    [NotNull] 
    private static readonly string ProfileFilePath = GetProfilePath("profile.xml");

    #endregion

    #region Properties

    /// <summary>
    ///   Gets a value indicating whether [file exists].
    /// </summary>
    /// <value> <c>true</c> if [file exists]; otherwise, <c>false</c> . </value>
    public static bool FileExists
    {
      get
      {
        return FileSystem.Local.File.Exists(ProfileFilePath);
      }
    }

    /// <summary>
    ///   Gets a value indicating whether this instance is valid.
    /// </summary>
    /// <value> <c>true</c> if this instance is valid; otherwise, <c>false</c> . </value>
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
          Log.Warn("The profile is invalid", typeof(ProfileManager), ex);

          return false;
        }
      }
    }

    /// <summary>
    ///   Gets Profile.
    /// </summary>
    [CanBeNull]
    public static Profile Profile { get; private set; }

    #endregion

    #region Public Methods

    /// <summary>
    ///   Initializes this instance.
    /// </summary>
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
        var deserializer = new XmlSerializer(typeof (Profile));
        using (TextReader textReader = new StreamReader(profileFilePath))
        {
          return (Profile) deserializer.Deserialize(textReader);
        }
      }
      catch (Exception ex)
      {
        Log.Warn("An error occurred during reading profile", typeof(ProfileManager), ex);

        FileSystem.Local.Directory.TryDelete(profileFilePath);
        return null;
      }
    }

    /// <summary>
    /// Saves the changes.
    /// </summary>
    /// <param name="profile">
    /// The profile. 
    /// </param>
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
  }
}