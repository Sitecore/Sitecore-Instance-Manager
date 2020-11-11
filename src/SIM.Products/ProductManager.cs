using SIM.Products.ProductParsers;

namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  #region

  #endregion

  public static class ProductManager
  {
    #region Delegates

    public static event Action ProductManagerInitialized;

    #endregion

    private static IProductParser[] _productParsers;

    #region Properties

    [NotNull]
    public static IEnumerable<Product> StandaloneProducts
    {
      get
      {
        return Products.Where(p => p.IsStandalone).OrderByDescending(p => p.SortOrder);
      }
    }

    public static IEnumerable<Product> ContainerProducts
    {
      get
      {
        return Products.Where(p => p.IsContainer).OrderByDescending(p => p.SortOrder);
      }
    }

    public static IProductParser[] ProductParsers
    {
      get
      {
        if (_productParsers == null)
        {
          _productParsers = new IProductParser[] {new ContainerProductParser()};
        }

        return _productParsers;
      }
      
    }

    #endregion

    #region Public Methods

    public static Product GetProduct(FileInfo file)
    {
      return GetProduct(file.FullName);
    }

    [NotNull]
    public static Product GetProduct([NotNull] string productName)
    {
      Assert.ArgumentNotNull(productName, nameof(productName));
      var product = Products.FirstOrDefault(p => p.ToString(false).EqualsIgnoreCase(productName) || p.ToString(true).EqualsIgnoreCase(productName));
      if (product == null)
      {
        product = Products.FirstOrDefault(p => p.ToString().EqualsIgnoreCase(productName.Replace(".0.0", ".0")));
      }

      if (product != null)
      {
        return product;
      }

      var hotfixPos = productName.ToLowerInvariant().IndexOf("hotfix");
      if (hotfixPos >= 0)
      {
        var productNameWithoutHotfix = productName.Substring(0, hotfixPos).TrimEnd();
        product = Products.FirstOrDefault(p => p.ToString().EqualsIgnoreCase(productNameWithoutHotfix));
        if (product != null)
        {
          return product;
        }
      }

      return Product.Parse(productName);
    }

    public static void Initialize([NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(localRepository, nameof(localRepository));

      Refresh(localRepository);
      OnProductManagerInitialized();
    }

    #endregion

    #region Methods

    private static string[] GetProductFiles(string localRepository)
    {
      using (new ProfileSection("Get product files"))
      {
        ProfileSection.Argument("localRepository", localRepository);

        var zipFiles = FileSystem.FileSystem.Local.Directory.GetFiles(localRepository, "*.zip", SearchOption.AllDirectories);

        return ProfileSection.Result(zipFiles);
      }
    }

    private static void OnProductManagerInitialized()
    {
      Action handler = ProductManagerInitialized;
      if (handler != null)
      {
        handler();
      }
    }

    private static void ProcessFile(string file)
    {
      Assert.IsNotNullOrEmpty(file, nameof(file));

      using (new ProfileSection("Process file"))
      {
        ProfileSection.Argument("file", file);

        Product product;
        if (!Product.TryParse(file, out product))
        {
          ProfileSection.Result("Skipped (not a product)");
          return;
        }

        if (Products.Any(p => p.ToString().EqualsIgnoreCase(product.ToString())))
        {
          ProfileSection.Result("Skipped (already exist)");
          return;
        }

        Products.Add(product);
        ProfileSection.Result("Added");
      }
    }

    private static void Refresh([NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(localRepository, nameof(localRepository));

      using (new ProfileSection("Refresh product manager"))
      {
        ProfileSection.Argument("localRepository", localRepository);

        if (!string.IsNullOrEmpty(localRepository))
        {
          Assert.IsNotNull(localRepository.EmptyToNull(), "The Local Repository folder isn't specified in the Settings dialog");
          try
          {
            FileSystem.FileSystem.Local.Directory.AssertExists(localRepository, "The Local Repository folder ('{0}') doesn't exist".FormatWith(localRepository));
          }
          catch (Exception ex)
          {
            throw new ConfigurationErrorsException(ex.Message);
          }

          if (FileSystem.FileSystem.Local.Directory.Exists(localRepository))
          {
            var zipFiles = GetProductFiles(localRepository);

            Refresh(zipFiles);
          }
        }
      }
    }

    private static void Refresh(IEnumerable<string> zipFiles)
    {
      using (new ProfileSection("Refresh product manager"))
      {
        ProfileSection.Argument("zipFiles", zipFiles);

        Products.Clear();
        Modules.Clear();
        foreach (string file in zipFiles)
        {
          ProcessFile(file);
        }

        Modules.AddRange(Products.Where(p => !p.IsStandalone));
      }
    }

    #endregion

    #region Fields

    public static readonly List<Product> Modules = new List<Product>();

    public static readonly List<Product> Products = new List<Product>();

    #endregion

    public static Product FindProduct(ProductType type, [CanBeNull] string product, [CanBeNull] string version, [CanBeNull] string revision)
    {
      var products = type == ProductType.Standalone ? StandaloneProducts : Modules;
      if (!string.IsNullOrEmpty(product))
      {
        products = products.Where(x => x.Name.Equals(product, StringComparison.OrdinalIgnoreCase));
      }

      if (!string.IsNullOrEmpty(version))
      {
        products = products.Where(x => x.TwoVersion == version);
      }
      else
      {
        products = products.OrderByDescending(x => x.TwoVersion);
      }

      if (!string.IsNullOrEmpty(revision))
      {
        products = products.Where(x => x.Revision == revision);
      }
      else
      {
        products = products.OrderByDescending(x => x.Revision);
      }

      var distributive = products.FirstOrDefault();
      return distributive;
    }
  }
}