namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public abstract class Processor : DataObject
  {
    #region Fields

    public readonly List<Processor> NestedProcessors = new List<Processor>();

    #endregion

    #region Constructors

    protected Processor()
    {
      this.State = ProcessorState.Waiting;
    }

    #endregion

    #region Public Properties

    [CanBeNull]
    public Exception Error { get; private set; }

    [NotNull]
    public ProcessorDefinition ProcessorDefinition { get; set; }

    public ProcessorState State
    {
      get
      {
        object value = this.GetValue("State");
        Assert.IsNotNull(value, "value");

        return (ProcessorState)value;
      }

      private set
      {
        this.SetValue("State", value);
      }
    }

    [NotNull]
    public string Title
    {
      get
      {
        return this.ProcessorDefinition.Title;
      }
    }

    #endregion

    #region Properties

    #region Private properties

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
          Log.Warn(ex, "An error occurred during calculating processor.IsDone property");

          return false;
        }
      }
    }

    #endregion

    protected internal IPipelineController Controller { get; internal set; }

    #endregion

    #region Public Methods and Operators

    public virtual long EvaluateStepsCount([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return 1;
    }

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

          Log.Error(ex, "Processor of type '{0}' failed. {1}", this.ProcessorDefinition.Type.FullName, ex.Message);
          return false;
        }
      }

      return true;
    }

    public virtual bool IsRequireProcessing([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      return true;
    }

    public void Skip()
    {
      var controller = this.Controller;
      if (controller != null)
      {
        controller.ProcessorSkipped(this.ProcessorDefinition.Title);
      }

      this.State = ProcessorState.Inaccessible;
    }

    public override string ToString()
    {
      return this.ProcessorDefinition.With(x => x.ToString()) ?? "(empty processor)";
    }

    #endregion

    #region Methods

    protected void IncrementProgress()
    {
      var controller = this.Controller;
      if (controller != null)
      {
        controller.IncrementProgress();
      }
    }

    protected abstract void Process([NotNull] ProcessorArgs args);

    #endregion
  }
}