#region Usings

using System;

#endregion

namespace SIM.Tool.Wizards
{
  #region

  

  #endregion

  /// <summary>
  ///   The wizard pipeline.
  /// </summary>
  public class WizardPipeline
  {
    #region Fields

    /// <summary>
    ///   The args.
    /// </summary>
    public readonly Type Args;

    /// <summary>
    ///   The finish action hives.
    /// </summary>
    public readonly FinishActionHive[] FinishActionHives;

    /// <summary>
    ///   The finish actions.
    /// </summary>
    public readonly FinishAction[] FinishActions;

    /// <summary>
    ///   The finish text.
    /// </summary>
    public readonly string FinishText;

    /// <summary>
    ///   The start button text.
    /// </summary>
    public readonly string StartButtonText;

    /// <summary>
    ///   The start button text.
    /// </summary>
    public readonly string CancelButtonText;

    /// <summary>
    ///   The step infos.
    /// </summary>
    public readonly StepInfo[] StepInfos;

    /// <summary>
    ///   The title.
    /// </summary>
    public readonly string Title;

    /// <summary>
    ///   The name.
    /// </summary>
    private readonly string name;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="WizardPipeline"/> class.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="stepInfos">
    /// The step infos. 
    /// </param>
    /// <param name="args">
    /// The arguments. 
    /// </param>
    /// <param name="startButtonText">
    /// The start button text. 
    /// </param>
    /// <param name="finishText">
    /// The finish text. 
    /// </param>
    /// <param name="finishActions">
    /// The finish actions. 
    /// </param>
    /// <param name="finishActionHives">
    /// The finish action hives. 
    /// </param>
    public WizardPipeline(string name, string title, StepInfo[] stepInfos, Type args, string startButtonText, string cancelButtonText, string finishText, FinishAction[] finishActions, FinishActionHive[] finishActionHives)
    {
      this.name = name;
      this.Title = title;
      this.FinishText = finishText;
      this.StartButtonText = startButtonText;
      this.CancelButtonText = cancelButtonText;
      this.Args = args;
      this.StepInfos = stepInfos;
      this.FinishActions = finishActions;
      this.FinishActionHives = finishActionHives;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets the name.
    /// </summary>
    public string Name
    {
      get
      {
        return this.name;
      }
    }

    #endregion
  }
}