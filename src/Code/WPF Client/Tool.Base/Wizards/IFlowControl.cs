namespace SIM.Tool.Base.Wizards
{
  public interface IFlowControl
  {
    bool OnMovingNext(WizardArgs wizardArgs);
    bool OnMovingBack(WizardArgs wizardArgs);
  }
}
