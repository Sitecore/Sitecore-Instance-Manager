using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using JetBrains.Annotations;
using Sitecore.Diagnostics.Base;

namespace SIM.ContainerInstaller
{
  [UsedImplicitly]
  public class IdentityServerValuesGenerator : IIdentityServerValuesGenerator
  {
    public void Generate([NotNull] string targetFolder,
      out string idSecret,
      out string idCertificate,
      out string idCertificatePassword
      )
    {
      Assert.ArgumentNotNull(targetFolder, "targetFolder");

      string scriptFile = Path.Combine(Directory.GetCurrentDirectory(), "ContainerFiles/scripts/IdentityServerScript.ps1");
      PSFileExecutor ps = new PSFileExecutor(scriptFile, Path.GetDirectoryName(targetFolder));
      Collection<PSObject> results = ps.Execute();

      if (results.Count < 3)
      {
        throw new InvalidOperationException("Error in: IdentityServerVariablesGenerator. PS script has returned less than 3 results.");
      }

      idSecret = results[0].ToString();
      idCertificate = results[1].ToString();
      idCertificatePassword = results[2].ToString();

      Assert.ArgumentNotNullOrEmpty(idSecret, "idSecret");
      Assert.ArgumentNotNullOrEmpty(idCertificate, "idCertificate");
      Assert.ArgumentNotNullOrEmpty(idCertificatePassword, "idCertificatePassword");
    }
  }
}