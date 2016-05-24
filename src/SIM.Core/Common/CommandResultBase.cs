namespace SIM.Core.Common
{
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class CommandResultBase<TResult> : CommandResultBase
  {
    public TResult Data { get; set; }
  }

  public abstract class CommandResultBase
  {
    public bool Success { get; set; }

    [CanBeNull]
    public string Message { get; set; }
  }
}