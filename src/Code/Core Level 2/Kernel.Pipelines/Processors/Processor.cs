#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Processors
{

  #region

  #endregion

  /// <summary>
  ///   The processor.
  /// </summary>
  public abstract class Processor : DataObject
  {
    #region Fields

    /// <summary>
    ///   The nested processors.
    /// </summary>
    public readonly List<Processor> NestedProcessors = new List<Processor>();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref="Processor" /> class.
    /// </summary>
    protected Processor()
    {
      this.State = ProcessorState.Waiting;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   Gets Error.
    /// </summary>
    [CanBeNull]
    public Exception Error { get; private set; }

    /// <summary>
    ///   Gets or sets ProcessorDefinition.
    /// </summary>
    [NotNull]
    public ProcessorDefinition ProcessorDefinition { get; set; }

    /// <summary>
    ///   Gets State.
    /// </summary>
    public ProcessorState State
    {
      get
      {
        object value = this.GetValue("State");
        Assert.IsNotNull(value, "value");

        return (ProcessorState) value;
      }

      private set { this.SetValue("State", value); }
    }

    /// <summary>
    ///   Gets Title.
    /// </summary>
    [NotNull]
    public string Title
    {
      get { return this.ProcessorDefinition.Title; }
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Controller.
    /// </summary>
    protected internal IPipelineController Controller { get; internal set; }

    /// <summary>
    ///   Gets a value indicating whether IsDone.
    /// </summary>
    private bool IsDone
    {
      get
      {
        try
        {
          if (this.ProcessorDefinition.ProcessAlways)
          {
            this.State = ProcessorState.Waiting;
          }

          return this.State == ProcessorState.Done;
        }
        catch (Exception ex)
        {
          Log.Warn("An error occurred during calculating processor.IsDone property", this, ex);

          return false;
        }
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The evaluate steps count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The evaluate steps count. 
    /// </returns>
    public virtual long EvaluateStepsCount([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1;
    }

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <exception cref="ThreadAbortException">
    /// <c>ThreadAbortException</c>
    ///   .
    /// </exception>
    /// <returns>
    /// The execute. 
    /// </returns>
    public bool Execute([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var controller = this.Controller;
      if (!this.IsDone)
      {
        try
        {
          this.State = ProcessorState.Running;
          if (controller != null)
          {
            controller.ProcessorStarted(this.Title);
          }

          this.Process(args);
          if (controller != null)
          {
            controller.ProcessorDone(this.Title);
          }

          this.State = ProcessorState.Done;
          return true;
        }
        catch (ThreadAbortException)
        {
          throw;
        }
        catch (Exception ex)
        {
          this.State = ProcessorState.Error;
          this.Error = ex;
          if (controller != null)
          {
            controller.ProcessorCrashed(ex.Message);
          }
          SIM.Base.Log.Error("Processor of type '{0}' (a part of the '{1}' pipeline) failed. {2}".FormatWith(this.ProcessorDefinition.Type.FullName, this.ProcessorDefinition.OwnerPipelineName, ex.Message), typeof (Processor), ex);
          return false;
        }
      }

      return true;
    }

    /// <summary>
    /// The is require processing.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The is require processing. 
    /// </returns>
    public virtual bool IsRequireProcessing([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    /// <summary>
    ///   The skip.
    /// </summary>
    public void Skip()
    {
      var controller = this.Controller;
      if (controller != null)
      {
        controller.ProcessorSkipped(this.ProcessorDefinition.Title);
      }

      this.State = ProcessorState.Inaccessible;
    }

    /// <summary>
    ///   The to string.
    /// </summary>
    /// <returns> The to string. </returns>
    public override string ToString()
    {
      return this.ProcessorDefinition.With(x => x.ToString()) ?? "(empty processor)";
    }

    #endregion

    #region Methods

    /// <summary>
    ///   Increments the pipeline controller if specified
    /// </summary>
    protected void IncrementProgress()
    {
      var controller = this.Controller;
      if (controller != null)
      {
        controller.IncrementProgress();
      }
    }

    /// <summary>
    /// The process.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    protected abstract void Process([NotNull] ProcessorArgs args);

    #endregion
  }
}