namespace SIM.Tool.Base.Wizards
{
  using System;
  using System.Collections.Generic;

  #region

  #endregion

  public abstract class FinishActionHive
  {
    #region Fields

    protected Type WizardArgumentsType { get; }

    #endregion

    #region Constructors

    public FinishActionHive(Type type)
    {
      this.WizardArgumentsType = type;
    }

    #endregion

    #region Public methods

    public abstract IEnumerable<FinishAction> GetFinishActions(WizardArgs wizardArgs);

    #endregion
  }
}