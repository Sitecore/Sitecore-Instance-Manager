namespace SIM.Tool.Base.Wizards
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Windows;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Windows;

  #region

  #endregion

  public static class WizardPipelineManager
  {
    #region Fields

    private static readonly Dictionary<string, WizardPipeline> Definitions = new Dictionary<string, WizardPipeline>();

    #endregion

    #region Public methods

    public static IEnumerable<FinishAction> GetFinishActions(XmlElement finish, Type args)
    {
      var elements = finish.ChildNodes.OfType<XmlElement>();
      var actions = elements.Where(qq => qq.Name.EqualsIgnoreCase("action"));
      foreach (var ch in actions)
      {
        var finishAction = ParseFinishAction(args, ch);
        if (finishAction != null)
        {
          yield return finishAction;
        }
      }
    }

    public static FinishAction ParseFinishAction(Type args, XmlElement ch)
    {
      var text = ch.GetAttribute("text");
      if (string.IsNullOrEmpty(text))
      {
        Log.Error("Finish action doesn't have 'text' specified: {0}", ch.OuterXml);
        return null;
      }

      var typeName = ch.GetAttribute("type");
      if (string.IsNullOrEmpty(typeName))
      {
        Log.Error("Finish action doesn't have 'type' specified: {0}", ch.OuterXml);
        return null;
      }

      var type = Type.GetType(typeName);
      if (type == null)
      {
        Log.Error("Finish action points to missing '{0}' type: {1}", typeName, ch.OuterXml);
        return null;
      }

      var methodName = ch.GetAttribute("method");
      if (string.IsNullOrEmpty(methodName))
      {
        Log.Error("Finish action doesn't have 'method' specified: {0}", ch.OuterXml);
        return null;
      }

      var method = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, args != null ? new[]
      {
        args
      } : new Type[0], null);
      if (method == null)
      {
        Log.Error("Finish action points to missing '{0}' method of the '{1}' type: {2}", methodName, typeName, ch.OuterXml);
        return null;
      }

      return new FinishAction(text, method);
    }

    #endregion

    #region Public Methods

    #region Fields

    private static readonly Func<WizardPipeline, ProcessorArgs, object[], Wizard> CreateWizardWindow = (a, b, c) => new Wizard(a, b, c);
    private static bool flag;

    #endregion

    #region Public methods

    public static void Start(string name, Window owner, ProcessorArgs args = null, bool? isAsync = null, Action action = null, params object[] wizardArgsParameters)
    {
      Log.Info("Wizard pipeline '{0}' starts", name);
      using (new ProfileSection("Start wizard"))
      {
        ProfileSection.Argument("name", name);
        ProfileSection.Argument("owner", owner);
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("wizardArgsParameters", wizardArgsParameters);

        WizardPipeline wizardPipeline = Definitions[name];
        var wizard = CreateWizardWindow(wizardPipeline, args, wizardArgsParameters);
        var isSync = !(isAsync ?? !WinAppSettings.AppSysIsSingleThreaded.Value);
        if (isSync)
        {
          WindowHelper.ShowDialog(wizard, owner);
          if (action != null)
          {
            action();
          }
        }
        else
        {
          if (action != null && !flag)
          {
            flag = true;
            wizard.Closed += (o, e) =>
            {
              action();
              flag = false;
            };
          }

          WindowHelper.ShowWindow(wizard, owner);
        }
      }
    }

    #endregion

    #endregion

    #region Private methods

    private static FinishActionHive[] GetFinishActionHives(XmlElement finish, Type args)
    {
      return finish
        .With(f => f.ChildNodes.OfType<XmlElement>()
          .Where(qq => qq.Name.EqualsIgnoreCase("hive"))
          .Select(
            ww =>
              (FinishActionHive)
                ReflectionUtil.CreateObject(
                  ww.GetAttribute("type").EmptyToNull().IsNotNull(
                    "The type attribute of the {0} element is null or empty".FormatWith(ww.OuterXml)),
                  args))
          .ToArray()
        );
    }

    public static void Initialize(XmlElement wizardPipelinesElement)
    {
      Assert.ArgumentNotNull(wizardPipelinesElement, "wizardPipelinesElement");

      using (new ProfileSection("Initialize Wizard Pipeline Manager"))
      {
        try
        {
          var wizardPipelines = wizardPipelinesElement.SelectSingleNode("/configuration/wizardPipelines") as XmlElement;
          Assert.IsNotNull(wizardPipelines, "wizardPipelines");

          var wizardElements = wizardPipelines.ChildNodes.OfType<XmlElement>();
          foreach (XmlElement element in wizardElements)
          {
            InitializeWizardPipeline(element);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("WizardPipelineManager initialization failed with exception", true, ex);
        }
      }
    }

    private static void InitializeWizardPipeline(XmlElement element)
    {
      using (new ProfileSection("Initialize wizard pipeline"))
      {
        ProfileSection.Argument("element", element);

        string name1 = element.Name;
        try
        {
          XmlElement argsElement = element.SelectSingleElement("args");
          Type args = argsElement != null
            ? Type.GetType(argsElement.GetAttribute("type")).IsNotNull(
              "Cannot find the {0} type".FormatWith(argsElement.GetAttribute("type")))
            : null;
          XmlElement finish = element.SelectSingleElement("finish");
          string title = element.GetAttribute("title");
          var steps =
            element.SelectSingleElement("steps").IsNotNull(
              "Can't find the steps element in the WizardPipelines.config file").ChildNodes.OfType<XmlElement>().
              Select(
                step =>
                  new StepInfo(step.GetAttribute("name"), Type.GetType(step.GetAttribute("type")),
                    step.GetAttribute("param"))).ToArray();
          string cancelButtonText = element.GetAttribute("cancelButton");
          string startButtonText = element.GetAttribute("startButton");
          string finishText = element.GetAttribute("finishText");
          FinishAction[] finishActions = finish != null ? GetFinishActions(finish, args).ToArray() : null;
          var finishActionHives = GetFinishActionHives(finish, args);
          WizardPipeline wizardPipeline = new WizardPipeline(name1, title, steps, args, startButtonText,
            cancelButtonText, finishText, finishActions,
            finishActionHives);
          Definitions.Add(name1, wizardPipeline);

          ProfileSection.Result("Done");
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("WizardPipelineManager failed to load the {0} pipeline".FormatWith(name1), true, ex);

          ProfileSection.Result("Failed");
        }
      }
    }

    #endregion
  }
}