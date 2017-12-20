using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using SIM.Extensions;

namespace SIM.Pipelines.Install.Modules
{
  /// <summary>
  /// Details of Solr installation necessary for Solr feature support.
  /// </summary>
  public class SolrInformation
  {
    private XmlDocumentEx _solrInfoResponse { get; }

    #region Properties

    /// <summary>
    /// Guaranteed to be 4 or higher. An exception is thrown otherwise.
    /// </summary>
    public int MajorVersion { get; }

    /// <summary>
    /// Pulled from solr_home property or solr.solr.home command argument.
    /// </summary>
    public string SolrBasePath { get; }

    /// <summary>
    /// Used for error handling.
    /// </summary>
    public XmlDocumentEx SolrInfoResponse { get; }

    public string CollectionTemplate
    {
      get { return MajorVersion == 4 ? "collection1" : @"configsets\data_driven_schema_configs"; }
    }

    public string TemplateFullPath
    {
      get { return SolrBasePath.EnsureEnd(@"\") + CollectionTemplate;  }
    }

    #endregion

    #region Public Methods

    public SolrInformation(XmlDocumentEx solrInfoResponse)
    { 

      if (solrInfoResponse == null)
      {
        throw new ArgumentNullException(nameof(solrInfoResponse),
          $"The XML node passed to the constructor of the '{nameof(SolrInformation)}' class is null, which may be caused by incorrectly formatted XML.");
      }
      SolrInfoResponse = solrInfoResponse;

      XmlNode node = solrInfoResponse.SelectSingleNode("/response/lst[@name='lucene']/str[@name='solr-spec-version']");

      if (node == null || node.InnerText.IsNullOrEmpty())
      {
        throw new InvalidException($"The version of Solr could not be determined.", this);
      }

      MajorVersion = GetMajorVersionFrom(node.InnerText);

      if (MajorVersion<4) { throw new InvalidException($"Solr 4.0.0 or greater required.  Using Solr {MajorVersion}.", this);}


      SolrBasePath = solrInfoResponse.SelectSingleNode("/response/str[@name='solr_home']")?.InnerText;

      if (SolrBasePath.IsNullOrEmpty())
      {
        SolrBasePath = GetPathFromCommandArgs(solrInfoResponse);
      }

      if (SolrBasePath.IsNullOrEmpty())
      {
        throw new InvalidException(@"Solr base path could not be determined. For Solr 4, this is read from the ""solr.solr.home"" argument that should be visible on the Solr home screen. For Solr 5 and higher, this should also be visible in the ""solr_home"" setting.", this);
      }
    }
    #endregion

    #region Private Methods

    private string GetPathFromCommandArgs(XmlDocumentEx solrInfoResponse)
    {
      string commandArgsPath = @"/response/lst[@name=""jvm""]/lst[@name=""jmx""]/arr[@name=""commandLineArgs""]/str";

      IEnumerable<string> args = 
        solrInfoResponse.SelectElements(commandArgsPath).Select(e => e.InnerText);

      Regex solrHomeRegex = new Regex(@"^-Dsolr\.solr\.home=(.*)$");

      string matchingArg = args.FirstOrDefault(setting => solrHomeRegex.IsMatch(setting));

      if (matchingArg == null)
      {
        return string.Empty;
      }

      Match match = solrHomeRegex.Match(matchingArg);

      return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private int GetMajorVersionFrom(string versionText)
    {
      var r = new Regex(@"^(\d+)\.\d+.\d+");
      GroupCollection groups = r.Match(versionText).Groups;
      int value;
      if (groups.Count > 1 && int.TryParse(groups[1].Value, out value))
      {
        return value;
      }
      throw new InvalidException($"Solr version could not be determined. The call to /solr/admin/system/info returned a 'solr-spec-version' in unexpected form: '{versionText}'", this);
    }

    #endregion

    #region Nested Classes

    public class InvalidException : ApplicationException
    {
      private string _message { get; }
      private SolrInformation SolrInformation { get; }


      public InvalidException(string message, SolrInformation solrInformation)
      {
        _message = message;
        SolrInformation = solrInformation;
      }

      public override string Message => $"Invalid response from /solr/admin/info/system.\n\nDetails: {_message}\n\nSolr API Response:\n{SolrInformation.SolrInfoResponse.OuterXml}";

    }

    #endregion

  }
}