using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CertificateValidator : BaseValidator
  {
    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      X509Store store = new X509Store(this.Data["StoreName"], StoreLocation.LocalMachine);
      store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
      X509Certificate2Collection collection = store.Certificates;
      string prefix = string.Empty;
      if (this.Data.ContainsKey("Prefix"))
      {
        prefix = this.Data["Prefix"];
      }

      foreach (InstallParam p in paramsToValidate)
      {
        string search = prefix + p.Value;
        X509Certificate2Collection fcollection = this.FindCertificates(collection,search);

        foreach (X509Certificate2 x509 in fcollection)
        {
          bool isValid = this.ValidateCertificate(x509);
          if (!isValid)
          {
            yield return new ValidationResult(ValidatorState.Error, string.Format("Certificate {0} - {1} is not valid", x509.SubjectName, x509.Thumbprint), null);
          }
          x509.Reset();
        }
      }

      store.Close();
    }

    protected internal virtual bool ValidateCertificate(X509Certificate2 x509)
    {
      X509Chain chain = this.BuildChain();
      return this.ValidateChain(chain, x509);
    }

    protected internal virtual bool ValidateChain(X509Chain chain, X509Certificate2 x509)
    {
      return chain.Build(x509);
    }

    protected internal X509Chain BuildChain()
    {
      X509Chain chain = new X509Chain();
      X509ChainPolicy chainPolicy = new X509ChainPolicy()
      {
        RevocationMode = X509RevocationMode.NoCheck,
        RevocationFlag = X509RevocationFlag.EntireChain
      };

      chain.ChainPolicy = chainPolicy;
      return chain;
    }

    protected internal virtual X509Certificate2Collection FindCertificates(X509Certificate2Collection collection, object searchValue)
    {
      X509Certificate2Collection fcollection = collection.Find(X509FindType.FindByIssuerName, searchValue, false);
      if (fcollection.Count == 0)
      {
        fcollection = collection.Find(X509FindType.FindByThumbprint, searchValue, false);
      }

      if (fcollection.Count == 0)
      {
        fcollection = collection.Find(X509FindType.FindBySubjectName, searchValue, false);
      }

      return fcollection;
    }
  }
}
