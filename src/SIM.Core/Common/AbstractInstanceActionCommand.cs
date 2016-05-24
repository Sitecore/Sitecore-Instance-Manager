namespace SIM.Core.Common
{
  using Sitecore.Diagnostics.Base.Annotations;

  public abstract class AbstractInstanceActionCommand<T> : AbstractCommand<T>
  {
    [CanBeNull]
    public virtual string Name { get; [UsedImplicitly] set; }
  }
}