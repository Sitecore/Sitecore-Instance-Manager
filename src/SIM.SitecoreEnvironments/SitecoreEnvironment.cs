namespace SIM.SitecoreEnvironments
{
  using System;
  using System.Collections.Generic;

  public class SitecoreEnvironment
  {
    public SitecoreEnvironment() { }

    public SitecoreEnvironment(string instanceName, List<SitecoreEnvironment> sitecoreEnvironments)
    {
      if (sitecoreEnvironments != null)
      {
        foreach (SitecoreEnvironment sitecoreEnvironment in sitecoreEnvironments)
        {
          foreach (SitecoreEnvironmentMember sitecoreEnvironmentMember in sitecoreEnvironment.Members)
          {
            if (sitecoreEnvironmentMember.Name == instanceName)
            {
              ID = sitecoreEnvironment.ID;
              Name = sitecoreEnvironment.Name;
              return;
            }
          }
        }
      }

      ID = Guid.NewGuid();
      Name = instanceName;
    }

    public Guid ID { get; set; }

    public string Name { get; set; }

    public List<SitecoreEnvironmentMember> Members { get; set; }
  }
}
