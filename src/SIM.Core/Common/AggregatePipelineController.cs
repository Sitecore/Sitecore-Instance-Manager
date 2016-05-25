namespace SIM.Core.Common
{
  using System;
  using System.Collections.Generic;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Pipelines;

  public class AggregatePipelineController : IPipelineController
  {
    [NotNull]
    private readonly List<string> Messages = new List<string>();

    [CanBeNull]
    public string Message { get; private set; }

    public double Maximum { get; set; }

    public Pipeline Pipeline { get; set; }

    [NotNull]
    public IEnumerable<string> GetMessages()
    {
      return this.Messages.ToArray();
    }

    public string Ask([CanBeNull] string title, [CanBeNull] string defaultValue)
    {
      throw new NotImplementedException();
    }

    public bool Confirm([CanBeNull] string message)
    {
      throw new NotImplementedException();
    }

    public void Execute([CanBeNull] string path, string args)
    {
      throw new NotImplementedException();
    }

    public void Finish([CanBeNull] string message, bool closeInterface)
    {
      this.Message = message;
    }

    public void IncrementProgress()
    {
    }

    public void IncrementProgress(long progress)
    {
    }

    public void Pause()
    {
    }

    public void ProcessorCrashed([CanBeNull] string error)
    {
      this.Messages.Add(error);
    }

    public void ProcessorDone([CanBeNull] string title)
    {
    }

    public void ProcessorSkipped(string processorName)
    {
      this.Messages.Add("Skipped: " + processorName);
    }

    public void ProcessorStarted(string title)
    {
    }

    public void Resume()
    {
    }

    public string Select(string message, IEnumerable<string> options, bool allowMultipleSelection = false, string defaultValue = null)
    {
      throw new NotImplementedException();
    }

    public void Start(string replaceVariables, List<Step> steps)
    {
    }

    public void SetProgress(long progress)
    {
    }
  }
}