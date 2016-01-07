namespace SIM.Commands.Common
{
  public abstract class CommandResultBase
  {
    public bool Success { get; set; }

    public string Message { get; set; }

    public object Data { get; set; }
  }
}