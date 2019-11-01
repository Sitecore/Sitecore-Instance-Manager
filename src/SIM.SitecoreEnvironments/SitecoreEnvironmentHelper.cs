using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SIM.SitecoreEnvironments
{
  public static class SitecoreEnvironmentHelper
  {
    private const string FileName = "Environments.json";

    public static List<SitecoreEnvironment> SitecoreEnvironments
    {
      get;
      private set;
    }

    private static string FilePath
    {
      get { return Path.Combine(ApplicationManager.ProfilesFolder, FileName); }
    }

    private static List<SitecoreEnvironment> GetSitecoreEnvironmentData()
    {
      List<SitecoreEnvironment> sitecoreEnvironments;

      CreateEnvironmentsFileIfNeeded();

      using (StreamReader streamReader = new StreamReader(FilePath))
      {
        string data = streamReader.ReadToEnd();
        sitecoreEnvironments = JsonConvert.DeserializeObject<List<SitecoreEnvironment>>(data);
      }

      return sitecoreEnvironments;
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
  }
}