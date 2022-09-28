using JetBrains.Annotations;
using SIM.Pipelines.Processors;
using Sitecore.Diagnostics.Base;
using System;
using SIM.ContainerInstaller;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using SIM.Loggers;
using YamlDotNet.RepresentationModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using YamlDotNet.Serialization;

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

    private const string PathToCerts = @"C:\etc\traefik\certs";

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

      UpdateCertsConfigFile(args);

      string script = GetScript(args.EnvModel);

      PSExecutor ps = new PSScriptExecutor(destinationFolder, script);

      ExecuteScript(() => ps.Execute());
    }

    private void UpdateCertsConfigFile(InstallContainerArgs args)
    {
      YamlDocument yamlDocument = GenerateCertsConfigFile(args.EnvModel);

      string yamlFilePath = Path.Combine(args.Destination, PathToDynamicConfigFolder, CertsConfigFileName);

      try
      {
        Serializer serializer = new Serializer();
        using (FileStream fileStream = File.OpenWrite(yamlFilePath))
        using (StreamWriter streamWriter = new StreamWriter(fileStream))
        {
          serializer.Serialize(streamWriter, yamlDocument.RootNode);
        }
      }
      catch (Exception ex)
      {
        args.Logger.Error($"Could not update the '{CertsConfigFileName}' file. Message: {ex.Message}");

        throw;
      }
    }

    private List<string> GetHostnames(EnvModel envModel)
    {
      Regex regex = new Regex(DockerSettings.HostNameKeyPattern);

      string[] keys = envModel.GetNames().ToArray();

      IEnumerable<string> hostNamesKeys = keys.Where(n => regex.IsMatch(n));

      List<string> hostNames = new List<string>();

      foreach (string hostName in hostNamesKeys)
      {
        hostNames.Add(envModel[hostName]);
      }

      return hostNames;
    }

    private YamlDocument GenerateCertsConfigFile(EnvModel envModel)
    {
      List<YamlNode> certificates = new List<YamlNode>();

      foreach (string hostName in GetHostnames(envModel))
      {
        certificates.Add(new YamlMappingNode(
          new YamlScalarNode("certFile"), new YamlScalarNode($@"{PathToCerts}\{hostName}.crt"),
          new YamlScalarNode("keyFile"), new YamlScalarNode($@"{PathToCerts}\{hostName}.key")
        ));
      }

      return new YamlDocument(
         new YamlMappingNode(
           new YamlScalarNode("tls"), new YamlMappingNode(
             new YamlScalarNode("certificates"), new YamlSequenceNode(certificates))));
    }

    protected virtual string GetScript(EnvModel envModel)
    {
      string template = string.Empty;

      foreach (string hostName in GetHostnames(envModel))
      {
        template += Environment.NewLine + $@"mkcert -cert-file {PathToCertFolder}\{hostName}.crt -key-file {PathToCertFolder}\{hostName}.key ""{hostName}""";
      }

      return template;
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
  }
}