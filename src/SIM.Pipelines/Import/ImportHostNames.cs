namespace SIM.Pipelines.Import
{
  using SIM.Adapters.WebServer;

  public class ImportHostNames : ImportProcessor
  {
    #region Protected methods

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

    #endregion
  }
}