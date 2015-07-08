#region Usings

using System.Collections.Generic;
using SIM.Base;

#endregion

namespace SIM.Pipelines
{
  #region

  

  #endregion

  /// <summary>
  ///   The i pipeline controller.
  /// </summary>
  public interface IPipelineController
  {
    #region Properties

    /// <summary>
    ///   Gets or sets Maximum.
    /// </summary>
    double Maximum { get; set; }

    /// <summary>
    ///   Gets or sets Pipeline.
    /// </summary>
    [NotNull]
    Pipeline Pipeline { get; set; }

    #endregion

    #region Public Methods

    /// <summary>
    /// The ask.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="defaultValue">
    /// The default value. 
    /// </param>
    /// <returns>
    /// The ask. 
    /// </returns>
    [CanBeNull]
    string Ask([NotNull] string title, [NotNull] string defaultValue);

    /// <summary>
    /// The confirm.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <returns>
    /// The confirm. 
    /// </returns>
    bool Confirm([NotNull] string message);

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="path">
    /// The path. 
    /// </param>
    /// <param name="args">The args.</param>
    void Execute([NotNull] string path, [CanBeNull] string args);

    /// <summary>
    /// The finish.
    /// </summary>
    /// <param name="message">
    /// The message. 
    /// </param>
    /// <param name="closeInterface">
    /// The close interface. 
    /// </param>
    void Finish([NotNull] string message, bool closeInterface);

    /// <summary>
    ///   The increment progress.
    /// </summary>
    void IncrementProgress();

    /// <summary>
    ///   The increment progress.
    /// </summary>
    void IncrementProgress(long progress);
    /// <summary>
    ///   The pause.
    /// </summary>
    void Pause();

    /// <summary>
    /// The processor crashed.
    /// </summary>
    /// <param name="error">
    /// The error. 
    /// </param>
    void ProcessorCrashed([NotNull] string error);

    /// <summary>
    /// The processor done.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    void ProcessorDone([NotNull] string title);

    /// <summary>
    /// The processor skipped.
    /// </summary>
    /// <param name="processorName">
    /// The processor name. 
    /// </param>
    void ProcessorSkipped([NotNull] string processorName);

    /// <summary>
    /// The processor started.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    void ProcessorStarted([NotNull] string title);

    /// <summary>
    ///   The resume.
    /// </summary>
    void Resume();

    [CanBeNull]
    string Select([NotNull] string message, [NotNull] IEnumerable<string> options, bool allowMultipleSelection = false, string defaultValue = null);

    /// <summary>
    /// The start.
    /// </summary>
    /// <param name="replaceVariables">
    /// The replace variables. 
    /// </param>
    /// <param name="steps">
    /// The steps. 
    /// </param>
    void Start([NotNull] string replaceVariables, [NotNull] List<Step> steps);

    #endregion

    void SetProgress(long progress);
  }
}