namespace SIM.Tool.Base.Wizards
{
  public interface IWizardStep
  {
    #region Public methods

    void InitializeStep(WizardArgs wizardArgs);
    bool SaveChanges(WizardArgs wizardArgs);

    #endregion
  }
}