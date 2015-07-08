#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   The pipeline.
  /// </summary>
  public sealed class Pipeline
  {
    #region Fields

    /// <summary>
    ///   The controller.
    /// </summary>
    [CanBeNull]
    private readonly IPipelineController controller;

    /// <summary>
    /// The is async.
    /// </summary>
    private readonly bool isAsync;

    /// <summary>
    ///   The pipeline definition.
    /// </summary>
    [NotNull]
    private readonly PipelineDefinition pipelineDefinition;

    /// <summary>
    ///   The processor args.
    /// </summary>
    [NotNull]
    private readonly ProcessorArgs processorArgs;

    /// <summary>
    ///   The steps.
    /// </summary>
    [NotNull]
    private readonly List<Step> steps;

    /// <summary>
    ///   The title.
    /// </summary>
    [NotNull]
    private readonly string title;

    /// <summary>
    ///   The thread.
    /// </summary>
    [CanBeNull]
    private Thread thread;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Pipeline"/> class. The Pipeline is instantiated only once per pipeline start
    /// </summary>
    /// <param name="pipelineDefinition">
    /// The pipeline Definition. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <param name="isAsync">
    /// </param>
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

    /// <summary>
    /// The replace variables.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <returns>
    /// The replace variables. 
    /// </returns>
    [NotNull]
    public static string ReplaceVariables([NotNull] string message, [NotNull] AbstractArgs args)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(args, "args");
          
      using (new ProfileSection("Replace pipeline variables in message", typeof(Pipeline)))
      {
        ProfileSection.Argument("message", message);
        ProfileSection.Argument("args", args);

        Type type = args.GetType();
        foreach (var propertyName in message.Extract('{', '}', false))
        {
          var propertyValue = type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance).GetValue(args, new object[0]) as string;
          if(propertyValue != null)
          {
            message = message.Replace('{' + propertyName + '}', propertyValue);
          }
        }

        return ProfileSection.Result(message); 
      }              
    }

    /// <summary>
    ///   The abort.
    /// </summary>
    public void Abort()
    {
      if (this.thread != null && this.thread.IsAlive)
      {
        this.thread.Abort();
      }
    }

    /// <summary>
    ///   Restarts the pipeline
    /// </summary>
    public void Restart()
    {
      using (new ProfileSection("Restart pipeline", typeof(Pipeline)))
      {
        this.Start(); 
      }
    }

    #endregion

    #region Methods

    #region Private methods

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="obj">
    /// The obj. 
    /// </param>
    private static void Execute([NotNull] object obj)
    {
      Assert.ArgumentNotNull(obj, "obj");

      Pipeline.Execute((PipelineStartInfo)obj);       
    }

    private static void Execute([NotNull] PipelineStartInfo info)
    {
      using (new ProfileSection("Execute pipeline processors", typeof (Pipeline)))
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
        catch(Exception ex)
        {
          Log.Warn("An error occurred during executing a pipeline", typeof(Pipeline), ex);
          info.ProcessorArgs.Dispose();
        }
      }
    }

    /// <summary>
    /// The execute processors.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="processorList">
    /// The processor steps. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <param name="startThisAndNestedProcessors">
    /// The start this and nested processors. 
    /// </param>
    /// <returns>
    /// The execute processors. 
    /// </returns>
    private static bool ExecuteProcessors([NotNull] ProcessorArgs args, [NotNull] IEnumerable<Processor> processorList, [CanBeNull] IPipelineController controller = null, bool startThisAndNestedProcessors = true)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(processorList, "processorList");

      using (new ProfileSection("Execute pipeline processors", typeof(Pipeline)))
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

    /// <summary>
    /// The execute steps.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <param name="startThisAndNestedProcessors">
    /// The start this and nested processors. 
    /// </param>
    /// <returns>
    /// The execute steps. 
    /// </returns>
    private static bool ExecuteSteps([NotNull] ProcessorArgs args, [NotNull] IEnumerable<Step> steps, [CanBeNull] IPipelineController controller = null, bool startThisAndNestedProcessors = true)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(steps, "steps");

      using (new ProfileSection("Execute pipeline steps", typeof(Pipeline)))
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
              innerArgs = (ProcessorArgs) field.GetValue(args);
            }
            else
            {
              PropertyInfo property = type.GetProperty(argsName) ?? type.GetProperty(argsName, flags);
              if (property != null)
              {
                innerArgs = (ProcessorArgs) property.GetValue(args, new object[0]);
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

    /// <summary>
    ///   The dispose.
    /// </summary>
    public void Dispose()
    {
      this.processorArgs.Dispose();
    }

    /// <summary>
    ///   The method is called only once per pipeline start
    /// </summary>
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

    #region Nested type: PipelineStartInfo

    /// <summary>
    ///   The pipeline start info.
    /// </summary>
    private class PipelineStartInfo
    {
      #region Fields

      /// <summary>
      ///   The pipeline controller.
      /// </summary>
      public readonly IPipelineController PipelineController;

      /// <summary>
      ///   The processor args.
      /// </summary>
      public readonly ProcessorArgs ProcessorArgs;

      /// <summary>
      ///   The steps.
      /// </summary>
      public readonly List<Step> Steps;

      #endregion

      #region Constructors

      /// <summary>
      /// Initializes a new instance of the <see cref="PipelineStartInfo"/> class.
      /// </summary>
      /// <param name="processorArgs">
      /// The processor args. 
      /// </param>
      /// <param name="steps">
      /// The steps. 
      /// </param>
      /// <param name="pipelineController">
      /// The pipeline controller. 
      /// </param>
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