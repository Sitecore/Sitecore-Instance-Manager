using System;
using System.Runtime.Serialization;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  [Serializable]
  internal class SqlConnectionException : Exception  {  

    public SqlConnectionException()
    {
    }    

    public SqlConnectionException(string message) : base(message)
    {
    }

    public SqlConnectionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected SqlConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
  }
}