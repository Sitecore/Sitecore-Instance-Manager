#region Usings

using System;
using System.Collections.Generic;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Processors
{
  #region

  

  #endregion

  /// <summary>
  ///   The processor args.
  /// </summary>
  public class ProcessorArgs : AbstractArgs
  {
    #region Fields

    /// <summary>
    ///   The arguments.
    /// </summary>
    private readonly Dictionary<string, object> arguments = new Dictionary<string, object>();

    #endregion

    #region Events

    /// <summary>
    ///   The on completed.
    /// </summary>
    public event Action OnCompleted;

    #endregion

    #region Indexers

    /// <summary>
    ///   The this.
    /// </summary>
    /// <param name="key"> The key. </param>
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

    /// <summary>
    ///   The dispose.
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    ///   The fire on completed.
    /// </summary>
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