﻿using System.Collections.Generic;
using System.IO;
using Microsoft.Web.Administration;
using SIM.Adapters.WebServer;
using SIM.Pipelines.Install;
using SIM.Products;
using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;

namespace SIM.Pipelines
{
  using SIM.Extensions;

  public class SetupWebsiteHelper
  {
    #region Constants

    private const string DataFolderConfigFilePattern = @"<configuration xmlns:patch=""http://www.sitecore.net/xmlconfig/"">
  <sitecore>
    <sc.variable name=""dataFolder"">
      <patch:attribute name=""value"">{0}</patch:attribute>
    </sc.variable>
  </sitecore>
</configuration>";

    private const string NetFrameworkV2 = "v2.0";

    #endregion

    #region Public methods

    [NotNull]
    public static string ChooseAppPoolName([NotNull] string name, [NotNull] ApplicationPoolCollection applicationPools)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(applicationPools, nameof(applicationPools));

      var modifier = 0;
      var newname = name;
      while (applicationPools[newname] != null)
      {
        newname = name + '_' + ++modifier;
      }

      return newname;
    }

    public static void SetDataFolder([NotNull] string rootFolderPath, [CanBeNull] string dataFolderPath = null)
    {
      Assert.ArgumentNotNull(rootFolderPath, nameof(rootFolderPath));

      var dataFolder = Path.Combine(rootFolderPath, @"Website\App_Config\Include\zzz\DataFolder.config");

      // Sitecore 9 environment configuration folder
      var envDataFolder = Path.Combine(rootFolderPath, @"Website\App_Config\Environment\DataFolder.config");
      if (new FileInfo(envDataFolder).Directory.Exists)
      {
        dataFolder = envDataFolder;
      }

      var dir = Path.GetDirectoryName(dataFolder);
      if (!Directory.Exists(dir))
      {
        Directory.CreateDirectory(dir);
      }

      dataFolderPath = dataFolderPath ?? Path.Combine(rootFolderPath, "Data");
      var example = dataFolder + ".example";
      FileSystem.FileSystem.Local.File.DeleteIfExists(example);

      FileSystem.FileSystem.Local.File.WriteAllText(dataFolder, string.Format(DataFolderConfigFilePattern, dataFolderPath));
    }

    public static long SetupWebsite(bool enable32BitAppOnWin64, string webRootPath, bool forceNetFramework4, bool isClassic, 
      IEnumerable<BindingInfo> bindings, string name)
    {
      long siteId;
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext())
      {
        ApplicationPool appPool = context.ApplicationPools.Add(ChooseAppPoolName(name, context.ApplicationPools));
        appPool.ManagedRuntimeVersion = NetFrameworkV2;
        var id = Settings.CoreInstallWebServerIdentity.Value;
        ProcessModelIdentityType type = GetIdentityType(id);
        appPool.ProcessModel.IdentityType = type;
        appPool.ProcessModel.PingingEnabled = false;
        if (forceNetFramework4)
        {
          appPool.SetAttributeValue("managedRuntimeVersion", "v4.0");
        }

        appPool.Enable32BitAppOnWin64 = enable32BitAppOnWin64;
        appPool.ManagedPipelineMode = isClassic ? ManagedPipelineMode.Classic : ManagedPipelineMode.Integrated;

        if (type == ProcessModelIdentityType.SpecificUser)
        {
          var password = Settings.CoreInstallWebServerIdentityPassword.Value;
          appPool.ProcessModel.UserName = id;
          appPool.ProcessModel.Password = password;
        }

        Product product = Product.Parse(ProductHelper.DetectProductFullName(webRootPath));

        if (product.Name.EqualsIgnoreCase("Sitecore CMS"))
        {
          var ver = product.TwoVersion;
          if (ver.StartsWith("6.0") || ver.StartsWith("6.1"))
          {
            appPool.Enable32BitAppOnWin64 = true;
            appPool.ManagedPipelineMode = ManagedPipelineMode.Classic;
          }
          else if (ver.StartsWith("6.2"))
          {
            appPool.Enable32BitAppOnWin64 = true;
          }
        }

        Site site = null;
        foreach (BindingInfo binding in bindings)
        {
          var bindingInformation = $"{binding.IP}:{binding.Port}:{binding.Host}";
          if (site == null)
          {
            site = context.Sites.Add(name, binding.Protocol, bindingInformation, webRootPath);
          }
          else
          {
            site.Bindings.Add(bindingInformation, binding.Protocol);
          }
        }

        Assert.IsNotNull(site, nameof(site));
        siteId = site.Id;
        site.ApplicationDefaults.ApplicationPoolName = name;
        context.CommitChanges();
      }

      return siteId;
    }

    #endregion

    #region Private methods

    private static ProcessModelIdentityType GetIdentityType([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      if (name.EqualsIgnoreCase("NetworkService"))
      {
        return ProcessModelIdentityType.NetworkService;
      }

      if (name.EqualsIgnoreCase("ApplicationPoolIdentity"))
      {
        return ProcessModelIdentityType.ApplicationPoolIdentity;
      }

      if (name.EqualsIgnoreCase("LocalService"))
      {
        return ProcessModelIdentityType.LocalService;
      }

      if (name.EqualsIgnoreCase("LocalSystem"))
      {
        return ProcessModelIdentityType.LocalSystem;
      }

      return ProcessModelIdentityType.SpecificUser;
    }

    #endregion
  }
}