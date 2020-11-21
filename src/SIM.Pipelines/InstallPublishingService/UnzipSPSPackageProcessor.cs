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
  public class UnzipSPSPackageProcessor : InstallSPSProcessor
  {
    protected override void ProcessCore(InstallSPSProcessorArgs args)
    {
      ZipFile.ExtractToDirectory(args.SPSPackagePath, args.SPSWebroot);
    }
  }
}
