namespace SIM.Tool.Base.Wizards
{
  using System;

  #region

  #endregion

  public class WizardPipeline
  {
    #region Fields

    public string CancelButtonText { get; }

    public readonly FinishActionHive[] _FinishActionHives;

    public IAfterLastWizardPipelineStep AfterLastStep { get; }

    public readonly FinishAction[] _FinishActions;

    public string FinishText { get; }

    public string StartButtonText { get; }

    public readonly StepInfo[] _StepInfos;

    public string Title { get; }

    private string name { get; }

    #endregion

    #region Constructors

    public WizardPipeline(string name, string title, StepInfo[] stepInfos, Type args, string startButtonText, string cancelButtonText, string finishText, FinishAction[] finishActions, FinishActionHive[] finishActionHives, IAfterLastWizardPipelineStep afterLastStep)
    {
      this.name = name;
      Title = title;
      FinishText = finishText;
      StartButtonText = startButtonText;
      CancelButtonText = cancelButtonText;
      _StepInfos = stepInfos;
      _FinishActions = finishActions;
      _FinishActionHives = finishActionHives;
      AfterLastStep = afterLastStep;
    }

    #endregion

    #region Properties

    public string Name
    {
      get
      {
        return name;
      }
    }

    #endregion
  }
}