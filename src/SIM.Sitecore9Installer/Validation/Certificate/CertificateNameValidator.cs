using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using SitecoreInstaller.Validation.Abstractions;

namespace SitecoreInstaller.Validation.Certificate
{
    public class CertificateNameValidator//: IInstallationValidator//todo: add checks if certificate exists
    {
        public CertificateNameValidator()
        {
            Name = "CertificateNameValidator";
        }

        public bool IsExpired(string thumbprint)
        {
            if (String.IsNullOrEmpty(thumbprint))
                return true;
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.LocalMachine); //todo: make flexible, possible to get different storenames and locations
            certStore.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = certStore.Certificates.Find(
                X509FindType.FindByThumbprint, thumbprint, false);
            certStore.Close();
            if (certCollection.Count > 0)
            {
                if (DateTime.Now > certCollection[0].NotAfter)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public string Name { get; set; }

        public ValidationResult Result { get; }

      public ValidationResult Validate(Dictionary<string, string> installParams)
      {
        return ValidationResult.None;
      }

      public ValidationResult Validate()
      {
        throw new NotImplementedException();
      }

      public string Details { get => null;
          set => Details = value;
        }

        public ValidationResult Validate(string siteName)
        {
            return ValidationResult.Warning;
        }
    }
}
