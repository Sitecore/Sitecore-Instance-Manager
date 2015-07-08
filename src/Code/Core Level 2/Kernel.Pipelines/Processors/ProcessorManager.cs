#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using SIM.Base;

#endregion

namespace SIM.Pipelines.Processors
{

  #region

  #endregion

  /// <summary>
  ///   The processor manager.
  /// </summary>
  public static class ProcessorManager
  {
    #region Public Methods and Operators

    /// <summary>
    /// The create processors.
    /// </summary>
    /// <param name="processorDefinitions">
    /// The processor definitions. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static List<Processor> CreateProcessors([NotNull] IEnumerable<ProcessorDefinition> processorDefinitions, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(processorDefinitions, "processorDefinitions");
      Assert.ArgumentNotNull(args, "args");

      return new List<Processor>(CreateProcessorsPrivate(processorDefinitions, args, controller));
    }

    /// <summary>
    /// The get processors.
    /// </summary>
    /// <param name="container">
    /// The container. 
    /// </param>
    /// <param name="pipelineName">
    /// The pipeline name. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    public static List<ProcessorDefinition> GetProcessors([NotNull] XmlElement container, [NotNull] string pipelineName, XmlElement pipelineElement)
    {
      Assert.ArgumentNotNull(container, "container");
      Assert.ArgumentNotNull(pipelineName, "pipelineName");

      XmlNodeList processorNodes = container.SelectNodes("*");

      Assert.IsNotNull(processorNodes, "can't find " + pipelineName + "'s processors");
      Assert.IsTrue(processorNodes.Count > 0, "The " + pipelineName + " pipeline doesn't contain any nodes");

      var processorDefinitions = new List<ProcessorDefinition>();
      ParseProcessorDefinitions(processorNodes, processorDefinitions, pipelineElement);
      foreach (ProcessorDefinition definition in processorDefinitions)
      {
        definition.OwnerPipelineName = pipelineName;
      }
      return processorDefinitions;
    }

    /// <summary>
    /// The get processors count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    /// <returns>
    /// The get processors count. 
    /// </returns>
    public static long GetProcessorsCount([NotNull] ProcessorArgs args, [NotNull] List<Step> steps)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(steps, "steps");

      return steps.Select(step => step.Processors).Select(list => GetProcessorsCount(args, list)).Sum();
    }

    /// <summary>
    /// The get processors count.
    /// </summary>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="list">
    /// The list. 
    /// </param>
    /// <returns>
    /// The get processors count. 
    /// </returns>
    public static long GetProcessorsCount([NotNull] ProcessorArgs args, [NotNull] List<Processor> list)
    {
      Assert.ArgumentNotNull(args, "args");
      Assert.ArgumentNotNull(list, "list");

      return list.Sum(item =>
      {
        try
        {
          return item.EvaluateStepsCount(args);
        }
        catch (Exception ex)
        {
          Log.Error("Error during evaluating steps count of " + item.GetType().FullName, typeof(ProcessorManager), ex);
          return 1;
        }
      }) + list.Sum(q => GetProcessorsCount(args, q.NestedProcessors));
    }

    #endregion

    #region Methods

    /// <summary>
    /// The create processors private.
    /// </summary>
    /// <param name="processorDefinitions">
    /// The processor definitions. 
    /// </param>
    /// <param name="args">
    /// The args. 
    /// </param>
    /// <param name="controller">
    /// The controller. 
    /// </param>
    /// <returns>
    /// </returns>
    [NotNull]
    private static IEnumerable<Processor> CreateProcessorsPrivate([NotNull] IEnumerable<ProcessorDefinition> processorDefinitions, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller)
    {
      Assert.ArgumentNotNull(processorDefinitions, "processorDefinitions");
      Assert.ArgumentNotNull(args, "args");

      foreach (ProcessorDefinition processorDefinition in processorDefinitions)
      {
        foreach (Processor processor in processorDefinition.CreateProcessors(args))
        {
          processor.ProcessorDefinition = processorDefinition;
          bool isRequireProcessing = true;
          try
          {
            isRequireProcessing = processor.IsRequireProcessing(args);
          }
          catch (Exception ex)
          {
            SIM.Base.Log.Warn("Cannot detect if the processor {0} requires processing".FormatWith(processor.ProcessorDefinition.Type), typeof (ProcessorManager), ex);
          }

          if (isRequireProcessing)
          {
            List<Processor> nested = CreateProcessors(processorDefinition.NestedProcessorDefinitions, args, controller);
            processor.NestedProcessors.AddRange(nested);
            processor.Controller = controller;
            yield return processor;
          }
        }
      }
    }

    /// <summary>
    /// The parse processor definition.
    /// </summary>
    /// <param name="processorElement">
    /// The processor Element. 
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Can't instantiate processor definition: {0}
    /// </exception>
    [NotNull]
    private static ProcessorDefinition ParseProcessorDefinition([NotNull] XmlElement processorElement)
    {
      Assert.ArgumentNotNull(processorElement, "processorElement");

      string typeNameValue = processorElement.GetAttribute("type");
      string paramValue = processorElement.GetAttribute("param");
      string titleValue = processorElement.GetAttribute("title");
      string process = processorElement.GetAttribute("process");
      
      try
      {
        Type type = Type.GetType(typeNameValue);
        Assert.IsNotNull(type, "Can't find the '{0}' type".FormatWith(typeNameValue));

        ProcessorDefinition definition;
        var name = processorElement.Name;
        if (name.EqualsIgnoreCase("processor"))
        {
          definition = new SingleProcessorDefinition();
        }
          //Support for dynamic processors (e.g. processors are created dynamically)
        else
        {
          Assert.IsTrue(name.EqualsIgnoreCase("hive"), "The {0} element is not supported".FormatWith(name));
          definition = new MultipleProcessorDefinition();
        }
        definition.Type = type;
        definition.Param = paramValue;
        definition.ProcessAlways = process.EqualsIgnoreCase("always");
        definition.Title = titleValue;

        return definition;
      }
      catch (Exception ex)
      {
        throw new InvalidOperationException("Can't instantiate processor definition: {0}".FormatWith(typeNameValue), ex);
      }
    }

    /// <summary>
    /// The parse processor definitions.
    /// </summary>
    /// <param name="processorNodes">
    /// The processor nodes. 
    /// </param>
    /// <param name="processorDefinitions">
    /// The processor definitions. 
    /// </param>
    private static void ParseProcessorDefinitions([NotNull] XmlNodeList processorNodes, [NotNull] List<ProcessorDefinition> processorDefinitions, XmlElement parentNode)
    {
      Assert.ArgumentNotNull(processorNodes, "processorNodes");
      Assert.ArgumentNotNull(processorDefinitions, "processorDefinitions");

      foreach (XmlElement processorElement in processorNodes.OfType<XmlElement>())
      {
        ProcessorDefinition definition = ParseProcessorDefinition(processorElement);
        
        var definitionNode = parentNode.AddElement("processor");
        definitionNode.SetAttribute("type", definition.Type.FullName);
        definitionNode.SetAttribute("title", definition.Title);
        definitionNode.SetAttribute("process", definition.ProcessAlways ? "always" : "once");
        definitionNode.SetAttribute("param", definition.Param);

        // add nested processor definitions
        ParseProcessorDefinitions(processorElement.ChildNodes, definition.NestedProcessorDefinitions, definitionNode);
        processorDefinitions.Add(definition);
      }
    }

    #endregion
  }
}