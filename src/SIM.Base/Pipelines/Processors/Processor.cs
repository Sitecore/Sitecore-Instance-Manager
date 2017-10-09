namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using System.Threading;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public abstract class Processor : DataObject
  {
    #region Fields

    public readonly List<Processor> _NestedProcessors = new List<Processor>();

    #endregion

    #region Constructors

    protected Processor()
    {
      State = ProcessorState.Waiting;
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
        object value = GetValue("State");
        Assert.IsNotNull(value, nameof(value));

        return (ProcessorState)value;
      }

      private set
      {
        SetValue("State", value);
      }
    }

    [NotNull]
    public string Title
    {
      get
      {
        return ProcessorDefinition.Title;
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
          if (ProcessorDefinition.ProcessAlways)
          {
            State = ProcessorState.Waiting;
          }

          return State == ProcessorState.Done;
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
      Assert.ArgumentNotNull(args, nameof(args));

      return 1;
    }

    public bool Execute([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      var controller = Controller;
      if (!IsDone)
      {
        try
        {
          State = ProcessorState.Running;
          if (controller != null)
          {
            controller.ProcessorStarted(Title);
          }

          Process(args);
          if (controller != null)
          {
            controller.ProcessorDone(Title);
          }

          State = ProcessorState.Done;
          return true;
        }
        catch (ThreadAbortException)
        {
          throw;
        }
        catch (Exception ex)
        {
          State = ProcessorState.Error;
          Error = ex;
          if (controller != null)
          {
            controller.ProcessorCrashed(ex.Message);
          }

          Log.Error(ex, $"Processor of type '{ProcessorDefinition.Type.FullName}' failed. {ex.Message}");
          return false;
        }
      }

      return true;
    }

    public virtual bool IsRequireProcessing([NotNull] ProcessorArgs args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      return true;
    }

    public void Skip()
    {
      var controller = Controller;
      if (controller != null)
      {
        controller.ProcessorSkipped(ProcessorDefinition.Title);
      }

      State = ProcessorState.Inaccessible;
    }

    public override string ToString()
    {
      return ProcessorDefinition.With(x => x.ToString()) ?? "(empty processor)";
    }

    #endregion

    #region Methods

    protected void IncrementProgress()
    {
      var controller = Controller;
      if (controller != null)
      {
        controller.IncrementProgress();
      }
    }

    protected abstract void Process([NotNull] ProcessorArgs args);

    #endregion
  }
}