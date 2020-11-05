namespace SIM.SitecoreEnvironments
{
  public class SitecoreEnvironmentMember
  {
    public SitecoreEnvironmentMember(string name, string type)
    {
      Name = name;
      Type = type;
    }

    public string Name { get; set; }

    public string Type { get; set; }

    public enum Types
    {
      Site,
      Service,
      Container
    }
  }
}
