namespace ContainerInstaller
{
  public interface IIdentityServerValuesGenerator
  {
    void Generate(string targetFolder,
      out string idSecret,
      out string idCertificate,
      out string idCertificatePassword
    );
  }
}