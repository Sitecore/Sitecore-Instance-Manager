using System.Text.RegularExpressions;
using Sitecore.Diagnostics.Base;

namespace SIM.Products.ProductParsers
{
  public class ContainerProductParser : IProductParser
  {
    private const string ProductNamePattern = @"([a-zA-Z]{4,})";
    private const string ProductVersionPattern = @"(\d{1,2}\.\d{1,2}\.\d{1,2})";
    private const string ProductRevisionPattern = @"(\d{6}\.\d{1,6})";

    public static string ProductFileNamePattern { get; } = $@"{ProductNamePattern}\.{ProductVersionPattern}\.{ProductRevisionPattern}.zip$";

    public static Regex ProductRegex { get; } = new Regex(ProductFileNamePattern, RegexOptions.IgnoreCase);

    public bool TryParseName(string path, out string originalName)
    {
      string packagePath;
      string twoVersion;
      string triVersion;
      string revision;

      if (DoParse(path, out originalName, out packagePath, out twoVersion, out triVersion, out revision))
      {
        return true;
      }

      return false;
    }

    public bool TryParseProduct(string path, out Product product)
    {
      product = null;

      if (string.IsNullOrEmpty(path))
      {
        return false;
      }

      string originalName;
      string packagePath;
      string twoVersion;
      string triVersion;
      string revision;

      if (DoParse(path, out originalName, out packagePath, out twoVersion, out triVersion, out revision))
      {
        product = GetOrCreateProduct(originalName, packagePath, twoVersion, triVersion, revision);

        return true;
      }

      return false;
    }

    protected internal virtual Product GetOrCreateProduct(string originalName, string packagePath, string twoVersion, string triVersion, string revision)
    {
      return ProductManager.GetOrCreateProduct(originalName, packagePath, twoVersion, triVersion, revision);
    }

    protected virtual bool DoParse(string path, out string originalName, out string packagePath, out string twoVersion, out string triVersion, out string revision)
    {
      var match = ProductRegex.Match(path);

      if (!match.Success || match.Groups.Count < 4)
      {
        originalName = packagePath = twoVersion = triVersion = revision = default(string);

        return false;
      }

      originalName = match.Groups[1].Value;

      packagePath = path;

      string[] versions = match.Groups[2].Value.Split('.');
      twoVersion = $"{versions[0]}.{versions[1]}";

      triVersion = match.Groups[2].Value;

      revision = match.Groups[3].Value;

      return true;
    }
  }
}