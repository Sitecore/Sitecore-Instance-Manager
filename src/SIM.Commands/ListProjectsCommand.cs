namespace SIM.Commands
{
  using System;
  using Sitecore.Diagnostics.Base.Annotations;

  public class ListProjectsCommand : AbstractCommand
  {
    [CanBeNull]
    public virtual string Filter { get; set; }

    public override void Execute()
    {
      throw new NotImplementedException();
    }
  }
}