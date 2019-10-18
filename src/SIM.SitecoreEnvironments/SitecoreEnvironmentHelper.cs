namespace SIM.SitecoreEnvironments
{
  using System.Collections.Generic;
  using System.IO;
  using Newtonsoft.Json;

  public static class SitecoreEnvironmentHelper
  {
    public static List<SitecoreEnvironment> SitecoreEnvironments;

    public static List<SitecoreEnvironment> GetEnvironmentData()
    {
      List<SitecoreEnvironment> sitecoreEnvironments;
      using (StreamReader reader = new StreamReader(Path.Combine(ApplicationManager.ProfilesFolder, "Environments.json")))
      {
        string data = reader.ReadToEnd();
        sitecoreEnvironments = JsonConvert.DeserializeObject<List<SitecoreEnvironment>>(data);
      }

      return sitecoreEnvironments;
    }
  }
}