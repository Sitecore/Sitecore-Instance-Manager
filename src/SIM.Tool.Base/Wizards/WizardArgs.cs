namespace SIM.Tool.Base.Wizards
{
  using System;
  using System.Windows;
  using SIM.Pipelines.Processors;

  #region

  #endregion

  public abstract class WizardArgs : IDisposable
  {
    #region Public properties

    public Window WizardWindow { get; set; }

    #endregion

    #region Public Methods

    public virtual ProcessorArgs ToProcessorArgs()
    {
      return null;
    }

    #endregion

    public virtual void Dispose()
    {
    }
  }
}