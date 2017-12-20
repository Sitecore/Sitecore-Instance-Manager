namespace SIM.Pipelines.Import
{
  using SIM.Adapters.WebServer;

  public class ImportHostNames : ImportProcessor
  {
    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      if (args._Bindings.Count == 0)
      {
        Hosts.Append(args._SiteName);
      }
      else
      {
        foreach (string hostname in args._Bindings.Keys)
        {
          Hosts.Append(hostname);
        }
      }
    }

    #endregion
  }
}