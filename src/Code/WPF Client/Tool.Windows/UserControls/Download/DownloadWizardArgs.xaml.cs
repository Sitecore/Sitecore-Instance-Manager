using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using SIM.Tool.Windows.Pipelines.Download;

namespace SIM.Tool.Windows.UserControls.Download
{
  public class DownloadWizardArgs : WizardArgs
  {
    public DownloadWizardArgs()
    { 
    }

    public DownloadWizardArgs(string username, string password)
    {
      this.UserName = username;
      this.Password = password;
    }

    private readonly List<ProductDownloadInCheckbox> products = new List<ProductDownloadInCheckbox>();

    public List<ProductDownloadInCheckbox> Products
    {
      get
      {
        return this.products;
      }
    }

    public override SIM.Pipelines.Processors.ProcessorArgs ToProcessorArgs()
    {
      return new DownloadArgs(this.Cookies, this.Links, ProfileManager.Profile.LocalRepository, this.Sizes);
    }

    public ReadOnlyCollection<Uri> Links { get; set; }

    public UriBasedCollection<long> Sizes { get; set; }
    
    public string Password { get; set; }

    public string UserName { get; set; }

    public string Cookies { get; set; }

    public string[] Records { get; set; }
  }
}