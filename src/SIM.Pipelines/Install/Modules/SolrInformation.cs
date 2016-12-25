using System.Text.RegularExpressions;
using System.Xml;
using SIM.Extensions;

namespace SIM.Pipelines.Install.Modules
{
  public class SolrInformation
  {
    #region Properties

    /// <summary>
    /// Guaranteed to be 4 or higher. An exception is thrown otherwise.
    /// </summary>
    public int MajorVersion { get; }

    /// <summary>
    /// Guaranteed to be present if Solr version is 5.0.0 or higher.
    /// </summary>
    public string SolrBasePath { get; }

    #endregion

    #region Public Methods

    public SolrInformation(XmlDocumentEx solrInfoResponse)
    {
      XmlNode node = solrInfoResponse.SelectSingleNode("/response/lst[@name='lucene']/str[@name='solr-spec-version']");

      if (node == null || node.InnerText.IsNullOrEmpty())
      {
        throw new InvalidSolrInformationResponse($"Solr version could not be determined.");
      }

      MajorVersion = GetMajorVersionFrom(node.InnerText);

      if (MajorVersion<4) { throw new InvalidSolrInformationResponse("Solr 4.0.0 or greater required.");}


      SolrBasePath = solrInfoResponse.SelectSingleNode("/response/str[@name='solr_home']")?.InnerText ?? "";

      if (MajorVersion >= 5 && SolrBasePath.IsNullOrEmpty())
      {
        throw new InvalidSolrInformationResponse("'solr_home' element not found or empty.");
      }
   
    }

    #endregion

    #region Private Methods

    private int GetMajorVersionFrom(string versionText)
    {
      var r = new Regex(@"^(\d+)\.\d+.\d+");
      GroupCollection groups = r.Match(versionText).Groups;
      int value;
      if (groups.Count > 1 && int.TryParse(groups[1].Value, out value))
      {
        return value;
      }
      throw new InvalidSolrInformationResponse($"Solr version could not be determined. The call to /solr/admin/system/info returned a'solr-spec-version' in unexpected form: {versionText}");
    }

    #endregion

  }
}