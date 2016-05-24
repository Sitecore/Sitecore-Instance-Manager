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
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Core;
  using SIM.Pipelines;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;

  #endregion

  public partial class Wizard : IPipelineController
  {
    #region Fields

    public WizardArgs ProcessorArgs;
    private readonly ProcessorArgs args;

    private readonly Brush progressBar1Foreground;

    private readonly object[] wizardParams;

    private readonly WizardPipeline wizardPipeline;

    private double maximum = double.NaN;

    private ProcessorArgs processorArgs;

    #endregion

    #region Constructors

    public Wizard(WizardPipeline wizardPipeline, ProcessorArgs args, object[] parameters = null)
    {
      using (new ProfileSection("Create wizard instance", this))
      {
        ProfileSection.Argument("wizardPipeline", wizardPipeline);
        ProfileSection.Argument("args", args);
        ProfileSection.Argument("parameters", parameters);

        this.wizardPipeline = wizardPipeline;
        this.args = args;
        this.InitializeComponent();
        this.progressBar1Foreground = this.progressBar1.Foreground;
        this.wizardParams = parameters;
        if (!WinAppSettings.AppSysIsSingleThreaded.Value)
        {
          this.CancelButton.IsCancel = false;
          this.ResizeMode = ResizeMode.CanMinimize;
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
        return (double)this.Dispatcher.Invoke(DispatcherPriority.Normal, new Func<double>(this.GetMaximum));
      }

      set
      {
        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.SetMaximum(value)));
      }
    }

    public Pipeline Pipeline { get; set; }

    [NotNull]
    public List<Processor> Processors
    {
      set
      {
        Assert.ArgumentNotNull(value, "value");

        List<Processor> list = new List<Processor>();
        this.GetList(value, list);
        this.DataGrid.DataContext = list;
      }
    }

    #endregion

    #region Private properties

    private int PageNumber
    {
      get
      {
        return this.TabControl.SelectedIndex;
      }

      set
      {
        this.TabControl.SelectedIndex = value;
      }
    }

    private double Progress
    {
      get
      {
        return this.progressBar1.Value;
      }

      set
      {
        this.progressBar1.Value = value;
      }
    }

    private int StepsCount
    {
      get
      {
        return this.wizardPipeline.StepInfos.Length;
      }
    }

    #endregion

    #endregion

    #region Public Methods

    public new void Close()
    {
      using (new ProfileSection("Close wizard", this))
      {
        this.AbortPipeline();
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
      Assert.ArgumentNotNull(title, "title");

      return WindowHelper.Ask(title, defaultValue, this);
    }

    public bool Confirm([NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");

      return WindowHelper.ShowMessage(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    public void Execute([NotNull] string path, string arguments = null)
    {
      Assert.ArgumentNotNull(path, "path");

      if (arguments.EmptyToNull() == null)
      {
        CoreApp.RunApp(path);
        return;
      }

      CoreApp.RunApp(path, arguments);
    }

    public void Finish([NotNull] string message, bool closeInterface)
    {
      Assert.ArgumentNotNull(message, "message");

      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.FinishUnsafe(message, closeInterface)));
    }

    public void IncrementProgress()
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe()));
    }

    public void Pause()
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.PauseUnsafe));
    }

    public void ProcessorCrashed(string error)
    {
      Assert.ArgumentNotNull(error, "error");
    }

    public void ProcessorDone([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, "title");
    }

    public void ProcessorSkipped([NotNull] string processorName)
    {
      Assert.ArgumentNotNull(processorName, "processorName");
    }

    public void ProcessorStarted([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, "title");

      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.SetStatusUnsafe(title)));
    }

    public void Resume()
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.ResumeUnsafe));
    }

    [CanBeNull]
    public string Select([NotNull] string message, [NotNull] IEnumerable<string> options, bool allowMultipleSelection = false, string defaultValue = null)
    {
      Assert.ArgumentNotNull(message, "message");
      Assert.ArgumentNotNull(options, "options");

      return (string)this.Dispatcher.Invoke(new Func<string>(() => WindowHelper.AskForSelection("Select an option", "Select an option", message, options, this, defaultValue, allowMultipleSelection)));
    }

    public void SetProgress(long progress)
    {
      try
      {
        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.SetProgressUnsafe(progress)));
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during setting progress");
      }
    }

    public void SetProgressUnsafe(long progress)
    {
      this.Progress = progress;
    }

    public void Start(string title, List<Step> steps)
    {
      Assert.ArgumentNotNull(title, "title");
      Assert.ArgumentNotNull(steps, "steps");

      this.Start(title, steps, 0);
    }

    #endregion

    #endregion

    #region Methods

    #region Private properties

    private double ProgressNormalized
    {
      get
      {
        return this.Progress / this.GetMaximum();
      }
    }

    #endregion

    #region Protected methods

    protected void Start([NotNull] string title, [NotNull] List<Step> steps, int value)
    {
      Assert.ArgumentNotNull(title, "title");
      Assert.ArgumentNotNull(steps, "steps");

      this.HeaderDetails.Text = title;
      this.Maximum = value;
      List<Processor> p = new List<Processor>();
      foreach (Step step in steps)
      {
        List<Processor> pp = step.Processors;
        p.AddRange(pp);
      }

      this.Processors = p;
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
      Pipeline pipeline = this.Pipeline;
      if (pipeline != null)
      {
        pipeline.Abort();
      }
    }

    private void AddFinishAction(FinishAction action)
    {
      this.FinishActions.Children.Add(new CheckBox
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
          this.AddFinishAction(action);
        }
      }
    }

    private void AddStepToWizard(StepInfo stepInfo, WizardArgs wArgs)
    {
      Type ctrl = stepInfo.Control;
      Assert.IsNotNull(ctrl, "The {0} step contains null as a control".FormatWith(stepInfo.Title));
      var fullName = ctrl.FullName;
      Assert.IsTrue(ctrl.IsClass, "Control {0} is not class".FormatWith(fullName));

      if (!ctrl.GetInterfaces().Contains(typeof(IWizardStep)))
      {
        Log.Debug("Control {0} does not implement IWizardStep", fullName);
      }

      var param = stepInfo.Param;
      var wizardStep = (UserControl)(!string.IsNullOrEmpty(param) ? ReflectionUtil.CreateObject(ctrl, param) : ReflectionUtil.CreateObject(ctrl));

      this.ProcessorArgs = wArgs;

      this.TabControl.Items.Insert(this.TabControl.Items.Count - 2, new TabItem
      {
        AllowDrop = false, 
        Content = wizardStep, 
        Visibility = Visibility.Collapsed
      });
    }

    private void CustomButtonClick(object sender, RoutedEventArgs e)
    {
      var customButtonStep = this.TabControl.SelectedContent as ICustomButton;
      if (customButtonStep != null)
      {
        customButtonStep.CustomButtonClick();
      }
    }

    private void ErrorClick([CanBeNull] object sender, [NotNull] RoutedEventArgs e)
    {
      Assert.ArgumentNotNull(e, "e");

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
              string messageLabel = string.Format(@"{0} action failed with message: ", processor.Title);
              const string skipped = "Action was skipped because other one had problem - please find it, fix the problem and run the process again";
              string message;
              Exception exception = processor.Error;
              if (exception != null)
              {
                string exceptionMessage = exception.Message;
                if (exceptionMessage.Contains("cannot be upgraded because it is read-only") || exceptionMessage.Contains(": 15105"))
                {
                  message = "It seems that the NETWORK SERVICE identity doesn't have full access rights to the folder you selected to install the instance to." + Environment.NewLine + Environment.NewLine + exceptionMessage;
                }
                else
                {
                  message = messageLabel + exceptionMessage;
                }
              }
              else
              {
                message = skipped;
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
      Assert.ArgumentNotNull(message, "message");

      using (new ProfileSection("Finish wizard (unsafe)", this))
      {
        ProfileSection.Argument("message", message);
        ProfileSection.Argument("allDone", allDone);

        if (allDone)
        {
          this.AddFinishActions(this.wizardPipeline.FinishActions);

          if (this.wizardPipeline.FinishActionHives != null)
          {
            foreach (FinishActionHive hive in this.wizardPipeline.FinishActionHives.NotNull())
            {
              this.AddFinishActions(hive.GetFinishActions(this.ProcessorArgs));
            }
          }

          this.FinishTextBlock.Text = this.wizardPipeline.FinishText;
          this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.None;
          this.Progress = 0;
          this.NextButton.Visibility = Visibility.Hidden;
          this.CancelButton.Content = "Finish";
          this.CancelButton.Focus();
          this.PageNumber++;
          this.HeaderDetails.Text = "Completed";
        }
        else
        {
          this.SetStatusUnsafe("Some steps require your attention");
          this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Error;
          this.progressBar1.Foreground = Brushes.Red;
          this.NextButton.IsEnabled = true;
        }
      }
    }

    private void GetList([NotNull] IEnumerable<Processor> value, [NotNull] List<Processor> output)
    {
      Assert.ArgumentNotNull(value, "value");
      Assert.ArgumentNotNull(output, "output");

      foreach (Processor q in value)
      {
        output.Add(q);
        this.GetList(q.NestedProcessors, output);
      }
    }

    private double GetMaximum()
    {
      return this.progressBar1.Maximum;
    }

    private WizardArgs GetWizardArgs(Type argsType)
    {
      WizardArgs wArgs = null;
      if (argsType != null)
      {
        wArgs = (WizardArgs)ReflectionUtil.CreateObject(argsType, this.wizardParams ?? new object[0]);
        wArgs.WizardWindow = this;
      }

      return wArgs;
    }

    private void IncrementProgressUnsafe(long value = 1)
    {
      this.Progress += value;
      this.UpdateTaskbar();
    }

    private void InitializeIWizardStep(int n)
    {
      using (new ProfileSection("Initializing step", this))
      {
        ProfileSection.Argument("n", n);

        this.CustomButton.Visibility = Visibility.Hidden;
        TabItem item = this.TabControl.Items[n] as TabItem;
        Assert.IsNotNull(item, "item");

        var content = item.Content;
        Assert.IsNotNull(content, "content");

        var fullName = content.GetType().FullName;

        var control = content as UIElement;
        if (control == null)
        {
          Log.Warn("The {0} type is not UIElement-based", fullName);
          return;
        }

        var customButtonStep = control as ICustomButton;
        bool isVisible = customButtonStep != null;
        if (isVisible)
        {
          string name = customButtonStep.CustomButtonText;
          this.CustomButton.Content = name ?? string.Empty;
          isVisible = !string.IsNullOrEmpty(name);
        }

        this.CustomButton.Visibility = isVisible ? Visibility.Visible : Visibility.Hidden;

        var step = control as IWizardStep;

        // Progress or Final step
        if (step == null)
        {
          return;
        }

        var wizardArgs = this.ProcessorArgs;
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

        int n = i ?? this.PageNumber;

        using (new ProfileSection("Set header", this))
        {
          StepInfo[] stepInfos = this.wizardPipeline.StepInfos;
          if (stepInfos.Length > n)
          {
            string title = stepInfos[n].Title;
            this.HeaderDetails.Text = this.ReplaceVariables(title);
          }
        }

        this.InitializeIWizardStep(n);

        // regular step
        if (this.PageNumber < this.StepsCount - 1)
        {
          this.NextButton.Content = "Next";
          return;
        }

        // the last step before Progress Step           
        if (this.PageNumber == this.StepsCount - 1)
        {
          this.NextButton.Content = this.wizardPipeline.StartButtonText;
          return;
        }

        // the Progress Step    
        if (this.PageNumber == this.StepsCount)
        {
          PipelineManager.StartPipeline(this.wizardPipeline.Name, this.processorArgs, this);
          this.backButton.Visibility = Visibility.Hidden;
          this.CancelButton.Content = "Cancel";
          this.NextButton.IsEnabled = false;
          this.NextButton.Content = "Retry";
          this.NextButton.Click -= this.MoveNextClick;
          this.NextButton.Click += this.RetryClick;
        }
      }
    }

    private void MoveBackClick(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Move back click handler", this))
      {
        try
        {
          if (!this.ProcessStepUserControl(false))
          {
            return;
          }

          this.PageNumber--;
          this.InitializeStep();
          this.NextButton.IsEnabled = true;

          if (this.PageNumber == 0)
          {
            this.backButton.IsEnabled = false;
          }

          if (this.PageNumber < this.StepsCount - 1)
          {
            this.NextButton.Content = "Next";
          }



            // the last step before Progress Step
          else if (this.PageNumber == this.StepsCount - 1)
          {
            this.NextButton.Content = this.wizardPipeline.StartButtonText;
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, true, ex);
          this.Close();
        }
      }
    }

    private void MoveNextClick(object sender, RoutedEventArgs e)
    {
      using (new ProfileSection("Move next click handler", this))
      {
        try
        {
          if (!this.ProcessStepUserControl())
          {
            return;
          }

          if (this.PageNumber == this.StepsCount - 1)
          {
            try
            {
              this.processorArgs = this.ProcessorArgs != null ? this.ProcessorArgs.ToProcessorArgs() ?? this.args : this.args;
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError(ex.Message, false, ex);
              return;
            }
          }

          this.PageNumber++;

          this.backButton.IsEnabled = true;

          try
          {
            this.InitializeStep();
          }
          catch
          {
            this.MoveBackClick(sender, e);
            throw;
          }
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, false, ex);
          this.Close();
        }
      }
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void OnClosing(object sender, CancelEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      this.AbortPipeline();
    }

    private void PauseUnsafe()
    {
      this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
    }

    private bool ProcessStepUserControl(bool next = true)
    {
      using (new ProfileSection("Process StepUserControl", this))
      {
        ProfileSection.Argument("next", next);

        try
        {
          var item = (TabItem)this.TabControl.SelectedItem;
          var content = item.Content;
          Assert.IsNotNull(content, "content");

          var fullName = content.GetType().FullName;
          var step = content as IWizardStep;
          if (step == null)
          {
            Log.Warn("The {0} control does not implement IWizardStep", fullName);

            return ProfileSection.Result(true);
          }

          bool onMove = true;
          bool saveChanges = step.SaveChanges(this.ProcessorArgs);

          var flowControl = step as IFlowControl;
          if (flowControl != null)
          {
            onMove = next ? flowControl.OnMovingNext(this.ProcessorArgs) : flowControl.OnMovingBack(this.ProcessorArgs);
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

        AbstractArgs abstractArgs = this.ProcessorArgs as AbstractArgs ?? this.args;
        var result = abstractArgs != null ? Pipeline.ReplaceVariables(name, abstractArgs) : name;

        return ProfileSection.Result(result);
      }
    }

    private void ResumeUnsafe()
    {
      this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
    }

    private void RetryClick([CanBeNull] object sender, [CanBeNull] RoutedEventArgs e)
    {
      using (new ProfileSection("Wizard retry click handler", this))
      {
        try
        {
          this.Progress = 0;
          this.Maximum = this.maximum;
          this.progressBar1.Foreground = this.progressBar1Foreground;
          this.TaskbarItemInfo.ProgressValue = 0;
          this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
          this.NextButton.IsEnabled = false;
          this.Pipeline.Restart();
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, false, ex);
          this.Close();
        }
      }
    }

    private void SetMaximum(double value)
    {
      if (double.IsNaN(this.maximum))
      {
        this.maximum = value;
      }

      this.progressBar1.Maximum = value;
      this.UpdateTaskbar();
    }

    private void SetStatusUnsafe([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      this.HeaderDetails.Text = name;
    }

    private void UpdateTaskbar()
    {
      this.TaskbarItemInfo.ProgressValue = this.ProgressNormalized;
    }

    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (this.PageNumber != this.StepsCount && e.Key == Key.Escape)
      {
        if (e.Handled)
        {
          return;
        }

        e.Handled = true;
        this.Close();
      }
    }

    private void WindowLoaded(object sender, RoutedEventArgs e)
    {
      try
      {
        Type argsType = this.wizardPipeline.Args;
        var wArgs = this.GetWizardArgs(argsType);

        StepInfo[] sis = this.wizardPipeline.StepInfos;
        foreach (StepInfo stepInfo in sis)
        {
          this.AddStepToWizard(stepInfo, wArgs);
        }

        this.PageNumber = 0;

        this.InitializeStep();
        string title = this.ReplaceVariables(this.wizardPipeline.Title);
        this.Title = title;
        this.Header.Text = title;
      }
      catch (Exception ex)
      {
        WindowHelper.HandleError("Error occured while wizard loading", true, ex);
        this.Close();
      }
    }

    #endregion

    #endregion

    #region Public methods

    public void IncrementProgress(int progress)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe(progress)));
    }

    public void IncrementProgress(long progress)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe(progress)));
    }

    #endregion

    #region Private methods

    private void WindowClosed(object sender, EventArgs e)
    {
      foreach (CheckBox button in this.FinishActions.Children.OfType<CheckBox>())
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
                this.ProcessorArgs
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
      TabItem selectedItem = (TabItem)this.TabControl.SelectedItem;
      var control = selectedItem.Content as UserControl;
      if (control != null)
      {
        SetActive(control);
      }
    }

    #endregion
  }
}