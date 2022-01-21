using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Loggers;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.MultipleDeletion
{
  public class MultipleDeletion9Args : ProcessorArgs
  {
    public SqlConnectionStringBuilder _ConnectionString;

    public bool _ScriptsOnly;

    private readonly List<string> _Environments;

    public MultipleDeletion9Args(List<string> environments, ILogger logger)
    {
      this._Environments = environments;
      this.Logger = logger;
    }

    public List<string> Environments
    {
      get
      {
        return _Environments;
      }
    }

    public ILogger Logger
    {
      get; set;
    }
  }
}