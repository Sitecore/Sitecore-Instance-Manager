#region Usings

using SIM.Base;
using SIM.Instances;
using SIM.Pipelines.Agent;

#endregion

namespace SIM.Pipelines.Install.Modules
{
  #region

  

  #endregion

  /// <summary>
  ///   The start instance.
  /// </summary>
  [UsedImplicitly]
  public class StartInstance : InstallProcessor
  {
    #region Methods

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected override void Process([NotNull] InstallArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      Instance instance = args.Instance;
      Assert.IsNotNull(instance, "Instance");
      InstanceHelper.StartInstance(instance);
    }

    #endregion
  }
}