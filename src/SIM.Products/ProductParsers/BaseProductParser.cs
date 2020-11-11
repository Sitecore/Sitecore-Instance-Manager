using SIM.Extensions;
using Sitecore.Diagnostics.Base;
using System.Linq;

namespace SIM.Products.ProductParsers
{
  public class BaseProductParser : IProductParser
  {
    public virtual bool TryParseName(string path, out string originalName)
    {
      originalName = null;

      return false;
    }

    public virtual bool TryParseProduct(string path, out Product product)
    {
      product = null;

      return false;
    }

    protected internal virtual Product GetOrCreateProduct(
      string originalName,
      string packagePath,
      string twoVersion,
      string triVersion,
      string revision
      )
    {
      Assert.ArgumentNotNullOrEmpty(originalName, nameof(originalName));
      Assert.ArgumentNotNullOrEmpty(packagePath, nameof(packagePath));
      Assert.ArgumentNotNullOrEmpty(twoVersion, nameof(twoVersion));
      Assert.ArgumentNotNullOrEmpty(triVersion, nameof(triVersion));
      Assert.ArgumentNotNullOrEmpty(revision, nameof(revision));

      return ProductManager.Products.FirstOrDefault(p => p.OriginalName.Equals(originalName) && p.Revision.EqualsIgnoreCase(revision))
                ?? new Product
                {
                  OriginalName = originalName,
                  PackagePath = packagePath,
                  TwoVersion = twoVersion,
                  TriVersion = triVersion,
                  Revision = revision
                };
    }
  }
}
