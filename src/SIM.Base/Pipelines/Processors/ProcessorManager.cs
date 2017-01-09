namespace SIM.Pipelines.Processors
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public static class ProcessorManager
  {
    #region Public Methods and Operators

    [NotNull]
    public static List<Processor> CreateProcessors([NotNull] IEnumerable<ProcessorDefinition> processorDefinitions, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null)
    {
      Assert.ArgumentNotNull(processorDefinitions, nameof(processorDefinitions));
      Assert.ArgumentNotNull(args, nameof(args));

      return new List<Processor>(CreateProcessorsPrivate(processorDefinitions, args, controller));
    }

    [NotNull]
    public static List<ProcessorDefinition> GetProcessors([NotNull] XmlElement container, [NotNull] string pipelineName, XmlElement pipelineElement)
    {
      Assert.ArgumentNotNull(container, nameof(container));
      Assert.ArgumentNotNull(pipelineName, nameof(pipelineName));

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

    public static long GetProcessorsCount([NotNull] ProcessorArgs args, [NotNull] List<Step> steps)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.ArgumentNotNull(steps, nameof(steps));

      return steps.Select(step => step.Processors).Select(list => GetProcessorsCount(args, list)).Sum();
    }

    public static long GetProcessorsCount([NotNull] ProcessorArgs args, [NotNull] List<Processor> list)
    {
      Assert.ArgumentNotNull(args, nameof(args));
      Assert.ArgumentNotNull(list, nameof(list));

      return list.Sum(item =>
      {
        long itemStepsCount = 1;
        try
        {
          itemStepsCount = item.EvaluateStepsCount(args);
        }
        catch (Exception ex)
        {
          Log.Error(ex, $"Error during evaluating steps count of {item.GetType().FullName}");
        }

        return itemStepsCount + GetProcessorsCount(args, item.NestedProcessors);
      });
    }

    #endregion

    #region Methods

    [NotNull]
    private static IEnumerable<Processor> CreateProcessorsPrivate([NotNull] IEnumerable<ProcessorDefinition> processorDefinitions, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller)
    {
      Assert.ArgumentNotNull(processorDefinitions, nameof(processorDefinitions));
      Assert.ArgumentNotNull(args, nameof(args));

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
            Log.Warn(ex, $"Cannot detect if the processor {processor.ProcessorDefinition.Type} requires processing");
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

    [NotNull]
    private static ProcessorDefinition ParseProcessorDefinition([NotNull] XmlElement processorElement)
    {
      Assert.ArgumentNotNull(processorElement, nameof(processorElement));

      var typeNameValue = processorElement.GetAttribute("type");
      var paramValue = processorElement.GetAttribute("param");
      var titleValue = processorElement.GetAttribute("title");
      var process = processorElement.GetAttribute("process");

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
          
          // Support for dynamic processors (e.g. processors are created dynamically)
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

    private static void ParseProcessorDefinitions([NotNull] XmlNodeList processorNodes, [NotNull] List<ProcessorDefinition> processorDefinitions, XmlElement parentNode)
    {
      Assert.ArgumentNotNull(processorNodes, nameof(processorNodes));
      Assert.ArgumentNotNull(processorDefinitions, nameof(processorDefinitions));

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