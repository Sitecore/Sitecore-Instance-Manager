#region Usings

using System.Collections.Generic;
using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   The step.
  /// </summary>
  public class Step
  {
    #region Fields

    /// <summary>
    ///   The args name.
    /// </summary>
    [CanBeNull]
    public readonly string ArgsName;

    /// <summary>
    ///   The processors.
    /// </summary>
    [NotNull]
    public readonly List<Processor> Processors;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Step"/> class.
    /// </summary>
    /// <param name="processors">
    /// The processors. 
    /// </param>
    /// <param name="argsName">
    /// The args name. 
    /// </param>
    public Step([NotNull] List<Processor> processors, [CanBeNull] string argsName)
    {
      Assert.ArgumentNotNull(processors, "processors");

      this.Processors = processors;
      this.ArgsName = argsName;
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// The create steps.
    /// </summary>
    /// <param name="stepDefinitions">
    /// The step definitions. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static List<Step> CreateSteps([NotNull] List<StepDefinition> stepDefinitions, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(stepDefinitions, "stepDefinitions");
      Assert.ArgumentNotNull(args, "args");

      return new List<Step>(CreateStepsPrivate(stepDefinitions, args, controller));
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create steps private.
    /// </summary>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private static IEnumerable<Step> CreateStepsPrivate([NotNull] IEnumerable<StepDefinition> steps, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(steps, "steps");
      Assert.ArgumentNotNull(args, "args");

      foreach (StepDefinition stepDefinition in steps)
      {
        string argsName = stepDefinition.ArgsName.EmptyToNull();
        Step step = new Step(ProcessorManager.CreateProcessors(stepDefinition.ProcessorDefinitions, args, controller), argsName);
        Assert.IsNotNull(step, "Can't instantiate step");
        yield return step;
      }
    }

    #endregion
  }
}