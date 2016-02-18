namespace SIM.Core.Common
{
  using System;
  using System.Diagnostics;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public abstract class AbstractCommand<TResult> : ICommand
  {
    [NotNull]
    public object Execute()
    {
      var result = new CommandResult<TResult>();
      var timer = new Stopwatch();
      timer.Start();
      try
      {
        this.DoExecute(result);
        timer.Stop();
      }
      catch (Exception ex)
      {
        timer.Stop();
        Log.Error(ex, "{0} command has failed with unhandled exception", this.GetType().Name);
        result.Success = false;
        result.Message = ex.Message;
      }

      result.Elapsed = timer.Elapsed;
      return result;
    }

    protected abstract void DoExecute([NotNull] CommandResultBase<TResult> result);
  }
}