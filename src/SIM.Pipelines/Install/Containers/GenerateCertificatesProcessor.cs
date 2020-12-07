using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using System;
using SIM.ContainerInstaller;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class GenerateCertificatesProcessor : Processor
  {
    private const string PathToCertFolder = "traefik\\certs";

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      Assert.ArgumentNotNull(args, "args");

      string destinationFolder = args.Destination;

      Assert.ArgumentNotNullOrEmpty(destinationFolder, "destinationFolder");

      string script = GetScript(args);

      PSExecutor ps = new PSScriptExecutor(destinationFolder, script);

      var result = ps.Execute();
    }

    protected virtual string GetScript(InstallContainerArgs args)
    {
      Topology topology = args.Topology;
      string path = args.Destination;

      switch (topology)
      {
        case Topology.Xm1:
          return GetXm1Script(args.EnvModel["CM_HOST"], args.EnvModel["CD_HOST"], args.EnvModel["ID_HOST"]);
        case Topology.Xp0:
          return GetXp0Script(args.EnvModel["CM_HOST"], args.EnvModel["ID_HOST"]);
        case Topology.Xp1:
          return GetXp1Script(args.EnvModel["CM_HOST"], args.EnvModel["CD_HOST"], args.EnvModel["ID_HOST"]);
        default:
          throw new InvalidOperationException("Generate certificates script cannot be resolved for '" + topology.ToString() + "'");
      }
    }

    private string GetXp1Script(string cmHost, string cdHost, string idHost)
    {
      return GetXm1Script(cmHost, cdHost, idHost);
    }

    private string GetXp0Script(string cmHost, string idHost)
    {
      string template = @"
mkcert -cert-file {0}\{1}.crt -key-file {0}\{1}.key ""{1}""
mkcert -cert-file {0}\{2}.crt -key-file {0}\{2}.key ""{2}""";

      return string.Format(template, PathToCertFolder, cmHost, idHost);
    }

    private string GetXm1Script(string cmHost, string cdHost, string idHost)
    {
      string template = @"
mkcert -cert-file {0}\{1}.crt -key-file {0}\{1}.key ""{1}""
mkcert -cert-file {0}\{2}.crt -key-file {0}\{2}.key ""{2}""
mkcert -cert-file {0}\{3}.crt -key-file {0}\{3}.key ""{3}""";

      return string.Format(template, PathToCertFolder, cmHost, idHost, cdHost);
    }
  }
}