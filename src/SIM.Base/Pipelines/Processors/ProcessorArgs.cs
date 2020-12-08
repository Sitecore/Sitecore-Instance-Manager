namespace SIM.Pipelines.Processors
{
  using Sitecore.Diagnostics.Base;
  using System;

  #region

  #endregion

  public class ProcessorArgs : IDisposable
  {             
    #region Events

    public event Action OnCompleted;

    #endregion   

    public ProcessorArgs()
    {

    }
    public ProcessorArgs(string pipelineName)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, nameof(pipelineName));
      this.PipelineName = pipelineName;
    }

    public string PipelineName { get; protected set; }

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