using System.Collections.Generic;
using System.Xml;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public class Log4NetRepairer : MakeNodeCommentRepairerProcessor
  {
    protected override IEnumerable<string> GetPaths(XmlDocument doc, Instance instance)
    {
      return new[] { "/configuration/log4net//encoding" };
    }
  }
}