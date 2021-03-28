using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sitecore.Diagnostics.Logging;

namespace SIM.SitecoreEnvironments
{
  public static class SitecoreEnvironmentHelper
  {
    private const string FileName = "Environments.json";

    private static List<SitecoreEnvironment> sitecoreEnvironments;

    public static List<SitecoreEnvironment> SitecoreEnvironments
    {
      get
      {
        if (sitecoreEnvironments == null)
        {
          sitecoreEnvironments = new List<SitecoreEnvironment>();
        }

        return sitecoreEnvironments;
      }
      private set
      {
        sitecoreEnvironments = value;
      }
    }

    private static string FilePath
    {
      get { return Path.Combine(ApplicationManager.ProfilesFolder, FileName); }
    }

    private static List<SitecoreEnvironment> GetSitecoreEnvironmentData()
    {
      try
      {
        CreateEnvironmentsFileIfNeeded();

        using (StreamReader streamReader = new StreamReader(FilePath))
        {
          string data = streamReader.ReadToEnd();
          return JsonConvert.DeserializeObject<List<SitecoreEnvironment>>(data);
        }
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "An error occurred during getting the list of Sitecore environments");

        return new List<SitecoreEnvironment>();
      }
    }
    
    private static string GetSerializedSitecoreEnvironmentData(List<SitecoreEnvironment> sitecoreEnvironments)
    {
      return JsonConvert.SerializeObject(sitecoreEnvironments);
    }

    public static void SaveSitecoreEnvironmentData(List<SitecoreEnvironment> sitecoreEnvironments)
    {
      CreateEnvironmentsFileIfNeeded();

      using (StreamWriter streamWriter = new StreamWriter(FilePath))
      {
        streamWriter.WriteLine(GetSerializedSitecoreEnvironmentData(sitecoreEnvironments));
      }
    }

    private static void CreateEnvironmentsFileIfNeeded()
    {
      if (!File.Exists(FilePath))
      {
        File.Create(FilePath).Close();
      }
    }

    public static void RefreshEnvironments()
    {
      SitecoreEnvironments = GetSitecoreEnvironmentData();
    }


    public static SitecoreEnvironment GetExistingOrNewSitecoreEnvironment(string instanceName)
    {    
      return GetExistingSitecoreEnvironment(instanceName)?? new SitecoreEnvironment(instanceName);
    }

    [CanBeNull]
    public static SitecoreEnvironment GetExistingSitecoreEnvironment(string instanceName)
    {
      foreach (SitecoreEnvironment sitecoreEnvironment in SitecoreEnvironments)
      {
        foreach (SitecoreEnvironmentMember sitecoreEnvironmentMember in sitecoreEnvironment.Members)
        {
          if (sitecoreEnvironmentMember.Name == instanceName)
          {
            return sitecoreEnvironment;
          }
        }
      }

      return null;
    }

    public static bool TryGetEnvironmentById(Guid environmentId, [CanBeNull]out SitecoreEnvironment environment)
    {
      environment = SitecoreEnvironments.FirstOrDefault(e => e.ID.Equals(environmentId));

      return environment != null;
    }

    public static void AddSitecoreEnvironment(SitecoreEnvironment sitecoreEnvironment)
    {
      // Do not add new Sitecore environment if its name already exists in the Environments.json file
      foreach (SitecoreEnvironment se in SitecoreEnvironments)
      {
        if (se.Name == sitecoreEnvironment.Name)
        {
          return;
        }
      }

      SitecoreEnvironments.Add(sitecoreEnvironment);
      SaveSitecoreEnvironmentData(SitecoreEnvironments);
    }
  }
}