namespace SIM.Tool.Base.Wizards
{
  using System;
  using System.Windows;
  using SIM.Loggers;
  using SIM.Pipelines.Processors;

  #region

  #endregion

  public abstract class WizardArgs : IDisposable
  {
    #region Public properties

    public Window WizardWindow { get; set; }

    public ILogger Logger { get; set; }

    //Indicates if the installation has been completed
    public bool HasInstallationBeenCompleted { get; set; }

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