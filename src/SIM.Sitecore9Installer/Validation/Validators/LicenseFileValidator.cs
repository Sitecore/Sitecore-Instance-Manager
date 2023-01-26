using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace SIM.Sitecore9Installer.Validation.Validators
{
  public class LicenseFileValidator : IValidator
  {
    private const string expirationNodeName = "expiration";

    private const string expirationNodeDateTimeFormat = "yyyyMMdd'T'HHmmss";

    private const string expirationNodeParentNodeAttribute = "Id";

    private readonly Dictionary<string, List<string>> ExpiredDatesAndLicences;

    private readonly Dictionary<string, List<string>> AlmostExpiredDatesAndLicences;

    public LicenseFileValidator()
    {
      Data = new Dictionary<string, string>();
      ExpiredDatesAndLicences = new Dictionary<string, List<string>>();
      AlmostExpiredDatesAndLicences = new Dictionary<string, List<string>>();
    }

    public Dictionary<string, string> Data { get; set; }

    public virtual string SuccessMessage => "The license file has been located successfully.";

    public IEnumerable<ValidationResult> Evaluate(IEnumerable<Tasks.Task> tasks)
    {
      bool hasErrors = false;
      IEnumerable<InstallParam> licenseFilePaths = tasks.Select(t => t.LocalParams.FirstOrDefault(l => l.Name == Data["LicenseFileVariable"]))
        .Where(p => p != null);
      IEnumerable<string> uniquelicenseFilePaths = licenseFilePaths.Select(x => x.Value).Distinct();

      foreach (string path in uniquelicenseFilePaths)
      {
        if (!FileExists(path))
        {
          yield return new ValidationResult(ValidatorState.Error, $"Unable to locate the '{path}' license file.", null);
          hasErrors = true;
        }
        else
        {
          XmlDocument doc = new XmlDocument();
          doc.Load(path);

          XmlNodeList expirationNodes = doc.GetElementsByTagName(expirationNodeName);
          foreach (XmlNode expirationNode in expirationNodes)
          {
            DateTime expirationDateTime = DateTime.ParseExact(expirationNode.InnerText, expirationNodeDateTimeFormat, CultureInfo.InvariantCulture);
            if (expirationDateTime.Date <= DateTime.Now.Date)
            {
              string expiredShortDate = expirationDateTime.ToShortDateString();
              string expiredLicense = expirationNode.ParentNode.ParentNode.Attributes[expirationNodeParentNodeAttribute].Value;
              if (!ExpiredDatesAndLicences.ContainsKey(expiredShortDate))
              {
                ExpiredDatesAndLicences.Add(expiredShortDate, new List<string>() { expiredLicense });
              }
              else
              {
                ExpiredDatesAndLicences[expiredShortDate].Add(expiredLicense);
              }
            }
            else if (expirationDateTime.Date < DateTime.Now.AddMonths(1).Date)
            {
              string almostExpiredShortDate = expirationDateTime.ToShortDateString();
              string almostExpiredLicense = expirationNode.ParentNode.ParentNode.Attributes[expirationNodeParentNodeAttribute].Value;
              if (!AlmostExpiredDatesAndLicences.ContainsKey(almostExpiredShortDate))
              {
                AlmostExpiredDatesAndLicences.Add(almostExpiredShortDate, new List<string>() { almostExpiredLicense });
              }
              else
              {
                AlmostExpiredDatesAndLicences[almostExpiredShortDate].Add(almostExpiredLicense);
              }
            }
          }

          if (ExpiredDatesAndLicences.Keys.Any())
          {
            hasErrors = true;
            foreach (KeyValuePair<string, List<string>> expiredDateAndLicences in ExpiredDatesAndLicences)
            {
              yield return new ValidationResult(ValidatorState.Error,
                $"The '{path}' license file contains the following licenses that have been expired on '{expiredDateAndLicences.Key}':\n\n{string.Join("\n", expiredDateAndLicences.Value)}", null);
            }
          }

          if (AlmostExpiredDatesAndLicences.Keys.Any())
          {
            hasErrors = true;
            foreach (KeyValuePair<string, List<string>> almostExpiredDateAndLicences in AlmostExpiredDatesAndLicences)
            {
              yield return new ValidationResult(ValidatorState.Warning,
                $"The '{path}' license file contains the following licenses that expire soon on '{almostExpiredDateAndLicences.Key}':\n\n{string.Join("\n", almostExpiredDateAndLicences.Value)}", null);
            }
          }
        }
      }

      if (!hasErrors)
      {
        yield return new ValidationResult(ValidatorState.Success, SuccessMessage, null);
      }
    }

    protected internal virtual bool FileExists(string path) => File.Exists(path);
  }
}
