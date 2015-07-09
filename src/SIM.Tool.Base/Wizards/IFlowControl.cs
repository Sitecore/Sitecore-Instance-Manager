namespace SIM.Tool.Base.Wizards
{
  public interface IFlowControl
  {
    #region Public methods

    bool OnMovingBack(WizardArgs wizardArgs);
    bool OnMovingNext(WizardArgs wizardArgs);

    #endregion
  }
}