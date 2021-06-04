namespace SIM.Tool.Windows.Dialogs
{
  public class CredentialsContext
  {
    public string UserName { get; }

    public string Password { get; }

    public string Uri { get; }

    public CredentialsContext(string userName, string password, string uri = null)
    {
      this.UserName = userName;
      this.Password = password;
      this.Uri = uri;
    }
  }
}