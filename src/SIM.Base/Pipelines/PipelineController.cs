namespace SIM.Pipelines
{
  #region

  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public interface IPipelineController
  {
    #region Properties

    double Maximum { get; set; }

    [NotNull]
    Pipeline Pipeline { get; set; }

    #endregion

    #region Public Methods

    [CanBeNull]
    string Ask([NotNull] string title, [NotNull] string defaultValue);

    bool Confirm([NotNull] string message);

    void Execute([NotNull] string path, [CanBeNull] string args);

    void Finish([NotNull] string message, bool closeInterface);

    void IncrementProgress();

    void IncrementProgress(long progress);

    void Pause();

    void ProcessorCrashed([NotNull] string error);

    void ProcessorDone([NotNull] string title);

    void ProcessorSkipped([NotNull] string processorName);

    void ProcessorStarted([NotNull] string title);

    void Resume();

    [CanBeNull]
    string Select([NotNull] string message, [NotNull] IEnumerable<string> options, bool allowMultipleSelection = false, string defaultValue = null);

    void Start([NotNull] string replaceVariables, [NotNull] List<Step> steps);

    #endregion

    #region Public methods

    void SetProgress(long progress);

    #endregion
  }
}