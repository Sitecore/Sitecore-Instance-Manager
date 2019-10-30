using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace SIM.SitecoreEnvironments
{
  public static class SitecoreEnvironmentHelper
  {
    private const string FileName = "Environments.json";

    public static string FilePath
    {
      get { return Path.Combine(ApplicationManager.ProfilesFolder, FileName); }
    }

    public static List<SitecoreEnvironment> SitecoreEnvironments;

    public static List<SitecoreEnvironment> GetSitecoreEnvironmentData()
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
    
    public static string GetSerializedSitecoreEnvironmentData(List<SitecoreEnvironment> sitecoreEnvironments)
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

    public static void CreateEnvironmentsFileIfNeeded()
    {
      if (!File.Exists(FilePath))
      {
        File.Create(FilePath).Close();
      }
    } 
  }
}