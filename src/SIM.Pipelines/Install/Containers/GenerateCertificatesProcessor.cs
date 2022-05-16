using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using System;
using SIM.ContainerInstaller;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using SIM.Loggers;
using SIM.ContainerInstaller.Modules;

namespace SIM.Pipelines.Install.Containers
{
  [UsedImplicitly]
  public class GenerateCertificatesProcessor : Processor
  {
    private string ProcessorName => this.GetType().Name;

    private ILogger _logger;

    [NotNull]
    private ILogger Logger
    {
      get
      {
        if (_logger == null)
        {
          _logger = new EmptyLogger();
        }

        return _logger;
      }
      set
      {
        _logger = value;
      }
    }

    private const string PathToCertFolder = "traefik\\certs";

    private const string PathToDynamicConfigFolder = "traefik\\config\\dynamic";

    private const string CertsConfigFileName = "certs_config.yaml";

    protected override void Process([NotNull] ProcessorArgs arguments)
    {
      Assert.ArgumentNotNull(arguments, "arguments");

      InstallContainerArgs args = (InstallContainerArgs)arguments;

      this._logger = args.Logger;

      Assert.ArgumentNotNull(args, "args");

      string destinationFolder = args.Destination;

      Assert.ArgumentNotNullOrEmpty(destinationFolder, "destinationFolder");

      UpdateTlsDynamicConfig(args);

      string script = GetScript(args);

      PSExecutor ps = new PSScriptExecutor(destinationFolder, script);

      ExecuteScript(() => ps.Execute());
    }

    private void UpdateTlsDynamicConfig(InstallContainerArgs args)
    {
      string yamlContent = GetConfig(args);

      string yamlFileName = Path.Combine(args.Destination, PathToDynamicConfigFolder, CertsConfigFileName);

      try
      {
        UpdateConfigFile(yamlFileName, yamlContent);
      }
      catch (Exception ex)
      {
        args.Logger.Error($"Could not update the '{CertsConfigFileName}' file. Message: {ex.Message}");

        throw;
      }
    }

    private string GetConfig(InstallContainerArgs args)
    {
      Topology topology = args.Topology;

      string pathToCerts = @"C:\etc\traefik\certs";

      switch (topology)
      {
        case Topology.Xm1:
        case Topology.Xp1:
          return $@"tls:
  certificates:
    - certFile: {pathToCerts}\{args.EnvModel.CmHost}.crt
      keyFile: {pathToCerts}\{args.EnvModel.CmHost}.key
    - certFile: {pathToCerts}\{args.EnvModel.CdHost}.crt
      keyFile: {pathToCerts}\{args.EnvModel.CdHost}.key
    - certFile: {pathToCerts}\{args.EnvModel.IdHost}.crt
      keyFile: {pathToCerts}\{args.EnvModel.IdHost}.key
";
        case Topology.Xp0:
          return $@"tls:
  certificates:
    - certFile: {pathToCerts}\{args.EnvModel.CmHost}.crt
      keyFile: {pathToCerts}\{args.EnvModel.CmHost}.key
    - certFile: {pathToCerts}\{args.EnvModel.IdHost}.crt
      keyFile: {pathToCerts}\{args.EnvModel.IdHost}.key
";
        default:
          throw new InvalidOperationException("Config is not defined for '" + topology.ToString() + "' topology.");
      }
    }

    private void UpdateConfigFile(string fileName, string content)
    {
      File.WriteAllText(fileName, content);
    }

    protected virtual string GetScript(InstallContainerArgs args)
    {
      Topology topology = args.Topology;

      switch (topology)
      {
        case Topology.Xm1:
          return GetXm1Script(args.EnvModel.CmHost, args.EnvModel.CdHost, args.EnvModel.IdHost);
        case Topology.Xp0:
          return GetXp0Script(args.EnvModel.CmHost, args.EnvModel.IdHost);
        case Topology.Xp1:
          return GetXp1Script(args.EnvModel.CmHost, args.EnvModel.CdHost, args.EnvModel.IdHost);
        default:
          throw new InvalidOperationException("Generate certificates script cannot be resolved for '" + topology.ToString() + "'");
      }
    }

    private void ExecuteScript(Func<Collection<PSObject>> p)
    {
      try
      {
        p.Invoke();
      }
      catch (CommandNotFoundException ex)
      {
        this._logger.Error($"Failed to generate certificates in '{this.ProcessorName}'. Message: {ex.Message}");

        this._logger.Info(
          $"{this.ProcessorName}: please make sure that 'mkcert.exe' has been installed. See 'https://containers.doc.sitecore.com/docs/run-sitecore#install-mkcert' for additional details.");

        throw;
      }
      catch (Exception ex)
      {
        this._logger.Error($"Failed to generate certificates in '{this.ProcessorName}'. Message: {ex.Message}");

        throw;
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