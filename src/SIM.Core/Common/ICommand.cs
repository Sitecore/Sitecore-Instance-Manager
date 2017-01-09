namespace SIM.Core.Common
{
  using JetBrains.Annotations;

  public interface ICommand
  {
    [CanBeNull]
    CommandResult Execute();
  }
}