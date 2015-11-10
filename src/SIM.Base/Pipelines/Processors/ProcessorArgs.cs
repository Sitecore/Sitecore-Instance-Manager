namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base.Annotations;

  #region

  #endregion

  public class ProcessorArgs : AbstractArgs
  {
    #region Fields

    private readonly Dictionary<string, object> arguments = new Dictionary<string, object>();

    #endregion

    #region Events

    public event Action OnCompleted;

    #endregion

    #region Indexers

    [CanBeNull]
    public object this[string key]
    {
      get
      {
        return this.arguments[key];
      }

      set
      {
        this.arguments[key] = value;
      }
    }

    #endregion

    #region Public Methods

    public virtual void Dispose()
    {
    }

    public void FireOnCompleted()
    {
      if (this.OnCompleted != null)
      {
        this.OnCompleted();
      }
    }

    #endregion
  }
}