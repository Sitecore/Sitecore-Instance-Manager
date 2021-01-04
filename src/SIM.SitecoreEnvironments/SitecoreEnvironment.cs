namespace SIM.SitecoreEnvironments
{
  using System;
  using System.Collections.Generic;

  public class SitecoreEnvironment
  {
    public SitecoreEnvironment()
    {
      this.EnvType = EnvironmentType.OnPrem;
    }

    public SitecoreEnvironment(string instanceName) : this(instanceName, EnvironmentType.OnPrem) { }
    public SitecoreEnvironment(string instanceName, EnvironmentType type)
    {
      ID = Guid.NewGuid();
      Name = instanceName;
      this.EnvType = type;
    }

    public Guid ID { get; set; }

    public string Name { get; set; }

    public string Image
    {
      get
      {
        switch (EnvType)
        {
          case EnvironmentType.OnPrem:
          {
            return @"pack://application:,,,/SIM.Tool.Windows;component/Images/16/sitecore.png";
          }
          case EnvironmentType.Container:
          {
            return @"pack://application:,,,/SIM.Tool.Windows;component/Images/16/docker.png";
          }
          default:
          {
            return null;
          }
        }
      }
    }

    public List<SitecoreEnvironmentMember> Members { get; set; }

    public string UnInstallDataPath { get; set; }

    public EnvironmentType EnvType { get; set; }
      
    public enum EnvironmentType { OnPrem, Container}
  }
}
