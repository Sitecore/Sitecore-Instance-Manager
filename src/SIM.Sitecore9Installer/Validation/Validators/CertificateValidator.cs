using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class CertificateValidator : BaseValidator
  {
    public override string SuccessMessage => $"Invalid certificates have not been found in {this.Data["StoreName"]}.";

    protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      string prefix = string.Empty;
      if (this.Data.ContainsKey("Prefix"))
      {
        prefix = this.Data["Prefix"];
      }

      foreach (InstallParam p in paramsToValidate)
      {
        string search = prefix + p.Value;
        X509Certificate2Collection fcollection = this.FindCertificates(search);

        foreach (X509Certificate2 x509 in fcollection)
        {
          bool isValid = this.ValidateCertificate(x509);
          if (!isValid)
          {
            yield return new ValidationResult(ValidatorState.Error, string.Format("Certificate {0} - {1} is not valid", x509.SubjectName.Name, x509.Thumbprint), null);
          }
          x509.Reset();
        }
      }
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

    protected internal virtual X509Certificate2Collection FindCertificates(object searchValue)
    {
      X509Certificate2Collection fcollection = FindInLocation(searchValue, StoreLocation.LocalMachine);
      fcollection.AddRange(FindInLocation(searchValue, StoreLocation.CurrentUser));
      return fcollection;
    }

    private X509Certificate2Collection FindInLocation(object searchValue, StoreLocation location)
    {
      X509Certificate2Collection fcollection = new X509Certificate2Collection();
      X509Store store = new X509Store(this.Data["StoreName"], location);
      try
      {
        store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
        X509Certificate2Collection collection = store.Certificates;

        fcollection = collection.Find(X509FindType.FindBySubjectName, searchValue, false);
        if (fcollection.Count == 0)
        {
          fcollection = collection.Find(X509FindType.FindByThumbprint, searchValue, false);
        }        
      }
      finally
      {
        store.Close();
      }

      return fcollection;
    }
  }
}
