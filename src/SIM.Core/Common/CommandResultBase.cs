namespace SIM.Core.Common
{
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class CommandResultBase<TResult>
  {
    public bool Success { get; set; }

    [CanBeNull]
    public string Message { get; set; }

    public TResult Data { get; set; }
  }
}