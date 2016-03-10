namespace SIM.Tool.Base.Wizards
{
  using System;

  #region

  #endregion

  public class WizardPipeline
  {
    #region Fields

    public readonly Type Args;
    public readonly string CancelButtonText;

    public readonly FinishActionHive[] FinishActionHives;

    public readonly FinishAction[] FinishActions;

    public readonly string FinishText;

    public readonly string StartButtonText;

    public readonly StepInfo[] StepInfos;

    public readonly string Title;

    private readonly string name;

    #endregion

    #region Constructors

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