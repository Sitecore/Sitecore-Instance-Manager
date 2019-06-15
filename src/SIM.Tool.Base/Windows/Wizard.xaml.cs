namespace SIM.Tool.Base.Windows
{
  #region

  using System;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Linq;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Documents;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Shell;
  using System.Windows.Threading;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;
  using SIM.Extensions;
  using SIM.Pipelines;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;

  #endregion

  public partial class Wizard : IPipelineController
  {
    #region Fields

    public WizardArgs WizardArgs { get; }

    private ProcessorArgs Args { get; }

    private Brush ProgressBar1Foreground { get; }

    private WizardPipeline WizardPipeline { get; }

    private double _Maximum = double.NaN;
    

    private ProcessorArgs _ProcessorArgs;

    #endregion

    #region Constructors

    public Wizard(WizardPipeline wizardPipeline, ProcessorArgs args, [NotNull] Func<WizardArgs> createWizardArgs)
    {
      using (new ProfileSection("Create wizard instance", this))
      {
        ProfileSection.Argument("wizardPipeline", wizardPipeline);
        ProfileSection.Argument("args", args);
        
        WizardArgs = createWizardArgs();
        if (WizardArgs != null)
        {
          WizardArgs.WizardWindow = this;
        }

        WizardPipeline = wizardPipeline;
        Args = args;
        InitializeComponent();
        ProgressBar1Foreground = progressBar1.Foreground;
        if (!WinAppSettings.AppSysIsSingleThreaded.Value)
        {
          CancelButton.IsCancel = false;
          ResizeMode = ResizeMode.CanMinimize;
        }
      }
    }

    #endregion

    #region Properties

    #region Public properties

    public double Maximum
    {
      get
      {
        return (double)Dispatcher.Invoke(DispatcherPriority.Normal, new Func<double>(GetMaximum));
      }

      set
      {
        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => SetMaximum(value)));
      }
    }

    public Pipeline Pipeline { get; set; }

    [NotNull]
    public List<Processor> Processors
    {
      set
      {
        Assert.ArgumentNotNull(value, nameof(value));

        List<Processor> list = new List<Processor>();
        GetList(value, list);
        DataGrid.DataContext = list;
      }
    }

    #endregion

    #region Private properties

    private int PageNumber
    {
      get
      {
        return TabControl.SelectedIndex;
      }

      set
      {
        TabControl.SelectedIndex = value;
      }
    }

    private double Progress
    {
      get
      {
        return progressBar1.Value;
      }

      set
      {
        progressBar1.Value = value;
      }
    }

    private int StepsCount
    {
      get
      {
        return WizardPipeline._StepInfos.Length;
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public new void Close()
    {
      using (new ProfileSection("Close wizard", this))
      {
        AbortPipeline();
        if (!WinAppSettings.AppSysIsSingleThreaded.Value)
        {
          base.Close();
        }
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IPipelineController

    [CanBeNull]
    public string Ask([NotNull] string title, [NotNull] string defaultValue)
    {
      Assert.ArgumentNotNull(title, nameof(title));

      return WindowHelper.Ask(title, defaultValue, this);
    }

    public bool Confirm([NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, nameof(message));

      return WindowHelper.ShowMessage(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public void Execute([NotNull] string path, string arguments = null)
    {
      Assert.ArgumentNotNull(path, nameof(path));

      if (arguments.EmptyToNull() == null)
      {
        CoreApp.RunApp(path);
        return;
      }

      CoreApp.RunApp(path, arguments);
    }

    public void Finish([NotNull] string message, bool closeInterface)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => FinishUnsafe(message, closeInterface)));
    }

    public void IncrementProgress()
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => IncrementProgressUnsafe()));
    }

    public void Pause()
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(PauseUnsafe));
    }

    public void ProcessorCrashed(string error)
    {
      Assert.ArgumentNotNull(error, nameof(error));
    }

    public void ProcessorDone([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, nameof(title));
    }

    public void ProcessorSkipped([NotNull] string processorName)
    {
      Assert.ArgumentNotNull(processorName, nameof(processorName));
    }

    public void ProcessorStarted([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, nameof(title));

      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => SetStatusUnsafe(title)));
    }

    public void Resume()
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(ResumeUnsafe));
    }

    [CanBeNull]
    public string Select([NotNull] string message, [NotNull] IEnumerable<string> options, bool allowMultipleSelection = false, string defaultValue = null)
    {
      Assert.ArgumentNotNull(message, nameof(message));
      Assert.ArgumentNotNull(options, nameof(options));

      return (string)Dispatcher.Invoke(() => WindowHelper.AskForSelection("Select an option", "Select an option", message, options, this, defaultValue, allowMultipleSelection));
    }

    public void SetProgress(long progress)
    {
      try
      {
        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => SetProgressUnsafe(progress)));
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during setting progress");
      }
    }

    public void SetProgressUnsafe(long progress)
    {
      Progress = progress;
    }

    public void Start(string title, List<Step> steps)
    {
      Assert.ArgumentNotNull(title, nameof(title));
      Assert.ArgumentNotNull(steps, nameof(steps));

      Start(title, steps, 0);
    }

    #endregion

    #endregion

    #region Methods

    #region Private properties

    private double ProgressNormalized
    {
      get
      {
        return Progress / GetMaximum();
      }
    }

    #endregion

    #region Protected methods

    protected void Start([NotNull] string title, [NotNull] List<Step> steps, int value)
    {
      Assert.ArgumentNotNull(title, nameof(title));
      Assert.ArgumentNotNull(steps, nameof(steps));

      HeaderDetails.Text = title;
      Maximum = value;
      List<Processor> p = new List<Processor>();
      foreach (Step step in steps)
      {
        List<Processor> pp = step._Processors;
        p.AddRange(pp);
      }

      Processors = p;
    }

    #endregion

    #region Private methods

    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
      if (depObj != null)
      {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
        {
          DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
          if (child != null && child is T)
          {
            yield return (T)child;
          }

          foreach (T childOfChild in FindVisualChildren<T>(child))
          {
            yield return childOfChild;
          }
        }
      }
    }

    private static void SetActive(UIElement control)
    {
      if (!SetActive<ComboBox>(control))
      {
        if (!SetActive<ListBox>(control))
        {
          SetActive<TextBox>(control);
        }
      }
    }

    private static bool SetActive<T>(UIElement control) where T : DependencyObject, IInputElement
    {
      var textbox = FindVisualChildren<T>(control).FirstOrDefault();
      if (textbox != null)
      {
        FocusManager.SetFocusedElement(textbox, textbox);
        return true;
      }

      return false;
    }

    private void AbortPipeline()
    {
      Pipeline pipeline = Pipeline;
      if (pipeline != null)
      {
        pipeline.Abort();
      }
    }

    private void AddFinishAction(FinishAction action)
    {
      FinishActions.Children.Add(new CheckBox
      {
        Content = action.Text, 
        Tag = action
      });
    }

    private void AddFinishActions(IEnumerable<FinishAction> finishActions)
    {
      if (finishActions != null)
      {
        foreach (FinishAction action in finishActions)
        {
          AddFinishAction(action);
        }
      }
    }

    private void AddStepToWizard(StepInfo stepInfo)
    {
      Type ctrl = stepInfo.Control;
      Assert.IsNotNull(ctrl, "The {0} step contains null as a control".FormatWith(stepInfo.Title));
      var fullName = ctrl.FullName;
      Assert.IsTrue(ctrl.IsClass, "Control {0} is not class".FormatWith(fullName));

      if (!ctrl.GetInterfaces().Contains(typeof(IWizardStep)))
      {
        Log.Debug($"Control {fullName} does not implement IWizardStep");
      }

      var param = stepInfo.Param;
      var wizardStep = (UserControl)(!string.IsNullOrEmpty(param) ? ReflectionUtil.CreateObject(ctrl, param) : ReflectionUtil.CreateObject(ctrl));
      
      TabControl.Items.Insert(TabControl.Items.Count - 2, new TabItem
      {
        AllowDrop = false, 
        Content = wizardStep, 
        Visibility = Visibility.Collapsed
      });
    }

    private void CustomButtonClick(object sender, RoutedEventArgs e)
    {
      var customButtonStep = TabControl.SelectedContent as ICustomButton;
      if (customButtonStep != null)
      {
        customButtonStep.CustomButtonClick();
      }
    }

    private void ErrorClick([CanBeNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(e, nameof(e));

      Hyperlink hyperlink = e.OriginalSource as Hyperlink;
      if (hyperlink != null)
      {
        Processor processor = hyperlink.DataContext as Processor;
        if (processor != null)
        {
          switch (processor.State)
          {
            case ProcessorState.Error:
            case ProcessorState.Inaccessible:
            {
              var messageLabel = $@"{processor.Title} action failed with message: ";
              const string Skipped = "Action was skipped because other one had problem - please find it, fix the problem and run the process again";
                string message;
              Exception exception = processor.Error;
              if (exception != null)
              {
                var exceptionMessage = exception.Message;
                if (exceptionMessage.Contains("cannot be upgraded because it is read-only") || exceptionMessage.Contains(": 15105"))
                {
                  message =
                    $"It seems that the NETWORK SERVICE identity doesn\'t have full access rights to the folder you selected to install the instance to.{Environment.NewLine}{Environment.NewLine}{exceptionMessage}";
                }
                else
                {
                  message = messageLabel + exceptionMessage;
                }
              }
              else
              {
                message = Skipped;
              }

              WindowHelper.HandleError(message, true, null);
              break;
            }
          }
        }
      }
    }

    private void FinishUnsafe([NotNull] string message, bool allDone)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      using (new ProfileSection("Finish wizard (unsafe)", this))
      {
        ProfileSection.Argument("message", message);
        ProfileSection.Argument("allDone", allDone);

        if (allDone)
        {
          WizardPipeline.AfterLastStep?.Execute(this.WizardArgs);
          AddFinishActions(WizardPipeline._FinishActions);

          if (WizardPipeline._FinishActionHives != null)
          {
            foreach (FinishActionHive hive in WizardPipeline._FinishActionHives.NotNull())
            {
              AddFinishActions(hive.GetFinishActions(WizardArgs));
            }
          }

          FinishTextBlock.Text = WizardPipeline.FinishText;
          TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
          Progress = 0;
          NextButton.Visibility = Visibility.Hidden;
          CancelButton.Content = "Finish";
          CancelButton.Focus();
          PageNumber++;
          HeaderDetails.Text = "Completed";
        }
        else
        {
          SetStatusUnsafe("Some steps require your attention");
          TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
          progressBar1.Foreground = Brushes.Red;
          NextButton.IsEnabled = true;
        }
      }
    }

    private void GetList([NotNull] IEnumerable<Processor> value, [NotNull] List<Processor> output)
    {
      Assert.ArgumentNotNull(value, nameof(value));
      Assert.ArgumentNotNull(output, nameof(output));

      foreach (Processor q in value)
      {
        output.Add(q);
        GetList(q._NestedProcessors, output);
      }
    }

    private double GetMaximum()
    {
      return progressBar1.Maximum;
    }

    private void IncrementProgressUnsafe(long value = 1)
    {
      Progress += value;
      UpdateTaskbar();
    }

    private void InitializeIWizardStep(int n)
    {
      using (new ProfileSection("Initializing step", this))
      {
        ProfileSection.Argument("n", n);

        CustomButton.Visibility = Visibility.Hidden;
        TabItem item = TabControl.Items[n] as TabItem;
        Assert.IsNotNull(item, nameof(item));

        var content = item.Content;
        Assert.IsNotNull(content, nameof(content));

        var fullName = content.GetType().FullName;

        var control = content as UIElement;
        if (control == null)
        {
          Log.Warn($"The {fullName} type is not UIElement-based");
          return;
        }

        var customButtonStep = control as ICustomButton;
        bool isVisible = customButtonStep != null;
        if (isVisible)
        {
          var name = customButtonStep.CustomButtonText;
          CustomButton.Content = name ?? string.Empty;
          isVisible = !string.IsNullOrEmpty(name);
        }

        CustomButton.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        var step = control as IWizardStep;

        // Progress or Final step
        if (step == null)
        {
          return;
        }

        var wizardArgs = WizardArgs;
        using (new ProfileSection("Initialize step (inner)", this))
        {
          ProfileSection.Argument("step", step);
          ProfileSection.Argument("wizardArgs", wizardArgs);

          step.InitializeStep(wizardArgs);
        }

        SetActive(control);
      }
    }

    private void InitializeStep(int? i = null)
    {
      using (new ProfileSection("Initializing step", this))
      {
        ProfileSection.Argument("i", i);

        var n = i ?? PageNumber;

        using (new ProfileSection("Set header", this))
        {
          StepInfo[] stepInfos = WizardPipeline._StepInfos;
          if (stepInfos.Length > n)
          {
            var title = stepInfos[n].Title;
            HeaderDetails.Text = ReplaceVariables(title);
          }
        }

        InitializeIWizardStep(n);

        // regular step
        if (PageNumber < StepsCount - 1)
        {
          NextButton.Content = "Next";
          return;
        }

        // the last step before Progress Step           
        if (PageNumber == StepsCount - 1)
        {
          NextButton.Content = WizardPipeline.StartButtonText;
          return;
        }

        // the Progress Step    
        if (PageNumber == StepsCount)
        {
          backButton.Visibility = Visibility.Hidden;

          if (!PipelineManager.Definitions.ContainsKey(WizardPipeline.Name))
          {
            Finish("Done.", true);

            return;
          }

          PipelineManager.StartPipeline(WizardPipeline.Name, _ProcessorArgs, this);
          CancelButton.Content = "Cancel";
          NextButton.IsEnabled = false;
          NextButton.Content = "Retry";
          NextButton.Click -= MoveNextClick;
          NextButton.Click += RetryClick;
        }
      }
    }

    private void MoveBackClick(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Move back click handler", this))
      {
        try
        {
          if (!ProcessStepUserControl(false))
          {
            return;
          }

          PageNumber--;
          InitializeStep();
          NextButton.IsEnabled = true;

          if (PageNumber == 0)
          {
            backButton.IsEnabled = false;
          }

          if (PageNumber < StepsCount - 1)
          {
            NextButton.Content = "Next";
          }



            // the last step before Progress Step
          else if (PageNumber == StepsCount - 1)
          {
            NextButton.Content = WizardPipeline.StartButtonText;
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(
            $"Something went wrong with Wizard logic. It is to be closed. {Environment.NewLine}{Environment.NewLine}{ex.Message}", true, ex);
          Close();
        }
      }
    }

    private void MoveNextClick(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Move next click handler", this))
      {
        try
        {
          if (!ProcessStepUserControl())
          {
            return;
          }

          if (PageNumber == StepsCount - 1)
          {
            try
            {
              _ProcessorArgs = WizardArgs != null ? WizardArgs.ToProcessorArgs() ?? Args : Args;
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError("Failed to process move next click", false, ex);
              return;
            }
          }

          PageNumber++;

          backButton.IsEnabled = true;

          try
          {
            InitializeStep();
          }
          catch
          {
            MoveBackClick(sender, e);
            throw;
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(
            $"Something went wrong with Wizard logic. It is to be closed. {Environment.NewLine}{Environment.NewLine}{ex.Message}", false, ex);
          Close();
        }
      }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
      Close();
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      AbortPipeline();
    }

    private void PauseUnsafe()
    {
      TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
    }

    private bool ProcessStepUserControl(bool next = true)
    {
      using (new ProfileSection("Process StepUserControl", this))
      {
        ProfileSection.Argument("next", next);

        try
        {
          var item = (TabItem)TabControl.SelectedItem;
          var content = item.Content;
          Assert.IsNotNull(content, nameof(content));

          var fullName = content.GetType().FullName;
          var step = content as IWizardStep;
          if (step == null)
          {
            Log.Warn($"The {fullName} control does not implement IWizardStep");

            return ProfileSection.Result(true);
          }

          bool onMove = true;
          bool saveChanges = step.SaveChanges(WizardArgs);

          var flowControl = step as IFlowControl;
          if (flowControl != null)
          {
            onMove = next ? flowControl.OnMovingNext(WizardArgs) : flowControl.OnMovingBack(WizardArgs);
          }

          if (!saveChanges || !onMove)
          {
            return ProfileSection.Result(false);
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(ex.Message, true, ex);

          return ProfileSection.Result(false);
        }

        return ProfileSection.Result(true);
      }
    }

    private string ReplaceVariables(string name)
    {
      using (new ProfileSection("Replace variables", this))
      {
        ProfileSection.Argument("name", name);

        var abstractArgs = WizardArgs as object ?? Args;
        var result = abstractArgs != null ? Pipeline.ReplaceVariables(name, abstractArgs) : name;

        return ProfileSection.Result(result);
      }
    }

    private void ResumeUnsafe()
    {
      TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
    }

    private void RetryClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      using (new ProfileSection("Wizard retry click handler", this))
      {
        try
        {
          Progress = 0;
          Maximum = _Maximum;
          progressBar1.Foreground = ProgressBar1Foreground;
          TaskbarItemInfo.ProgressValue = 0;
          TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
          NextButton.IsEnabled = false;
          Pipeline.Restart();
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError(
            $"Something went wrong with Wizard logic. It is to be closed. {Environment.NewLine}{Environment.NewLine}{ex.Message}", false, ex);
          Close();
        }
      }
    }

    private void SetMaximum(double value)
    {
      if (double.IsNaN(_Maximum))
      {
        _Maximum = value;
      }

      progressBar1.Maximum = value;
      UpdateTaskbar();
    }

    private void SetStatusUnsafe([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      HeaderDetails.Text = name;
    }

    private void UpdateTaskbar()
    {
      TaskbarItemInfo.ProgressValue = ProgressNormalized;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, nameof(sender));
      Assert.ArgumentNotNull(e, nameof(e));

      if (PageNumber != StepsCount && e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        Close();
      }
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        StepInfo[] sis = WizardPipeline._StepInfos;
        foreach (StepInfo stepInfo in sis)
        {
          AddStepToWizard(stepInfo);
        }

        PageNumber = 0;

        InitializeStep();
        var title = ReplaceVariables(WizardPipeline.Title);
        Title = title;
        Header.Text = title;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Error occured while wizard loading", true, ex);
        Close();
      }
    }

    #endregion

    #endregion

    #region Public methods

    public void IncrementProgress(int progress)
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => IncrementProgressUnsafe(progress)));
    }

    public void IncrementProgress(long progress)
    {
      Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => IncrementProgressUnsafe(progress)));
    }

    #endregion

    #region Private methods

    private void WindowClosed(object sender, EventArgs e)
    {
      foreach (CheckBox button in FinishActions.Children.OfType<CheckBox>())
      {
        if (button.IsChecked == true)
        {
          FinishAction action = (FinishAction)button.Tag;
          if (action != null)
          {
            try
            {
              action.Method.Invoke(null, new object[]
              {
                WizardArgs
              });
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError("The {0} finish action has thrown an exception".FormatWith(action.Text), true, ex);
            }
          }
        }
      }
    }

    private void WindowContentRendered(object sender, EventArgs e)
    {
      TabItem selectedItem = (TabItem)TabControl.SelectedItem;
      var control = selectedItem.Content as UserControl;
      if (control != null)
      {
        SetActive(control);
      }
    }

    #endregion

    public void Dispose()
    {
      WizardArgs?.Dispose();
    }
  }
}