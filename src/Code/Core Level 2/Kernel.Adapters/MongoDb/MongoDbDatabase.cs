using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Adapters.MongoDb
{
  using SIM.Base;

  public class MongoDbDatabase
  {
    [NotNull]
    private readonly string name;

    [NotNull]
    private readonly string connectionString;

    public MongoDbDatabase(string name, string connectionString)
    {
      this.name = name;
      this.connectionString = connectionString;
    }

    [NotNull]
    public string Name
    {
      get
      {
        return this.name;
      }
    }

    [NotNull]
    public string LogicalName
    {
      get
      {
        return this.connectionString.Substring(connectionString.LastIndexOf('/')+1);
      }
    }

    [NotNull]
    public string ConnectionString
    {
      get
      {
        return this.connectionString;
      }
    }
  }
}
