using SIM.Sitecore9Installer;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using System.IO;
using System.Linq;

namespace SIM.Tool.Windows.Pipelines.Solr
{
  public static class AddSolrToSimConfig
  {
    public static void Run(Install9WizardArgs args)
    {
      BaseParameters solrParams = args.Tasker.Tasks.First(t => t.Name == "Solr").LocalParams;
      string solrDomain = solrParams.First(p => p.Name == "SolrDomain").Value;
      string solrInstallRoot = solrParams.First(p => p.Name == "SolrInstallRoot").Value;
      string solrPort = solrParams.First(p => p.Name == "SolrPort").Value;
      string solrVersion = solrParams.First(p => p.Name == "SolrVersion").Value;
      string solrService = $"solr-{solrVersion}-{solrDomain}-{solrPort}";
      string solrAdminUsername = solrParams.FirstOrDefault(p => p.Name == "SolrAdminUserName")?.Value ?? string.Empty;
      string solrAdminPassword = solrParams.FirstOrDefault(p => p.Name == "SolrAdminPassword")?.Value ?? string.Empty;
      SolrDefinition solr = new SolrDefinition()
      {
        Name = solrService,
        Root = Path.Combine(solrInstallRoot, solrService),
        Service = solrService,
        Url = "https://localhost:" + solrPort + "/solr",
        AdminUsername = solrAdminUsername,
        AdminPassword = solrAdminPassword,
      };
      ProfileManager.Profile.Solrs.Add(solr);
      ProfileManager.SaveChanges(ProfileManager.Profile);
    }
  }
}
