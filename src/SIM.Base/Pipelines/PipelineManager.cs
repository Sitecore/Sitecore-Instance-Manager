﻿namespace SIM.Pipelines
{
  #region

  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Xml;
  using SIM.Pipelines.Processors;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #endregion

  public static class PipelineManager
  {
    #region Fields
    
    public static readonly Dictionary<string, PipelineDefinition> Definitions = new Dictionary<string, PipelineDefinition>();

    #endregion

    #region Public Methods and Operators

    #region Public methods

    [NotNull]
    public static XmlElement GetPipelines(XmlDocumentEx document)
    {
      Assert.ArgumentNotNull(document, nameof(document));

      XmlElement pipelinesNode = document.SelectSingleNode("configuration/pipelines") as XmlElement;
      Assert.IsNotNull(pipelinesNode, "Can't find pipelines configuration node");

      return pipelinesNode;
    }

    public static XmlElement Initialize(string pipelinesConfigFilePath)
    {
      Assert.ArgumentNotNull(pipelinesConfigFilePath, nameof(pipelinesConfigFilePath));

      var document = XmlDocumentEx.LoadFile(pipelinesConfigFilePath);
      XmlElement pipelinesNode = GetPipelines(document);

      return Initialize(pipelinesNode);
    }

    public static XmlElement Initialize(XmlElement pipelinesNode)
    {
      Log.Debug($"Pipelines RAW configuration: {pipelinesNode.OuterXml}");
      Definitions.Clear();
      var resultXmlConfig = XmlDocumentEx.LoadXml("<pipelines />");

      foreach (XmlElement element in pipelinesNode.ChildNodes.OfType<XmlElement>())
      {
        var pipelineName = element.Name;
        var title = element.GetAttribute("title");
        Assert.IsNotNullOrEmpty(title, $"The '{pipelineName}' pipeline definition doesn't contain the title attribute");

        var pipelineNode = resultXmlConfig.DocumentElement.AddElement(pipelineName);
        pipelineNode.SetAttribute("title", title);

        List<StepDefinition> steps = new List<StepDefinition>();
        XmlNodeList stepNodes = element.SelectNodes("step");

        // Processors are placed into steps
        if (stepNodes != null && stepNodes.Count > 0)
        {
          foreach (XmlElement step in stepNodes.OfType<XmlElement>())
          {
            // Clever mechanism of steps reusing. Doesn't seem to be used somewhere.
            var fromPipeline = step.GetAttribute("pipeline");
            var args = step.GetAttribute("args").EmptyToNull();
            if (!string.IsNullOrEmpty(fromPipeline))
            {
              PipelineDefinition def = Definitions[fromPipeline];
              if (args != null)
              {
                def.Steps.ForEach(s => s.ArgsName = args);
              }

              AddSteps(steps, def);
            }
              
              // All step's processors are interted to the step
            else
            {
              var stepNode = pipelineNode.AddElement("step");
              stepNode.SetAttribute("args", args);

              List<ProcessorDefinition> processorDefinitions = ProcessorManager.GetProcessors(step, pipelineName, stepNode);
              AddStep(steps, processorDefinitions);
            }
          }
        }
          
          // Otherwise all the processors are placed into the default step.
        else
        {
          List<ProcessorDefinition> processorDefinitions = ProcessorManager.GetProcessors(element, pipelineName, pipelineNode);
          steps.Add(new StepDefinition(processorDefinitions));
        }

        Definitions.Add(pipelineName, new PipelineDefinition
        {
          Steps = steps, 
          Title = title
        });
      }

      return resultXmlConfig.DocumentElement;
    }

    public static void StartPipeline([NotNull] string pipelineName, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController pipelineController = null, bool isAsync = true)
    {
      Assert.ArgumentNotNull(pipelineName, nameof(pipelineName));
      Assert.ArgumentNotNull(args, nameof(args));

      Log.Info($"Pipeline '{pipelineName}' starts, isAsync: {isAsync.ToString(CultureInfo.InvariantCulture)}");
      using (new ProfileSection("Start pipeline"))
      {
        ProfileSection.Argument("pipelineName", pipelineName);
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("pipelineController", pipelineController);
        ProfileSection.Argument("isAsync", isAsync);

        var pipeline = CreatePipeline(pipelineName, args, pipelineController, isAsync);
        pipeline.Start();
      }
    }

    #endregion

    #region Private methods

    private static void AddStep(List<StepDefinition> steps, List<ProcessorDefinition> processorDefinitions)
    {
      steps.Add(new StepDefinition(processorDefinitions));
    }

    private static void AddSteps(List<StepDefinition> steps, PipelineDefinition def)
    {
      steps.AddRange(def.Steps);
    }

    #endregion

    #endregion

    #region Methods

    [NotNull]
    private static Pipeline CreatePipeline([NotNull] string pipelineName, [NotNull] ProcessorArgs args, [CanBeNull] IPipelineController controller = null, bool isAsync = true)
    {
      Assert.ArgumentNotNull(pipelineName, nameof(pipelineName));
      Assert.ArgumentNotNull(args, nameof(args));

      using (new ProfileSection("Create pipeline"))
      {
        ProfileSection.Argument("pipelineName", pipelineName);
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("controller", controller);
        ProfileSection.Argument("isAsync", isAsync);

        Assert.IsTrue(Definitions.ContainsKey(pipelineName), $"The {pipelineName} pipeline defintion does not exist");
        PipelineDefinition definition = Definitions[pipelineName];

        Pipeline pipeline = new Pipeline(definition, args, controller, isAsync);
        if (controller != null)
        {
          controller.Pipeline = pipeline;
        }

        return ProfileSection.Result(pipeline);
      }
    }

    #endregion
  }
}