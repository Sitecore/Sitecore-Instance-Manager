namespace SIM.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Threading;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public sealed class Pipeline
  {
    #region Fields

    [CanBeNull]
    private IPipelineController Controller { get; }

    private bool IsAsync { get; }

    [NotNull]
    private PipelineDefinition PipelineDefinition { get; }

    [NotNull]
    private ProcessorArgs ProcessorArgs { get; }

    [NotNull]
    private readonly List<Step> _Steps;

    [NotNull]
    private string Title { get; }

    [CanBeNull]
    private Thread _Thread;

    #endregion

    #region Constructors

    public Pipeline([NotNull] PipelineDefinition pipelineDefinition, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null, bool isAsync = true)
    {
      Assert.ArgumentNotNull(pipelineDefinition, nameof(pipelineDefinition));
      Assert.ArgumentNotNull(args, nameof(args));

      Controller = controller;
      PipelineDefinition = pipelineDefinition;
      Title = pipelineDefinition.Title;
      _Steps = Step.CreateSteps(PipelineDefinition.Steps, args, controller);
      IsAsync = isAsync;

      // Storing args for restarting pipeline
      ProcessorArgs = args;
    }

    #endregion

    #region Public Methods

    [NotNull]
    public static string ReplaceVariables([NotNull] string message, [NotNull] object args)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(args, nameof(args));

      using (new ProfileSection("Replace pipeline variables in message"))
      {
        ProfileSection.Argument("message", message);
        ProfileSection.Argument("args", args);

        Type type = args.GetType();
        foreach (var propertyName in message.Extract('{', '}', false))
        {
          var propertyValue = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(args, new object[0]) as string;
          if (propertyValue != null)
          {
            message = message.Replace('{' + propertyName + '}', propertyValue);
          }
        }

        return ProfileSection.Result(message);
      }
    }

    public void Abort()
    {
      if (_Thread != null && _Thread.IsAlive)
      {
        _Thread.Abort();
      }
    }

    public void Restart()
    {
      using (new ProfileSection("Restart pipeline"))
      {
        Start();
      }
    }

    #endregion

    #region Methods

    #region Public methods

    public void Dispose()
    {
      ProcessorArgs.Dispose();
    }

    public void Start()
    {
      using (new ProfileSection("Start pipeline", this))
      {
        if (Controller != null)
        {
          Controller.Start(ReplaceVariables(Title, ProcessorArgs), _Steps);
        }

        Assert.IsTrue(_Thread == null || !_Thread.IsAlive, "The previous thread didn't complete its job");

        var pipelineStartInfo = new PipelineStartInfo(ProcessorArgs, _Steps, Controller);
        if (IsAsync)
        {
          _Thread = new Thread(Execute);
          _Thread.SetApartmentState(ApartmentState.STA);

          // Calls the Execute method in thread  
          _Thread.Start(pipelineStartInfo);
          return;
        }

        Execute(pipelineStartInfo);
      }
    }

    #endregion

    #region Private methods

    private static void Execute([NotNull] object obj)
    {
      Assert.ArgumentNotNull(obj, nameof(obj));

      Execute((PipelineStartInfo)obj);
    }

    private static void Execute([NotNull] PipelineStartInfo info)
    {
      using (new ProfileSection("Execute pipeline processors"))
      {
        ProfileSection.Argument("info", info);

        try
        {
          if (info.PipelineController != null)
          {
            info.PipelineController.Maximum = ProcessorManager.GetProcessorsCount(info.ProcessorArgs, info._Steps);
          }

          bool result = ExecuteSteps(info.ProcessorArgs, info._Steps, info.PipelineController);

          if (info.PipelineController != null)
          {
            info.PipelineController.Finish("Done.", result);
          }

          if (result)
          {
            info.ProcessorArgs.FireOnCompleted();
            info.ProcessorArgs.Dispose();
          }
        }
        catch (Exception ex)
        {
          Log.Warn(ex, "An error occurred during executing a pipeline");
          info.ProcessorArgs.Dispose();
        }
      }
    }

    private static bool ExecuteProcessors([NotNull] ProcessorArgs args, [NotNull] IEnumerable<Processor> processorList, [CanBeNull] IPipelineController controller = null, bool startThisAndNestedProcessors = true)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.ArgumentNotNull(processorList, nameof(processorList));

      using (new ProfileSection("Execute pipeline processors"))
      {
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("processorList", processorList);
        ProfileSection.Argument("controller", controller);
        ProfileSection.Argument("startThisAndNestedProcessors", startThisAndNestedProcessors);

        bool result = startThisAndNestedProcessors;
        foreach (Processor processor in processorList)
        {
          bool processorResult = startThisAndNestedProcessors;

          if (processorResult)
          {
            processorResult = processor.Execute(args);
          }
          else
          {
            processor.Skip();
          }

          if (controller != null)
          {
            controller.IncrementProgress();
          }

          // Process nested steps
          result &= ExecuteProcessors(args, processor._NestedProcessors, controller, processorResult);
        }

        return ProfileSection.Result(result);
      }
    }

    private static bool ExecuteSteps([NotNull] ProcessorArgs args, [NotNull] IEnumerable<Step> steps, [CanBeNull] IPipelineController controller = null, bool startThisAndNestedProcessors = true)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.ArgumentNotNull(steps, nameof(steps));

      using (new ProfileSection("Execute pipeline steps"))
      {
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("steps", steps);
        ProfileSection.Argument("controller", controller);
        ProfileSection.Argument("startThisAndNestedProcessors", startThisAndNestedProcessors);

        foreach (Step step in steps)
        {
          ProcessorArgs innerArgs = null;

          /* Use args' member as args for nested pipeline*/
          var argsName = step.ArgsName.EmptyToNull();
          if (argsName != null)
          {
            Type type = args.GetType();
            const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = type.GetField(argsName) ?? type.GetField(argsName, Flags);
            if (field != null)
            {
              innerArgs = (ProcessorArgs)field.GetValue(args);
            }
            else
            {
              PropertyInfo property = type.GetProperty(argsName) ?? type.GetProperty(argsName, Flags);
              if (property != null)
              {
                innerArgs = (ProcessorArgs)property.GetValue(args, new object[0]);
              }
            }

            Assert.IsNotNull(innerArgs, "Inner args are null, " + argsName);
          }

          startThisAndNestedProcessors = ExecuteProcessors(innerArgs ?? args, step._Processors, controller, 
            startThisAndNestedProcessors);
        }

        return startThisAndNestedProcessors;
      }
    }

    #endregion

    // Called by thread
    #endregion

    #region Nested type: PipelineStartInfo

    private class PipelineStartInfo
    {
      #region Fields

      public IPipelineController PipelineController { get; }

      public ProcessorArgs ProcessorArgs { get; }

      public readonly List<Step> _Steps;

      #endregion

      #region Constructors

      public PipelineStartInfo([NotNull] ProcessorArgs processorArgs, [NotNull] List<Step> steps, [CanBeNull] IPipelineController pipelineController = null)
      {
        Assert.ArgumentNotNull(processorArgs, nameof(processorArgs));
        Assert.ArgumentNotNull(steps, nameof(steps));

        ProcessorArgs = processorArgs;
        PipelineController = pipelineController;
        _Steps = steps;
      }

      #endregion
    }

    #endregion
  }
}