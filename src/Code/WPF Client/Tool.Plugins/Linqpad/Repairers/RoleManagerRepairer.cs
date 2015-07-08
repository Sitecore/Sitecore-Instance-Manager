using System.Collections.Generic;
using System.Xml;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public class RoleManagerRepairer : MakeNodeCommentRepairerProcessor
  {
    protected override IEnumerable<string> GetPaths(XmlDocument doc, Instance instance)
    {
      return new [] { "/configuration/sitecore/pipelines/indexing.filterIndex.outbound/processor[@type='Sitecore.ContentSearch.Pipelines.IndexingFilters.ApplyOutboundSecurityFilter, Sitecore.ContentSearch']" };
    }
  }
}