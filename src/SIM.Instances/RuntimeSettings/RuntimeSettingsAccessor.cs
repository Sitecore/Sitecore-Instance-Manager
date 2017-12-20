namespace SIM.Instances.RuntimeSettings
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using SIM.Adapters.MongoDb;
  using SIM.Adapters.SqlServer;
  using SIM.Adapters.WebServer;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.ConfigBuilder;
  using Sitecore.Diagnostics.Logging;

  public class RuntimeSettingsAccessor
  {
    #region Constructors

    public RuntimeSettingsAccessor(Instance instance)
    {
      Instance = instance;
    }

    #endregion

    #region Protected properties

    protected Instance Instance { get; set; }

    protected virtual string WebConfigPath
    {
      get
      {
        try
        {
          return WebConfig.GetWebConfigPath(Instance.WebRootPath);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed to get web config path of {Instance.WebRootPath}", ex);
        }
      }
    }

    #endregion

    #region Public methods

    public virtual ICollection<Database> GetDatabases()
    {
      try
      {
        var webConfigDocument = GetWebConfigResult();
        var webRootPath = Instance.WebRootPath;
        return WebConfig.GetDatabases(webRootPath, webConfigDocument);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Failed to get databases of {Instance.WebRootPath}", ex);
      }
    }

    public virtual ICollection<MongoDbDatabase> GetMongoDatabases()
    {
      try
      {
        var webConfigDocument = GetWebConfigResult();
        var webRootPath = Instance.WebRootPath;
        return WebConfig.GetMongoDatabases(webRootPath, webConfigDocument);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException($"Failed to get mongo databases of {Instance.WebRootPath}", ex);
      }
    }                

    public virtual string GetScVariableValue([NotNull] string variableName)
    {
      try
      {
        var webConfigResult = GetWebConfigResult();
        return WebConfig.GetScVariable(webConfigResult, variableName);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format("Failed to get {1} sc variable of {0}", Instance.WebRootPath, variableName), ex);
      }
    }

    public virtual XmlDocument GetShowconfig(bool normalize = false)
    {
      using (new ProfileSection("Computing showconfig", this))
      {
        try
        {
          ProfileSection.Argument("normalize", normalize);

          var showConfig = ConfigBuilder.Build(WebConfigPath, false, normalize);

          return ProfileSection.Result(showConfig);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed to get showconfig of {Instance.WebRootPath}", ex);
        }
      }
    }

    public virtual string GetSitecoreSettingValue(string name)
    {
      try
      {
        var webConfigResult = GetWebConfigResult();
        return WebConfig.GetSitecoreSetting(name, webConfigResult);
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException(string.Format("Failed to get {1} sitecore setting of {0}", Instance.WebRootPath, name), ex);
      }
    }

    public virtual XmlDocument GetWebConfigResult(bool normalize = false)
    {
      using (new ProfileSection("Computing web config result", this))
      {
        try
        {
          ProfileSection.Argument("normalize", normalize);

          var webConfigResult = ConfigBuilder.Build(WebConfigPath, true, normalize);

          return ProfileSection.Result(webConfigResult);
        }
        catch (Exception ex)
        {
          throw new InvalidOperationException($"Failed to get web config result of {Instance.WebRootPath}", ex);
        }
      }
    }

    #endregion
  }
}