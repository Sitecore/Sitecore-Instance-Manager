namespace SIM.Core.Commands
{
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Instances;

  public class StateCommand : AbstractInstanceActionCommand<string>
  {
    protected override void DoExecute(Instance instance, CommandResult<string> result)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
      Assert.ArgumentNotNull(result, nameof(result));     

      result.Data = instance.State.ToString();
    }
  }
}