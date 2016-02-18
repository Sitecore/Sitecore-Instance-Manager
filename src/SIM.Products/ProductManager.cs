namespace SIM.Products
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Linq;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  #region

  #endregion

  public static class ProductManager
  {
    #region Delegates

    public static event Action ProductManagerInitialized;

    #endregion

    #region Properties

    [NotNull]
    public static IEnumerable<Product> StandaloneProducts
    {
      get
      {
        return Products.Where(p => p.IsStandalone).OrderByDescending(p => p.SortOrder);
      }
    }

    #endregion

    #region Public Methods

    [NotNull]
    public static Product GetProduct([NotNull] string productName)
    {
      Assert.ArgumentNotNull(productName, "productName");
      var product = Products.FirstOrDefault(p => p.ToString().EqualsIgnoreCase(productName));
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

    [CanBeNull]
    public static IEnumerable<Product> GetProducts([CanBeNull] string productName, [CanBeNull] string version, [CanBeNull] string revision)
    {
      IEnumerable<Product> products = Products;
      if (!string.IsNullOrEmpty(productName))
      {
        products = products.Where(p => p.Name.EqualsIgnoreCase(productName));
      }

      if (!string.IsNullOrEmpty(version))
      {
        products = products.Where(p => p.Version == version);
      }

      if (!string.IsNullOrEmpty(revision))
      {
        products = products.Where(p => p.Revision == revision);
      }

      return products;
    }

    public static void Initialize([NotNull] string localRepository)
    {
      Assert.ArgumentNotNull(localRepository, "localRepository");

      Refresh(localRepository);
      OnProductManagerInitialized();
    }

    public static void Initialize(List<string> zipFiles)
    {
      Assert.ArgumentNotNull(zipFiles, "zipFiles");

      Refresh(zipFiles);
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
      Assert.IsNotNullOrEmpty(file, "file");

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
      Assert.ArgumentNotNull(localRepository, "localRepository");

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

    public static Product FindProduct(ProductType type, string product, string version, string revision)
    {
      var products = type == ProductType.Standalone ? ProductManager.StandaloneProducts : ProductManager.Modules;
      if (!string.IsNullOrEmpty(product))
      {
        products = products.Where(x => x.Name.Equals(product, StringComparison.OrdinalIgnoreCase));
      }

      if (!string.IsNullOrEmpty(version))
      {
        products = products.Where(x => x.Version == version);
      }
      else
      {
        products = products.OrderByDescending(x => x.Version);
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