namespace SIM
{
  using JetBrains.Annotations;

  public interface IProfile
  {
    [CanBeNull]
    string ConnectionString { get; set; }

    [CanBeNull]
    string InstancesFolder { get; set; }

    [CanBeNull]
    string License { get; set; }

    [CanBeNull]
    string LocalRepository { get; set; }

    void Save();
  }
}