namespace SIM.Tool.Base.Wizards
{
  using System;

  #region

  #endregion

  public class WizardPipeline
  {
    #region Fields

    public Type Args { get; }
    public string CancelButtonText { get; }

    public readonly FinishActionHive[] FinishActionHives;

    public readonly FinishAction[] FinishActions;

    public string FinishText { get; }

    public string StartButtonText { get; }

    public readonly StepInfo[] StepInfos;

    public string Title { get; }

    private string name { get; }

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