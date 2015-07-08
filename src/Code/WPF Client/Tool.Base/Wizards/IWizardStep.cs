namespace SIM.Tool.Base.Wizards
{
  public interface IWizardStep
  {
    void InitializeStep(WizardArgs wizardArgs);
    bool SaveChanges(WizardArgs wizardArgs);
  }
}
