using SIM.ContainerInstaller;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Install.Containers
{
  public class ConvertLicenseProcessor : Processor
  {
    private ISitecoreLicenseConverter converter = new SitecoreLicenseCoverter();
    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      InstallContainerArgs args = (InstallContainerArgs)arguments;
      string licensePath = Path.Combine(args.Destination, "license.xml");
      if (FileSystem.FileSystem.Local.File.Exists(licensePath))
      {
        args.EnvModel.SitecoreLicense = this.converter.Convert(licensePath);
      }
    }
  }
}
