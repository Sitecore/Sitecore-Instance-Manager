namespace SIM.Commands.Common
{
  public abstract class CommandResultBase
  {
    public bool Success { get; set; }

    public object Data { get; set; }
  }
}