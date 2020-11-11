namespace SIM.Products.ProductParsers
{
  public interface IProductParser
  {
    bool TryParseName(string packagePath, out string originalName);

    bool TryParseProduct(string packagePath, out Product product);
  }
}
