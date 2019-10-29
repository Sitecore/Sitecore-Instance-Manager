using System.Linq;
using SIM.Sitecore9Installer;
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

    public static SitecoreEnvironment CreateSitecoreEnvironment(string prefix, List<PowerShellTask> tasks)
    {
      SitecoreEnvironment sitecoreEnvironment = new SitecoreEnvironment(prefix);
      sitecoreEnvironment.Members = new List<SitecoreEnvironmentMember>();

      foreach (PowerShellTask powerShellTask in tasks)
      {
        InstallParam installParam = powerShellTask.LocalParams.FirstOrDefault(x => x.Name == "SiteName");
        if (installParam != null && !string.IsNullOrEmpty(installParam.Value))
        {
          if (installParam.Value.StartsWith("$SqlDbPrefix+"))
          {
            sitecoreEnvironment.Members.Add(new SitecoreEnvironmentMember(installParam.Value.Replace("$SqlDbPrefix+", prefix).Replace("\"", string.Empty), SitecoreEnvironmentMember.Types.Site.ToString()));
          }
          else
          {
            sitecoreEnvironment.Members.Add(new SitecoreEnvironmentMember(installParam.Value, SitecoreEnvironmentMember.Types.Site.ToString()));
          }
        }
      }

      return sitecoreEnvironment;
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