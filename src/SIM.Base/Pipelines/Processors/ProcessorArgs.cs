namespace SIM.Pipelines.Processors
{
  using System;

  #region

  #endregion

  public class ProcessorArgs : IDisposable
  {             
    #region Events

    public event Action OnCompleted;

    #endregion   

    #region Public Methods

    public virtual void Dispose()
    {
    }

    public void FireOnCompleted()
    {
      OnCompleted?.Invoke();
    }

    #endregion
  }
}