namespace SIM.Pipelines.MultipleDeletion
{
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using SIM.Pipelines.Processors;

  public class MultipleDeletionArgs : ProcessorArgs
  {
    #region Fields

    public SqlConnectionStringBuilder ConnectionString;

    private readonly List<string> _instances;

    #endregion

    #region Constructors

    public MultipleDeletionArgs(List<string> instances)
    {
      this._instances = instances;
    }

    #endregion

    #region Public properties

    public List<string> Instances
    {
      get
      {
        return this._instances;
      }
    }

    #endregion
  }
}