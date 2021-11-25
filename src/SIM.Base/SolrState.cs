namespace SIM
{
  public class SolrState
  {
    [RenderInDataGreed]
    public string Name { get; set; }

    [RenderInDataGreed]
    public CurrentState State { get; set; }

    [RenderInDataGreed]
    public string Version { get; set; }

    [RenderInDataGreed]
    public string Url { get; set; }

    public enum CurrentState
    {
      Running,
      Stopped,
      ServiceNotExist
    }
  }
}