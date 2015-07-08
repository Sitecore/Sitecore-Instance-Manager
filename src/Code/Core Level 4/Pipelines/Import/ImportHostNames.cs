using SIM.Adapters.WebServer;

namespace SIM.Pipelines.Import
{
  public class ImportHostNames : ImportProcessor
  {
    protected override void Process(ImportArgs args)
    {
      if (args.bindings.Count == 0)
      {
        Hosts.Append(args.siteName);
      }

      else
      {
        foreach (string hostname in args.bindings.Keys)
        {
          Hosts.Append(hostname);
        }
      }
    }
  }
}
