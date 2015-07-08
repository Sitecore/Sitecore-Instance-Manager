using System.Collections.Generic;
using System.Data.SqlClient;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.MultipleDeletion
{
  public class MultipleDeletionArgs : ProcessorArgs
  {
    public MultipleDeletionArgs(List<string> instances)
    {
      _instances = instances;
    }

    public SqlConnectionStringBuilder ConnectionString;

    private readonly List<string> _instances;

    public List<string> Instances
    {
      get
      {
        return _instances;
      }
    } 
  }
}
