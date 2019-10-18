using System.Linq;
using SIM.Sitecore9Installer;

namespace SIM.SitecoreEnvironments
{
  using System.Collections.Generic;
  using System.IO;
  using Newtonsoft.Json;

  public static class SitecoreEnvironmentHelper
  {
    public static List<SitecoreEnvironment> SitecoreEnvironments;

    public static List<SitecoreEnvironment> GetSitecoreEnvironmentData()
    {
      List<SitecoreEnvironment> sitecoreEnvironments;
      using (StreamReader reader = new StreamReader(Path.Combine(ApplicationManager.ProfilesFolder, "Environments.json")))
      {
        string data = reader.ReadToEnd();
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
            sitecoreEnvironment.Members.Add(new SitecoreEnvironmentMember(installParam.Value.Replace("$SqlDbPrefix+", prefix).Replace("\"", string.Empty), powerShellTask.Name));
          }
          else
          {
            sitecoreEnvironment.Members.Add(new SitecoreEnvironmentMember(installParam.Value, powerShellTask.Name));
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
      using (StreamWriter wr = new StreamWriter(Path.Combine(ApplicationManager.ProfilesFolder, "Environments.json")))
      {
        wr.WriteLine(GetSerializedSitecoreEnvironmentData(sitecoreEnvironments));
      }
    }
  }
}