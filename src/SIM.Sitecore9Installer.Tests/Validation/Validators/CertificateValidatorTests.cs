using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using SIM.Sitecore9Installer.Validation.Validators;
using Xunit;
using Task = SIM.Sitecore9Installer.Tasks.Task;

namespace SIM.Sitecore9Installer.Tests.Validation.Validators
{
  public class CertificateValidatorTests
  {

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void CertificateDoesNotExist(IEnumerable<Task> tasks)
    {
      string paramName = "somename";
      tasks.First().LocalParams.AddOrUpdateParam(paramName, "somevalue",InstallParamType.String);
      CertificateValidator val = Substitute.ForPartsOf<CertificateValidator>();
      val.WhenForAnyArgs(a=>a.FindCertificates(null)).DoNotCallBase();
      val.FindCertificates(null).ReturnsForAnyArgs(new X509Certificate2Collection());
      val.Data["StoreName"] = "Root";
      val.Data["ParamNames"] = paramName;
      
      Assert.DoesNotContain(val.Evaluate(tasks), r => r.State == Sitecore9Installer.Validation.ValidatorState.Error);
      val.DidNotReceiveWithAnyArgs().ValidateCertificate(null);

    }

    [Theory]
    [ClassData(typeof(ValidatorTestSetup))]
    public void CertificateIsValid(IEnumerable<Task> tasks)
    {
      string paramName = "somename";
      tasks.First().LocalParams.AddOrUpdateParam(paramName, "somevalue", InstallParamType.String);
      CertificateValidator val = Substitute.ForPartsOf<CertificateValidator>();
      val.WhenForAnyArgs(a => a.FindCertificates( null)).DoNotCallBase();
      X509Certificate2Collection collection = new X509Certificate2Collection();
      collection.Add(new X509Certificate2());
      val.FindCertificates( null).ReturnsForAnyArgs(collection);
      val.Data["StoreName"] = "Root";
      val.Data["ParamNames"] = paramName;
      val.WhenForAnyArgs(a=>a.ValidateChain(null,null)).DoNotCallBase();
      val.ValidateChain(null, null).ReturnsForAnyArgs(true);

      Assert.DoesNotContain(val.Evaluate(tasks), r => r.State == Sitecore9Installer.Validation.ValidatorState.Error);
      val.Received().ValidateCertificate(collection[0]);
    }
  }
}
