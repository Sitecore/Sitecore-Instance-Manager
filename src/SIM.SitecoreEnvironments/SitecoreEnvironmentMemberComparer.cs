using System.Collections.Generic;

namespace SIM.SitecoreEnvironments
{
  public class SitecoreEnvironmentMemberComparer : IEqualityComparer<SitecoreEnvironmentMember>
  {
    public bool Equals(SitecoreEnvironmentMember x, SitecoreEnvironmentMember y)
    {
      return x.Name == y.Name;
    }

    public int GetHashCode(SitecoreEnvironmentMember obj)
    {
      return obj.Name.GetHashCode();
    }
  }
}
