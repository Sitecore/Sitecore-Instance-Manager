namespace SIM.Tool.Windows.UserControls.Download8
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Windows;
  using Alienlab.NetExtensions;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Wizards;

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

    private static string GetMarketplaceCookie(string username, string password)
    {
      const string baseUri = "https://dev.sitecore.net";
      var request = FormHelper.CreatePostRequest(new Uri(baseUri + @"/api/authorization"));
      request.ContentType = @"application/json;charset=UTF-8";
      var cookies = new CookieContainer();
      request.CookieContainer = cookies;
      var content = "{" + string.Format("\"username\":\"{0}\",\"password\":\"{1}\"", username, password) + "}";
      request.ContentLength = content.Length;
      using (var inputStream = request.GetRequestStream())
      {
        using (var writer = new StreamWriter(inputStream))
        {
          writer.Write(content);
        }
      }

      using (var response = (HttpWebResponse)request.GetResponse())
      {
        using (var responseStream = response.GetResponseStream())
        {
          using (var streamReader = new StreamReader(responseStream))
          {
            if (streamReader.ReadToEnd() == "true")
            {
              var sitecoreNetCookies = cookies.GetCookies(new Uri("http://sitecore.net"));
              var marketplaceCookie = sitecoreNetCookies["marketplace_login"];
              if (marketplaceCookie == null)
              {
                throw new InvalidOperationException("The username or password or both are incorrect, or an unexpected error happen");
              }

              return marketplaceCookie + "; " + GetSessionCookie(baseUri);
            }
          }
        }
      }


      throw new InvalidOperationException("The username or password or both are incorrect, or an unexpected error happen");
    }

    private static string GetSessionCookie(string url)
    {
      var request = FormHelper.CreateRequest(new Uri(url));
      var cookies = new CookieContainer();
      request.CookieContainer = cookies;

      using (request.GetResponse())
      {
        return cookies.GetCookies(new Uri("http://dev.sitecore.net"))["ASP.NET_SessionId"].ToString();
      }
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
        () => cookies = GetMarketplaceCookie(username, password), 
        "Download Sitecore 8.x Wizard", 
        Window.GetWindow(this), 
        "Authenticating");

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