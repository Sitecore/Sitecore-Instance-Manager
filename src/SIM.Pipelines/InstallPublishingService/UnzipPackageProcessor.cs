using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.InstallPublishingService
{
  public class UnzipPackageProcessor : InstallPublishingServiceProcessor
  {
    protected override void ProcessCore(InstallPublishingServiceProcessorArgs args)
    {
      ZipFile.ExtractToDirectory(args.PublishingServicePackagePath, args.PublishingServiceWebroot);
    }
  }
}
