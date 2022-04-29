using System.Collections.Generic;
using YamlDotNet.RepresentationModel;

namespace SIM.ContainerInstaller.Modules
{
  public interface IYamlFileGeneratorHelper
  {
    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor100();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrArgsFor100();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor100();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor100();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor101();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor101();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor101();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor101();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlArgsFor102();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateMsSqlInitArgsFor102();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateSolrInitArgsFor102();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCdArgsFor102();

    IEnumerable<KeyValuePair<YamlNode, YamlNode>> GenerateCmArgsFor102();
  }
}