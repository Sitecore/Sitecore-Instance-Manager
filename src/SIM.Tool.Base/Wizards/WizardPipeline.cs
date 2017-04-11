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

    public readonly FinishActionHive[] _FinishActionHives;

    public readonly FinishAction[] _FinishActions;

    public string FinishText { get; }

    public string StartButtonText { get; }

    public readonly StepInfo[] _StepInfos;

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
      this._StepInfos = stepInfos;
      this._FinishActions = finishActions;
      this._FinishActionHives = finishActionHives;
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