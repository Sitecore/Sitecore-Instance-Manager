#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Threading;
using System.Xml;
using SIM.Base;
using SIM.Pipelines;
using SIM.Pipelines.Processors;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Base.Wizards;

#endregion

namespace SIM.Tool.Wizards.Windows
{
  #region

  using System.IO;

  #endregion

  /// <summary>
  ///   Interaction logic for Wizard.xaml
  /// </summary>
  public partial class Wizard : IPipelineController
  {
    #region Fields

    /// <summary>
    ///   The args.
    /// </summary>
    private readonly ProcessorArgs args;

    /// <summary>
    ///   The progress bar 1 foreground.
    /// </summary>
    private readonly Brush progressBar1Foreground;

    /// <summary>
    ///   The wizard params.
    /// </summary>
    private readonly object[] wizardParams;

    /// <summary>
    ///   The wizard pipeline.
    /// </summary>
    private readonly WizardPipeline wizardPipeline;

    /// <summary>
    ///   The processor args.
    /// </summary>
    public WizardArgs ProcessorArgs;

    /// <summary>
    ///   The maximum.
    /// </summary>
    private double maximum = double.NaN;

    ProcessorArgs processorArgs;
    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Wizard"/> class.
    /// </summary>
    /// <param name="wizardPipeline">
    /// The wizard pipeline. 
    /// </param>
    /// <param name="args">
    /// The arguments. 
    /// </param>
    /// <param name="parameters">
    /// The parameters. 
    /// </param>
    /// <exception cref="Exception">
    /// asdasdasdas
    /// </exception>
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
        if (!AppSettings.AppSysIsSingleThreaded.Value)
        {
          this.CancelButton.IsCancel = false;
          this.ResizeMode = ResizeMode.CanMinimize;
        }
      }
    }

    #endregion

    #region Properties

    #region Public properties

    /// <summary>
    ///   Sets Processors.
    /// </summary>
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

    /// <summary>
    ///   Gets or sets the maximum.
    /// </summary>
    /// <value> The maximum. </value>
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

    /// <summary>
    ///   Gets or sets the pipeline.
    /// </summary>
    /// <value> The pipeline. </value>
    public Pipeline Pipeline
    {
      get;
      set;
    }

    #endregion

    #region Private properties

    /// <summary>
    ///   Gets or sets the page number.
    /// </summary>
    /// <value> The page number. </value>
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

    /// <summary>
    ///   Gets or sets the progress.
    /// </summary>
    /// <value> The progress. </value>
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

    /// <summary>
    ///   Gets the steps count.
    /// </summary>
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

    /// <summary>
    ///   Manually closes a <see cref="T:System.Windows.Window" /> .
    /// </summary>
    public new void Close()
    {
      using (new ProfileSection("Close wizard", this))
      {
        this.AbortPipeline();
        if (!AppSettings.AppSysIsSingleThreaded.Value)
        {
          base.Close();
        }
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IPipelineController

    /// <summary>
    /// Asks the specified title.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="defaultValue">
    /// </param>
    /// <returns>
    /// The string. 
    /// </returns>
    [CanBeNull]
    public string Ask([NotNull] string title, [NotNull] string defaultValue)
    {
      Assert.ArgumentNotNull(title, "title");

      return WindowHelper.Ask(title, defaultValue, this);
    }

    /// <summary>
    /// Confirms the specified message.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <returns>
    /// The boolean. 
    /// </returns>
    public bool Confirm([NotNull] string message)
    {
      Assert.ArgumentNotNullOrEmpty(message, "message");

      return WindowHelper.ShowMessage(message, "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
    }

    /// <summary>
    /// Executes the specified path.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="arguments">arguments</param>
    public void Execute([NotNull] string path, string arguments = null)
    {
      Assert.ArgumentNotNull(path, "path");

      if (arguments.EmptyToNull() == null)
      {
        WindowHelper.RunApp(path);
        return;
      }

      WindowHelper.RunApp(path, arguments);
    }

    /// <summary>
    /// Finishes the specified message.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="closeInterface">
    /// if set to <c>true</c> [close interface]. 
    /// </param>
    public void Finish([NotNull] string message, bool closeInterface)
    {
      Assert.ArgumentNotNull(message, "message");

      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.FinishUnsafe(message, closeInterface)));
    }

    /// <summary>
    ///   Increments the progress.
    /// </summary>
    public void IncrementProgress()
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe()));
    }

    /// <summary>
    ///   Pauses this instance.
    /// </summary>
    public void Pause()
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(this.PauseUnsafe));
    }

    /// <summary>
    /// Processors the crashed.
    /// </summary>
    /// <param name="error">
    /// The error. 
    /// </param>
    public void ProcessorCrashed(string error)
    {
      Assert.ArgumentNotNull(error, "error");
    }

    /// <summary>
    /// Processors the done.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    public void ProcessorDone([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, "title");
    }

    /// <summary>
    /// Processors the skipped.
    /// </summary>
    /// <param name="processorName">
    /// Name of the processor. 
    /// </param>
    public void ProcessorSkipped([NotNull] string processorName)
    {
      Assert.ArgumentNotNull(processorName, "processorName");
    }

    /// <summary>
    /// Processors the started.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    public void ProcessorStarted([NotNull] string title)
    {
      Assert.ArgumentNotNull(title, "title");

      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.SetStatusUnsafe(title)));
    }

    /// <summary>
    ///   Resumes this instance.
    /// </summary>
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

    /// <summary>
    /// Starts the specified title.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    public void Start(string title, List<Step> steps)
    {
      Assert.ArgumentNotNull(title, "title");
      Assert.ArgumentNotNull(steps, "steps");

      this.Start(title, steps, 0);
    }

    public void SetProgress(long progress)
    {
      try
      {
        this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.SetProgressUnsafe(progress)));
      }
      catch (Exception ex)
      {
        Log.Error("Error during setting progress", this, ex);
      }
    }

    public void SetProgressUnsafe(long progress)
    {
      this.Progress = progress;
    }

    #endregion

    #endregion

    #region Methods

    #region Protected methods

    /// <summary>
    /// Starts the specified title.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    /// <param name="value">
    /// The value. 
    /// </param>
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

    private double ProgressNormalized
    {
      get
      {
        return this.Progress / this.GetMaximum();
      }
    }

    /// <summary>
    ///   Aborts the pipeline.
    /// </summary>
    private void AbortPipeline()
    {
      Pipeline pipeline = this.Pipeline;
      if (pipeline != null)
      {
        pipeline.Abort();
      }
    }

    /// <summary>
    /// Customs the button click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void CustomButtonClick(object sender, RoutedEventArgs e)
    {
      var customButtonStep = this.TabControl.SelectedContent as ICustomButton;
      if (customButtonStep != null)
      {
        customButtonStep.CustomButtonClick();
      }
    }

    /// <summary>
    /// Errors the click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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

                WindowHelper.HandleError(message, true, null, this);
                break;
              }
          }
        }
      }
    }

    /// <summary>
    /// Finishes the unsafe.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="allDone">
    /// if set to <c>true</c> [all done]. 
    /// </param>
    private void FinishUnsafe([NotNull] string message, bool allDone)
    {
      Assert.ArgumentNotNull(message, "message");

      using (new ProfileSection("Finish wizard (unsafe)", this))
      {
        ProfileSection.Argument("message", message);
        ProfileSection.Argument("allDone", allDone);

        if (allDone)
        {
          AddFinishActions(this.wizardPipeline.FinishActions);

          if (this.wizardPipeline.FinishActionHives != null)
          {
            foreach (FinishActionHive hive in this.wizardPipeline.FinishActionHives.NotNull())
            {
              AddFinishActions(hive.GetFinishActions(this.ProcessorArgs));
            }
          }

          foreach (var plugin in PluginManager.GetEnabledPlugins())
          {
            var actionElements = plugin.PluginXmlDocument.SelectNodes("/plugin/wizards/" + this.wizardPipeline.Name + "/finish/action");
            foreach (var actionElement in actionElements.OfType<XmlElement>())
            {
              var finishAction = WizardPipelineManager.ParseFinishAction(this.ProcessorArgs.GetType(), actionElement);
              AddFinishAction(finishAction);
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

    private void AddFinishAction(FinishAction action)
    {
      this.FinishActions.Children.Add(new CheckBox
      {
        Content = action.Text, 
        Tag = action
      });
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    /// <param name="output">
    /// The output. 
    /// </param>
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

    /// <summary>
    ///   Gets the maximum.
    /// </summary>
    /// <returns> The maximum. </returns>
    private double GetMaximum()
    {
      return this.progressBar1.Maximum;
    }

    /// <summary>
    ///   Increments the progress unsafe.
    /// </summary>
    private void IncrementProgressUnsafe(long value = 1)
    {
      this.Progress += value;
      this.UpdateTaskbar();
    }

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

    /// <summary>
    /// Initializes the step.
    /// </summary>
    /// <param name="i">
    /// The i. 
    /// </param>
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

        InitializeIWizardStep(n);

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
          PipelineManager.StartPipeline(this.wizardPipeline.Name, processorArgs, this);
          this.backButton.Visibility = Visibility.Hidden;
          this.CancelButton.Content = "Cancel";
          this.NextButton.IsEnabled = false;
          this.NextButton.Content = "Retry";
          this.NextButton.Click -= this.MoveNextClick;
          this.NextButton.Click += this.RetryClick;
        }
      }
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
          Log.Warn("The {0} type is not UIElement-based".FormatWith(fullName), this);
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

    /// <summary>
    /// Moves the back click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, true, ex, this);
          this.Close();
        }
      }
    }

    /// <summary>
    /// Moves the next click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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
              WindowHelper.HandleError(ex.Message, false, ex, this);
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
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, false, ex, this);
          this.Close();
        }
      }
    }

    /// <summary>
    /// Called when [cancel].
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
    private void OnCancel(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    /// <summary>
    /// Called when [closing].
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data. 
    /// </param>
    private void OnClosing(object sender, CancelEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      this.AbortPipeline();
    }

    /// <summary>
    ///   Pauses the unsafe.
    /// </summary>
    private void PauseUnsafe()
    {
      this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Paused;
    }

    /// <summary>
    /// Processes the step user control.
    /// </summary>
    /// <param name="next">
    /// if set to <c>true</c> [next]. 
    /// </param>
    /// <returns>
    /// The step user control. 
    /// </returns>
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
            Log.Warn("The {0} control does not implement IWizardStep".FormatWith(fullName), this);

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
          WindowHelper.HandleError(ex.Message, true, ex, this);

          return ProfileSection.Result(false);
        }

        return ProfileSection.Result(true);
      }
    }

    /// <summary>
    /// Replaces the variables.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    /// <returns>
    /// The variables. 
    /// </returns>
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

    /// <summary>
    ///   Resumes the unsafe.
    /// </summary>
    private void ResumeUnsafe()
    {
      this.TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
    }

    /// <summary>
    /// Retries the click.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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
          WindowHelper.HandleError("Something went wrong with Wizard logic. It is to be closed. " + Environment.NewLine + Environment.NewLine + ex.Message, false, ex, this);
          this.Close();
        }
      }
    }

    /// <summary>
    /// Sets the maximum.
    /// </summary>
    /// <param name="value">
    /// The value. 
    /// </param>
    private void SetMaximum(double value)
    {
      if (double.IsNaN(this.maximum))
      {
        this.maximum = value;
      }

      this.progressBar1.Maximum = value;
      this.UpdateTaskbar();
    }

    /// <summary>
    /// Sets the status unsafe.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    private void SetStatusUnsafe([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      this.HeaderDetails.Text = name;
    }

    /// <summary>
    ///   Updates the taskbar.
    /// </summary>
    private void UpdateTaskbar()
    {
      this.TaskbarItemInfo.ProgressValue = ProgressNormalized;
    }

    /// <summary>
    /// Windows the key up.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data. 
    /// </param>
    private void WindowKeyUp([NotNull] object sender, [NotNull] KeyEventArgs e)
    {
      Assert.ArgumentNotNull(sender, "sender");
      Assert.ArgumentNotNull(e, "e");

      if (this.PageNumber != this.StepsCount && e.Key == Key.Escape)
      {
        if (e.Handled)
          return;
        e.Handled = true;
        this.Close();
      }
    }

    /// <summary>
    /// Windows the loaded.
    /// </summary>
    /// <param name="sender">
    /// The sender. 
    /// </param>
    /// <param name="e">
    /// The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data. 
    /// </param>
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
        WindowHelper.HandleError("Error occured while wizard loading", true, ex, this);
        this.Close();
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
        Log.Debug("Control {0} does not implement IWizardStep".FormatWith(fullName));
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

    #endregion

    #endregion

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
              action.Method.Invoke(null, new object[] { this.ProcessorArgs });
            }
            catch (Exception ex)
            {
              WindowHelper.HandleError("The {0} finish action has thrown an exception".FormatWith(action.Text), true, ex, this);
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


    public void IncrementProgress(int progress)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe(progress)));
    }

    public void IncrementProgress(long progress)
    {
      this.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => this.IncrementProgressUnsafe(progress)));
    }
  }
}