using System.Xml;
using SIM.Instances;

namespace SIM.Tool.Plugins.Linqpad.Repairers
{
  public interface IRepairer
  {
    void Repair(XmlDocument doc, Instance instance);
  }
}