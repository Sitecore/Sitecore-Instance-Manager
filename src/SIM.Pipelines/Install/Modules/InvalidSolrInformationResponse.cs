using System;

namespace SIM.Pipelines.Install.Modules
{
  public class InvalidSolrInformationResponse : ApplicationException
  {
    private readonly string _message;

    public InvalidSolrInformationResponse(string message)
    {
      _message = message;
    }

    public override string Message
    {
      get { return $"Invalid response from /solr/admin/info/system.  {_message}"; }
    }
  }
}