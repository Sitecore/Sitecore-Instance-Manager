using ContainerInstaller;
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
      if (FileSystem.FileSystem.Local.File.Exists(args.EnvModel.SitecoreLicense))
      {
        string licensePath = Path.Combine(args.Destination, "license.xml");
        FileSystem.FileSystem.Local.File.Copy(args.EnvModel.SitecoreLicense, licensePath);
        args.EnvModel.SitecoreLicense=this.converter.Convert(licensePath);
      }
    }
  }
}
