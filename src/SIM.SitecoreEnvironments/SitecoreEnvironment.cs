namespace SIM.SitecoreEnvironments
{
  using System;
  using System.Collections.Generic;

  public class SitecoreEnvironment
  {
    public SitecoreEnvironment() { }

    public SitecoreEnvironment(string instanceName)
    {
      ID = Guid.NewGuid();
      Name = instanceName;
    }

    public Guid ID { get; set; }

    public string Name { get; set; }

    public List<SitecoreEnvironmentMember> Members { get; set; }
  }
}
