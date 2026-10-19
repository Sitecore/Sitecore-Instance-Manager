using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace SIM
{
  [Serializable]
  public class SolrDefinition : ICloneable, IValidateable
  {
    string url;
    string adminUsername;

    [RenderInDataGreed]
    public string Name { get; set; }

    [RenderInDataGreed]
    public string Url
    {
      get => url; set => url = value?.Trim().TrimEnd('/', '#');
    }

    [RenderInDataGreed]
    public string Root { get; set; }

    [RenderInDataGreed]
    public string Service { get; set; }

    [RenderInDataGreed]
    [GridReadOnly]
    public string AdminUsername
    {
      get => adminUsername; set => adminUsername = value?.Trim();
    }

    // Plain text by design: SIM is a local dev tool.
    [RenderInDataGreed]
    [GridReadOnly]
    public string AdminPassword { get; set; }

    public bool HasCredentials
    {
      get
      {
        return !string.IsNullOrEmpty(this.AdminUsername) && !string.IsNullOrEmpty(this.AdminPassword);
      }
    }

    /// <summary>
    /// Value for the HTTP Authorization header, or null if no credentials are set.
    /// </summary>
    public string GetBasicAuthHeader()
    {
      if (!this.HasCredentials)
      {
        return null;
      }

      string token = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.AdminUsername + ":" + this.AdminPassword));
      return "Basic " + token;
    }

    public object Clone()
    {
      return new SolrDefinition()
      {
        Root = Root,
        Name = Name,
        url = Url,
        Service = Service,
        adminUsername = AdminUsername,
        AdminPassword = AdminPassword
      };
    }

    public bool HasAnyValuesInTheFields()
    {
      return !string.IsNullOrEmpty(this.Name) || !string.IsNullOrEmpty(this.Root) || !string.IsNullOrEmpty(this.Service) ||
          !string.IsNullOrEmpty(this.Url) || !string.IsNullOrEmpty(this.AdminUsername) ||
          !string.IsNullOrEmpty(this.AdminPassword);
    }

    public string ValidateAndGetError()
    {
      if (string.IsNullOrWhiteSpace(this.Name) || string.IsNullOrWhiteSpace(this.Url) ||
          string.IsNullOrWhiteSpace(this.Root))
      {
        return "Name, Root and Url must not be empty.";
      }

      Uri uri;

      try
      {
        uri = new Uri(this.Url, UriKind.Absolute);
      }
      catch (Exception)
      {
        return "Invalid solr URL.";
      }

      if (!uri.Scheme.Equals("https", StringComparison.InvariantCultureIgnoreCase))
      {
        return "Solr must be HTTPS";
      }

      if (!string.IsNullOrEmpty(uri.UserInfo))
      {
        return "Do not put credentials in the Solr URL. Use Admin Username and Admin Password instead.";
      }

      if (string.IsNullOrEmpty(this.AdminUsername) != string.IsNullOrEmpty(this.AdminPassword))
      {
        return "Admin Username and Admin Password must both be set or both be empty.";
      }

      if (!string.IsNullOrEmpty(this.AdminUsername) && this.AdminUsername.Contains(":"))
      {
        return "Admin Username must not contain ':'.";
      }

      if (!Directory.Exists(this.Root))
      {
        return "Solr root path does not exist.";
      }

      return string.Empty;
    }
  }
}
