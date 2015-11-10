namespace SIM.Pipelines.Import
{
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;
  using System.Xml;
  using SIM.Adapters.WebServer;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  internal class ImportRegisterWebsite : ImportProcessor
  {
    #region Public methods

    public string CreateNewAppPoolName(string oldName)
    {
      List<string> poolsNames = new List<string>();
      foreach (var appPool in WebServerManager.CreateContext(string.Empty).ApplicationPools)
      {
        poolsNames.Add(appPool.Name);
      }

      bool flag = false;
      while (!flag)
      {
        if ((from t in poolsNames
          where t == oldName
          select t).FirstOrDefault() == null)
        {
          return oldName;
        }
        else
        {
          oldName += "_imported";
        }
      }

      return null;
    }

    public long? CreateNewID(long? oldID)
    {
      using (WebServerManager.WebServerContext context = WebServerManager.CreateContext("InstanceMgr.Init"))
      {
        var instances = context.Sites;
        return oldID == null || instances.Any(x => x.Id == oldID) ? instances.Max(x => x.Id) + 1 : oldID;
      }
    }

    public string CreateNewName(string[] names, string oldName)
    {
      bool flag = false;
      while (!flag)
      {
        if ((from t in names
          where t == oldName
          select t).FirstOrDefault() == null)
        {
          return oldName;
        }
        else
        {
          oldName += "_imported";
        }
      }

      return null;
    }

    public string CreateNewSiteName(IEnumerable<Instance> instances, string oldName)
    {
      List<string> sitesNames = new List<string>();
      foreach (var instance in instances)
      {
        sitesNames.Add(instance.Name);
      }

      bool flag = false;
      while (!flag)
      {
        if ((from t in sitesNames
          where t == oldName
          select t).FirstOrDefault() == null)
        {
          return oldName;
        }
        else
        {
          oldName += "_imported";
        }
      }

      return null;
    }

    #endregion

    #region Protected methods

    protected override void Process(ImportArgs args)
    {
      // var websiteName = args.Instance.Name;
      // var appPoolName = WebServerManager.CreateContext(string.Empty).Sites[websiteName].ApplicationDefaults.ApplicationPoolName;        
      this.ChangeAppPoolSettingsIfNeeded(args.temporaryPathToUnpack.PathCombine(ImportArgs.appPoolSettingsFileName), args);
      this.ChangeWebsiteSettingsIfNeeded(args.temporaryPathToUnpack.PathCombine(ImportArgs.websiteSettingsFileName), args);
      var importAppPoolSettingsCommand = string.Format(@"%windir%\system32\inetsrv\appcmd add apppool /in < {0}", args.temporaryPathToUnpack.PathCombine(ImportArgs.appPoolSettingsFileName) + ".fixed.xml");
      var importWebsiteSettingsCommand = string.Format(@"%windir%\system32\inetsrv\appcmd add site /in < {0}", args.temporaryPathToUnpack.PathCombine(ImportArgs.websiteSettingsFileName) + ".fixed.xml");

      ExecuteCommand(importAppPoolSettingsCommand);
      ExecuteCommand(importWebsiteSettingsCommand);
    }

    #endregion

    #region Private methods

    private static void ExecuteCommand(string command)
    {
      var procStartInfo = new ProcessStartInfo("cmd", "/c " + command)
      {
        UseShellExecute = false, 
        CreateNoWindow = true
      };

      var proc = new Process
      {
        StartInfo = procStartInfo
      };
      proc.Start();
      proc.WaitForExit();
    }

    private void ChangeAppPoolSettingsIfNeeded(string path, ImportArgs args)
    {
      // should be executed before ChangeWebsiteSettingsIfNeeded
      // need to change AppName
      XmlDocumentEx appPoolSettings = new XmlDocumentEx();
      appPoolSettings.Load(path);
      args.appPoolName = this.CreateNewAppPoolName(args.appPoolName);
      appPoolSettings.SetElementAttributeValue("/appcmd/APPPOOL", "APPPOOL.NAME", args.appPoolName);
      appPoolSettings.SetElementAttributeValue("appcmd/APPPOOL/add", "name", args.appPoolName);

      appPoolSettings.Save(appPoolSettings.FilePath + ".fixed.xml");
    }

    private void ChangeRootFolderIfNeeded(string path)
    {
    }

    private void ChangeWebsiteSettingsIfNeeded(string path, ImportArgs args)
    {
      XmlDocumentEx websiteSettings = new XmlDocumentEx();
      websiteSettings.Load(path);
      args.siteName = this.CreateNewSiteName(InstanceManager.Instances, args.siteName);
      websiteSettings.SetElementAttributeValue("/appcmd/SITE", "SITE.NAME", args.siteName);
      websiteSettings.SetElementAttributeValue("/appcmd/SITE/site", "name", args.siteName);

      websiteSettings.SetElementAttributeValue("/appcmd/SITE", "bindings", "http/*:80:" + args.siteName);

      // need to change site ID
      args.siteID = this.CreateNewID(args.siteID);
      websiteSettings.SetElementAttributeValue("/appcmd/SITE", "SITE.ID", args.siteID.ToString());
      websiteSettings.SetElementAttributeValue("/appcmd/SITE/site", "id", args.siteID.ToString());

      // change apppool name
      websiteSettings.SetElementAttributeValue("/appcmd/SITE/site/application", "applicationPool", args.appPoolName);
      websiteSettings.SetElementAttributeValue("/appcmd/SITE/site/applicationDefaults", "applicationPool", args.appPoolName);

      // change root folder
      websiteSettings.SetElementAttributeValue("/appcmd/SITE/site/application/virtualDirectory", "physicalPath", args.rootPath + "\\Website");

      // TODO: need to change bindings in right way(maybe with the UI dialog)
      // websiteSettings.SetElementAttributeValue("/appcmd/SITE/site/bindings/binding[@bindingInformation='*:80:" + args.oldSiteName + "']", "bindingInformation", "*:80:" + args.siteName);
      XmlElement bindingsElement = websiteSettings.SelectSingleElement("/appcmd/SITE/site/bindings");
      if (bindingsElement != null)
      {
        bindingsElement.InnerXml = string.Empty;

        // it's a fucking HACK, I can't work with xml nodes
        foreach (var key in args.bindings.Keys)
        {
          bindingsElement.InnerXml += "<binding protocol=\"http\" bindingInformation=\"*:{1}:{0}\" />".FormatWith(key, args.bindings[key].ToString());
        }

        // foreach (XmlElement bindingElement in bindingsElement.ChildNodes)
        // {

        // //if (bindingElement.Attributes["bindingInformation"].Value.Split(':').Last() != null && bindingElement.Attributes["bindingInformation"].Value.Split(':').Last() != "")
        // //    args.siteBindingsHostnames.Add(bindingElement.Attributes["bindingInformation"].Value.Split(':').Last());
        // //if(bindingElement.Attributes["bindingInformation"].Value.Split(':').Last() != null && bindingElement.Attributes["bindingInformation"].Value.Split(':').Last() != "")
        // //bindingElement.Attributes["bindingInformation"].Value = bindingElement.Attributes["bindingInformation"].Value.Replace(bindingElement.Attributes["bindingInformation"].Value.Split(':').Last(), args.siteName);
        // }
      }

      websiteSettings.Save(websiteSettings.FilePath + ".fixed.xml");
    }

    #endregion


  }
}