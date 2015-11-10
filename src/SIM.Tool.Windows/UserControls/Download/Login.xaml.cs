namespace SIM.Tool.Windows.UserControls.Download
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Windows;
  using Alienlab.NetExtensions;
  using SIM.Products;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public partial class Login : IWizardStep, IFlowControl
  {
    #region Constructors

    public Login(string param)
    {
      this.InitializeComponent();
      this.TextBlock.Text = param;
    }

    #endregion

    #region IFlowControl Members

    [NotNull]
    private static string GetSdnCookie([NotNull] string username, [NotNull] string password)
    {
      Assert.ArgumentNotNull(username, "username");
      Assert.ArgumentNotNull(password, "password");

      var cookies = FormHelper.SubmitAndGetCookies(
        new Uri(@"https://sdn.sitecore.net/sdn5/misc/loginpage.aspx"), 
        @"ctl09$loginButton", 
        string.Empty, 
        new Dictionary<string, string>
        {
          {
            @"ctl09$emailTextBox", username
          }, 
          {
            @"ctl09$passwordTextBox", password
          }, 
          {
            @"SearchButton", string.Empty
          }
        });

      var cookie = cookies.GetCookies(new Uri("https://sitecore.net"))["sc_infrastructure_login"];
      if (cookie != null)
      {
        var session = cookies.GetCookies(new Uri("https://sdn.sitecore.net"))["ASP.NET_SessionId"];
        return cookie + "; " + session;
      }

      throw new InvalidOperationException("The username or password or both are incorrect, or an unexpected error happen");
    }

    bool IFlowControl.OnMovingBack(WizardArgs wizardArgs)
    {
      return true;
    }

    bool IFlowControl.OnMovingNext(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      if (!string.IsNullOrEmpty(args.Cookies) && this.UserName.Text.EqualsIgnoreCase(args.UserName) && this.Passowrd.Password.EqualsIgnoreCase(args.Password) && args.Releases.Length > 0)
      {
        return true;
      }

      args.Releases = Product.Service.GetVersions("Sitecore CMS")
        .Where(x => !x.Name.StartsWith("8"))
          .SelectMany(y => y.Releases).ToArray();

      var username = args.UserName;
      var password = args.Password;
      if (string.IsNullOrEmpty(username))
      {
        WindowHelper.HandleError("The provided username is empty", false);
        return false;
      }

      if (!username.Contains('@'))
      {
        WindowHelper.HandleError("The provided username is not an email", false);
        return false;
      }

      if (string.IsNullOrEmpty(password))
      {
        WindowHelper.HandleError("The provided password is empty", false);
        return false;
      }

      var cookies = string.Empty;
      WindowHelper.LongRunningTask(
        () => cookies = GetSdnCookie(username, password), 
        "Download Sitecore 6.x and 7.x Wizard", 
        Window.GetWindow(this), 
        "Authenticating"); // , "Validating provided credentials and getting an authentication token for downloading files");

      if (string.IsNullOrEmpty(cookies))
      {
        return false;
      }

      args.Cookies = cookies;

      return true;
    }

    #endregion

    #region IWizardStep Members

    void IWizardStep.InitializeStep(WizardArgs wizardArgs)
    {
      var args = (DownloadWizardArgs)wizardArgs;
      this.UserName.Text = args.UserName;
      this.Passowrd.Password = args.Password;
    }

    bool IWizardStep.SaveChanges(WizardArgs wizardArgs)
    {
      string username = this.UserName.Text.Trim();

      string password = this.Passowrd.Password;

      var args = (DownloadWizardArgs)wizardArgs;
      args.UserName = username;
      args.Password = password;

      return true;
    }

    #endregion
  }
}