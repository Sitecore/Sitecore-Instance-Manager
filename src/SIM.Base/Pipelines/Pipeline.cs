namespace SIM.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Threading;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public sealed class Pipeline
  {
    #region Fields

    [CanBeNull]
    private readonly IPipelineController controller;

    private readonly bool isAsync;

    [NotNull]
    private readonly PipelineDefinition pipelineDefinition;

    [NotNull]
    private readonly ProcessorArgs processorArgs;

    [NotNull]
    private readonly List<Step> steps;

    [NotNull]
    private readonly string title;

    [CanBeNull]
    private Thread thread;

    #endregion

    #region Constructors

    public Pipeline([NotNull] PipelineDefinition pipelineDefinition, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null, bool isAsync = true)
    {
      Assert.ArgumentNotNull(pipelineDefinition, "pipelineDefinition");
      Assert.ArgumentNotNull(args, "args");

      this.controller = controller;
      this.pipelineDefinition = pipelineDefinition;
      this.title = pipelineDefinition.Title;
      this.steps = Step.CreateSteps(this.pipelineDefinition.Steps, args, controller);
      this.isAsync = isAsync;

      // Storing args for restarting pipeline
      this.processorArgs = args;
    }

    #endregion

    #region Public Methods

    [NotNull]
    public static string ReplaceVariables([NotNull] string message, [NotNull] AbstractArgs args)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(args, "args");

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
      if (this.thread != null && this.thread.IsAlive)
      {
        this.thread.Abort();
      }
    }

    public void Restart()
    {
      using (new ProfileSection("Restart pipeline"))
      {
        this.Start();
      }
    }

    #endregion

    #region Methods

    #region Public methods

    public void Dispose()
    {
      this.processorArgs.Dispose();
    }

    public void Start()
    {
      using (new ProfileSection("Start pipeline", this))
      {
        if (this.controller != null)
        {
          this.controller.Start(ReplaceVariables(this.title, this.processorArgs), this.steps);
        }

        Assert.IsTrue(this.thread == null || !this.thread.IsAlive, "The previous thread didn't complete its job");

        var pipelineStartInfo = new PipelineStartInfo(this.processorArgs, this.steps, this.controller);
        if (this.isAsync)
        {
          this.thread = new Thread(Execute);
          this.thread.SetApartmentState(ApartmentState.STA);

          // Calls the Execute method in thread  
          this.thread.Start(pipelineStartInfo);
          return;
        }

        Execute(pipelineStartInfo);
      }
    }

    #endregion

    #region Private methods

    private static void Execute([NotNull] object obj)
    {
      Assert.ArgumentNotNull(obj, "obj");

      Pipeline.Execute((PipelineStartInfo)obj);
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
            info.PipelineController.Maximum = ProcessorManager.GetProcessorsCount(info.ProcessorArgs, info.Steps);
          }

          bool result = ExecuteSteps(info.ProcessorArgs, info.Steps, info.PipelineController);

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
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(processorList, "processorList");

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
          result &= ExecuteProcessors(args, processor.NestedProcessors, controller, processorResult);
        }

        return ProfileSection.Result(result);
      }
    }

    private static bool ExecuteSteps([NotNull] ProcessorArgs args, [NotNull] IEnumerable<Step> steps, [CanBeNull] IPipelineController controller = null, bool startThisAndNestedProcessors = true)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(steps, "steps");

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
          string argsName = step.ArgsName.EmptyToNull();
          if (argsName != null)
          {
            Type type = args.GetType();
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            FieldInfo field = type.GetField(argsName) ?? type.GetField(argsName, flags);
            if (field != null)
            {
              innerArgs = (ProcessorArgs)field.GetValue(args);
            }
            else
            {
              PropertyInfo property = type.GetProperty(argsName) ?? type.GetProperty(argsName, flags);
              if (property != null)
              {
                innerArgs = (ProcessorArgs)property.GetValue(args, new object[0]);
              }
            }

            Assert.IsNotNull(innerArgs, "Inner args are null, " + argsName);
          }

          startThisAndNestedProcessors = ExecuteProcessors(innerArgs ?? args, step.Processors, controller, 
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

      public readonly IPipelineController PipelineController;

      public readonly ProcessorArgs ProcessorArgs;

      public readonly List<Step> Steps;

      #endregion

      #region Constructors

      public PipelineStartInfo([NotNull] ProcessorArgs processorArgs, [NotNull] List<Step> steps, [CanBeNull] IPipelineController pipelineController = null)
      {
        Assert.ArgumentNotNull(processorArgs, "processorArgs");
        Assert.ArgumentNotNull(steps, "steps");

        this.ProcessorArgs = processorArgs;
        this.PipelineController = pipelineController;
        this.Steps = steps;
      }

      #endregion
    }

    #endregion
  }
}