#region Usings

using System;
using SIM.Base;

#endregion

namespace SIM.Tool.Base.Profiles
{
  #region

  

  #endregion

  /// <summary>
  ///   The profile.
  /// </summary>
  public class Profile : DataObject, ICloneable
  {
    #region Properties

    #region Public properties

    /// <summary>
    ///   Gets or sets the connection string.
    /// </summary>
    /// <value> The connection string. </value>
    [NotNull]
    public string  ConnectionString
    {
      get
      {
        return this.GetString("ConnectionString") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("ConnectionString", value);
      }
    }

    /// <summary>
    ///   Gets or sets the instances folder.
    /// </summary>
    /// <value> The instances folder. </value>
    [NotNull]
    public string InstancesFolder
    {
      get
      {
        return this.GetString("InstancesFolder") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("InstancesFolder", value);
      }
    }

    /// <summary>
    ///   Gets or sets the license.
    /// </summary>
    /// <value> The license. </value>
    [NotNull]
    public string License
    {
      get
      {
        return this.GetString("License") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("License", value);
      }
    }

    /// <summary>
    ///   Gets or sets the local repository.
    /// </summary>
    /// <value> The local repository. </value>
    [NotNull]
    public string LocalRepository
    {
      get
      {
        return this.GetString("LocalRepository") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("LocalRepository", value);
      }
    }

    [NotNull]
    public string Plugins 
    {
      get
      {
        return this.GetString("Plugins") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("Plugins", value);
      }
    }

    [NotNull]
    public string AdvancedSettings
    {
      get
      {
        return this.GetString("AdvancedSettings") ?? string.Empty;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.SetValue("AdvancedSettings", value);
      }
    }

    #endregion

    #region Protected methods

    /// <summary>
    ///   The validate instances folder.
    /// </summary>
    protected void ValidateInstancesFolder()
    {
      Assert.IsNotNullOrEmpty(this.InstancesFolder, "The InstancesFolder is null or empty");
      FileSystem.Local.Directory.AssertExists(this.InstancesFolder, string.Format("The InstancesFolder does not exist ({0})", this.InstancesFolder));
    }

    protected void ValidateLicense()
    {
      Assert.IsNotNullOrEmpty(this.License, "The license file isn't set");
      FileSystem.Local.File.AssertExists(this.License, "The license file doesn't exist");      
    }

    #endregion

    #endregion

    #region Implemented Interfaces

    #region ICloneable

    /// <summary>
    ///   Creates a new object that is a copy of the current instance.
    /// </summary>
    /// <returns> A new object that is a copy of this instance. </returns>
    public object Clone()
    {
      return new Profile {ConnectionString = this.ConnectionString, InstancesFolder = this.InstancesFolder, License = this.License, LocalRepository = this.LocalRepository, Plugins = this.Plugins};
    }

    #endregion

    #endregion
  }
}