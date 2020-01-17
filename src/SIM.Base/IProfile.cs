namespace SIM
{
  using JetBrains.Annotations;
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;

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

    [CanBeNull]
    List<SolrDefinition> Solrs { get; set; }

    List<VersionToSolr> VersionToSolrMap { get; set; }

    void Save();
  }
}