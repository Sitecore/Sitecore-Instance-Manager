namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using SIM.Base;
  using SIM.Pipelines.Processors;
  using SIM.Tool.Base.Profiles;
  using SIM.Tool.Base.Wizards;
  using SIM.Tool.Windows.Pipelines.Download8;

  public class DownloadWizardArgs : WizardArgs
  {
    [NotNull]
    private readonly List<ProductDownload8InCheckbox> products = new List<ProductDownload8InCheckbox>();

    [UsedImplicitly]
    public DownloadWizardArgs()
    { 
    }

    public DownloadWizardArgs([NotNull] string username, [NotNull] string password)
    {
      Assert.ArgumentNotNull(username, "username");
      Assert.ArgumentNotNull(password, "password");
      this.UserName = username;
      this.Password = password;
    }

    [NotNull]
    public List<ProductDownload8InCheckbox> Products
    {
      get
      {
        return this.products;
      }
    }

    [NotNull]
    public override ProcessorArgs ToProcessorArgs()
    {
      return new Download8Args(this.Cookies, this.Links, ProfileManager.Profile.LocalRepository);
    }

    public ReadOnlyCollection<Uri> Links { get; set; }
    
    public string Password { get; set; }

    public string UserName { get; set; }

    public string Cookies { get; set; }

    public string[] Records { get; set; }
  }
}