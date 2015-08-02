using System.Web.UI;

namespace SIM.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.ServiceModel;
  using System.Xml;
  using System.Xml.Linq;
  using SIM.Adapters.SqlServer;
  using SIM.Instances;
  using SIM.Pipelines.InstallModules;
  using SIM.Pipelines.SitecoreWebservices;
  using SIM.Products;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using SIM.Adapters.WebServer;

  #region

  #endregion

  public static class ConfigurationActions
  {
    #region Properties

    private static Credentials Credentials
    {
      get
      {
        return new Credentials
        {
          UserName = "sitecore\\admin", 
          Password = "b"
        };
      }
    }

    #endregion

    #region Public Methods and Operators

    public static void ExecuteActions([NotNull] Instance instance, [NotNull] Product[] modules, [CanBeNull] List<Product> done, [NotNull] string param, [CanBeNull] SqlConnectionStringBuilder connectionString, 
      [CanBeNull] IPipelineController controller = null, [CanBeNull] Dictionary<string, string> variables = null)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(modules, "modules");
      Assert.ArgumentNotNull(param, "param");

      variables = variables ?? new Dictionary<string, string>();
      foreach (Product module in modules)
      {
        if (done != null && done.Contains(module))
        {
          continue;
        }

        XmlDocument manifest = module.Manifest;

        if (manifest == null)
        {
          Log.Warn("The {0} doesn't have a manifest", module);
          if (done != null)
          {
            done.Add(module);
          }

          continue;
        }

        Product instanceProduct = instance.Product;
        if (!module.IsMatchRequirements(instanceProduct))
        {
          Log.Warn("The {0} doesn't suites for {1}", module, instanceProduct);
          if (done != null)
          {
            done.Add(module);
          }

          continue;
        }

        // Get the rules xpath
        string[] paramArgs = param.Split('|');
        string xpath =
          paramArgs.Length == 2
            ? Product.ManifestPrefix + paramArgs[0] + "/install/" + paramArgs[1] // is package
            : Product.ManifestPrefix + param + "/install"; // is archive

        XmlElement element = manifest.SelectSingleNode(xpath) as XmlElement;
        if (element == null)
        {
          Log.Warn("Can't find rules root (the {0} element in the manifest of the {3} file){1}The manifest is: {1}{2}", xpath, Environment.NewLine, manifest.OuterXml, module.PackagePath);
          if (done != null)
          {
            done.Add(module);
          }

          continue;
        }

        var sections = element.ChildNodes.OfType<XmlElement>().NotNull();
        FillVariables(sections, variables, instance, controller);
        var actions = sections.SingleOrDefault(s => s.Name.EqualsIgnoreCase("actions"));
        ProcessActions(instance, connectionString, controller, module, variables, actions);

        if (done != null)
        {
          done.Add(module);
        }
      }
    }

    public static Database GetMainDatabase(Instance instance, SqlConnectionStringBuilder connectionString)
    {
      using (new ProfileSection("Get main database"))
      {
        ProfileSection.Argument("instance", instance);
        ProfileSection.Argument("connectionString", connectionString);

        string sqlServerInstanceName = connectionString.DataSource;
        Database mainDatabase = null;
        string[] firstOrderDatabaseNames = new[]
        {
          "core", "master", "web"
        };
        var localDBs =
          instance.AttachedDatabases.Where(
            d => d.ConnectionString.DataSource == sqlServerInstanceName).ToArray();
        Log.Debug("localDbs.Length: {0}",  localDBs.Length);

        foreach (string databaseName in firstOrderDatabaseNames)
        {
          mainDatabase = localDBs.SingleOrDefault(d => d.Name == databaseName);
          Log.Debug("databaseName: {0}",  databaseName);
          Log.Debug("mainDatabase!=null: {0}",  mainDatabase != null);
          if (mainDatabase != null)
          {
            return ProfileSection.Result(mainDatabase);
          }
        }

        mainDatabase = localDBs.FirstOrDefault();

        return ProfileSection.Result(mainDatabase);
      }
    }

    public static IEnumerable<string> GetPlaceholderNames(Instance instance)
    {
      Credentials credentials = Credentials;
      EndpointAddress remoteAddress = new EndpointAddress(GetWebServiceUrl(instance));
      var client = GetWebServiceClient(remoteAddress);
      var doc = GetPlaceholders(credentials, client);
      return GetItems(doc).Select(node => node.InnerText);
    }

    #endregion

    #region Methods

    private static void AddDatabase([NotNull] Instance instance, [NotNull] IEnumerable<XmlElement> databases, [NotNull] Product module, SqlConnectionStringBuilder connectionString, IPipelineController controller)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(databases, "databases");
      Assert.ArgumentNotNull(module, "module");
      Assert.ArgumentNotNull(connectionString, "connectionString");
      foreach (XmlElement database in databases)
      {
        string name = database.GetAttribute("name");
        string role = database.GetAttribute("role").EmptyToNull() ?? name;
        string fileName = database.GetAttribute("fileName");
        string sourceFileName = database.GetAttribute("sourceFileName");
        string databasesFolder = GetDatabasesFolder(instance, connectionString, controller);
        var locationInPackage = database.GetAttribute("location");
        Assert.IsNotNull(databasesFolder, "databasesFolder");
        FileSystem.FileSystem.Local.Directory.AssertExists(databasesFolder);

        bool skipAttach = false;
        string realDBname = SqlServerManager.Instance.GenerateDatabaseRealName(instance.Name, role);
        string physicalPath = Path.Combine(databasesFolder, fileName);

        var newDatabaseConnectionString = new SqlConnectionStringBuilder(connectionString.ToString());
        newDatabaseConnectionString.InitialCatalog = realDBname;
        if (SqlServerManager.Instance.DatabaseExists(realDBname, newDatabaseConnectionString))
        {
          string databasePath = SqlServerManager.Instance.GetDatabaseFileName(realDBname, newDatabaseConnectionString);
          const string theDatabaseExists = "The database with the same name ('{0}') already exists in the current instance of SQL Server ('{1}')";
          if (string.IsNullOrEmpty(databasePath))
          {
            string message = string.Format(theDatabaseExists + ", but doesn't point to any file(s) and looks like corrupted.", realDBname, newDatabaseConnectionString.DataSource);
            if (!controller.Confirm(message + "  Would you like to delete it? If not then this installation will be interrupted."))
            {
              throw new InvalidOperationException(message);
            }

            SqlServerManager.Instance.DeleteDatabase(realDBname, newDatabaseConnectionString);
          }
          else if (!databasePath.EqualsIgnoreCase(physicalPath))
          {
            throw new InvalidOperationException(string.Format(theDatabaseExists + ", but points to files by another location ('{2}') than was expected ('{3}')", realDBname, newDatabaseConnectionString.DataSource, databasePath, physicalPath));
          }

          skipAttach = true;
        }

        if (!skipAttach)
        {
          ExtractDatabase(module, fileName, sourceFileName, databasesFolder, locationInPackage);
          SqlServerManager.Instance.AttachDatabase(realDBname, physicalPath, newDatabaseConnectionString);
        }

        instance.Configuration.ConnectionStrings.Add(name, newDatabaseConnectionString);
      }
    }

    private static void AddFormsToPlaceholder(string name, string url)
    {
      Credentials credentials = Credentials;
      Assert.IsNotNull(credentials, "credentials");

      EndpointAddress remoteAddress = new EndpointAddress(url);
      var client = GetWebServiceClient(remoteAddress);
      Assert.IsNotNull(client, "client");

      var doc = GetPlaceholders(credentials, client);
      Assert.IsNotNull(doc, "doc");

      var items = GetItems(doc);
      Assert.IsNotNull(items, "items");

      var firstItem = items.FirstOrDefault(node => node.InnerText.Equals(name));
      Assert.IsNotNull(firstItem, "firstItem");

      var placeholderId = firstItem.GetAttribute("id");
      Assert.IsNotNull(placeholderId, "placeholderId");

      XElement fields = client.GetItemFields(placeholderId, "en", "1", true, "master", credentials);
      Assert.IsNotNull(fields, "fields");

      XmlDocument xmlDocument = GetXmlDocument(fields);
      Assert.IsNotNull(xmlDocument, "xmlDocument");

      XmlNode field = xmlDocument.SelectSingleNode("/sitecore/field[@name='Allowed Controls']");
      Assert.IsNotNull(fields, "field");

      XmlElement fieldValueElement = field.ChildNodes.OfType<XmlElement>().FirstOrDefault();
      Assert.IsNotNull(fieldValueElement, "fieldValueElement");

      string value = fieldValueElement.InnerXml;
      Assert.IsNotNull(value, "value");

      const string formsRenderingID = "|{6D3B4E7D-FEF8-4110-804A-B56605688830}";
      value += formsRenderingID;
      fieldValueElement.InnerXml = value.TrimStart('|');

      string xml = xmlDocument.OuterXml;
      Assert.IsNotNull(xml, "xml");

      client.Save(xml, "master", credentials);
    }

    private static void AddSiteBinding(string instanceName, XmlElement action)
    {
      Assert.ArgumentNotNullOrEmpty(instanceName, "instanceName");
      Assert.ArgumentNotNull(action, "action");

      var host = action.GetAttribute("host");
      if (host.IsNullOrEmpty())
      {
        return;
      }

      var protocol = action.HasAttribute("protocol") ? action.GetAttribute("protocol") : "http";
      int port;

      if (!action.HasAttribute("port") || !int.TryParse(action.GetAttribute("port"), out port))
      {
        port = 80;
      }

      var ip = action.HasAttribute("ip") ? action.GetAttribute("ip") : "*";

      WebServerManager.AddHostBinding(instanceName, new BindingInfo(protocol, host, port, ip));
    }

    private static string ChangeWebRootToActual(string path, string webRootName)
    {
      if (path.StartsWith("/Website", true, CultureInfo.InvariantCulture) || path.StartsWith(@"\Website", true, CultureInfo.InvariantCulture))
      {
        return string.Format(@"/{0}{1}", webRootName, path.Substring(8));
      }

      if (path.StartsWith("Website", true, CultureInfo.InvariantCulture))
      {
        return string.Format(@"/{0}{1}", webRootName, path.Substring(7));
      }

      return path;
    }

    private static void EditFile(string virtualPath, IEnumerable<XmlElement> children, Instance instance, Dictionary<string, string> variables)
    {
      string instanceRootPath = instance.RootPath;

      var filePath = ChangeWebRootToActual(virtualPath, new DirectoryInfo(instance.WebRootPath).Name);
      var path = Path.Combine(instanceRootPath, filePath.TrimStart('/'));
      foreach (var child in children)
      {
        var name = child.Name.ToLower();
        switch (name)
        {
          case "replace":
          {
            var source = child.GetAttribute("source");
            var target = child.GetAttribute("target");
            FileSystem.FileSystem.Local.File.WriteAllText(path, FileSystem.FileSystem.Local.File.ReadAllText(path).Replace(source, target));
            break;
          }

          case "replacevariables":
          {
            var text = FileSystem.FileSystem.Local.File.ReadAllText(path);
            foreach (var variable in variables)
            {
              text = text.Replace(variable.Key, variable.Value);
            }

            text = text.Replace("{InstanceName}", instance.Name);
            FileSystem.FileSystem.Local.File.WriteAllText(path, text);
            break;
          }

          case "write":
            {                                                            
              var target = child.InnerXml;
              FileSystem.FileSystem.Local.File.WriteAllText(path, XmlDocumentEx.LoadXml(target).ToPrettyXmlString());
              break;
            }

          case "append":
          {
            var before = child.GetAttribute("before");
            var target = child.InnerXml;
            FileSystem.FileSystem.Local.File.WriteAllText(path, FileSystem.FileSystem.Local.File.ReadAllText(path).Replace(before, target + before));
            break;
          }

          case "move":
          {
            var target = child.GetAttribute("target");
            string destFileName = Path.Combine(instanceRootPath, target.TrimStart('/'));
            FileSystem.FileSystem.Local.Directory.DeleteIfExists(destFileName);
            FileSystem.FileSystem.Local.File.Move(path, destFileName);
            break;
          }

          case "copy":
          {
            var target = child.GetAttribute("target");
            string destFileName = Path.Combine(instanceRootPath, target.TrimStart('/'));
            FileSystem.FileSystem.Local.Directory.DeleteIfExists(destFileName);
            FileSystem.FileSystem.Local.File.Copy(path, destFileName);
            break;
          }
        }
      }
    }

    private static void ExtractDatabase(Product module, string fileName, string sourceFileName, string databasesFolder, string locationInPackage)
    {
      var packagePath = module.PackagePath;
      if (!string.IsNullOrEmpty(locationInPackage))
      {
        using (var tmp = FileSystem.FileSystem.Local.Directory.GetTempFolder())
        {
          var tmpPath = tmp.Path;
          ExtractDatabase(packagePath, fileName, sourceFileName, databasesFolder, locationInPackage, tmpPath);
        }
      }
      else
      {
        ExtractDatabase(packagePath, fileName, sourceFileName, databasesFolder, null, null);
      }
    }

    private static void ExtractDatabase(string packagePath, string fileName, string sourceFileName, string databasesFolder, string locationInPackage, string tmpPath)
    {
      ExtractDatabaseInner(fileName, sourceFileName, databasesFolder, packagePath, tmpPath, locationInPackage);

      fileName = Path.ChangeExtension(fileName, ".ldf");
      sourceFileName = Path.ChangeExtension(sourceFileName, ".ldf");
      ExtractDatabaseInner(fileName, sourceFileName, databasesFolder, packagePath, tmpPath, locationInPackage);
    }

    private static void ExtractDatabaseInner(string fileName, [CanBeNull] string sourceFileName, string databasesFolder, string packagePath, string tmpPath = null, string location = null)
    {
      Assert.ArgumentNotNullOrEmpty(fileName, "fileName");
      Assert.ArgumentNotNullOrEmpty(databasesFolder, "databasesFolder");
      Assert.ArgumentNotNullOrEmpty(packagePath, "packagePath");
      Assert.IsTrue(string.IsNullOrEmpty(location) == string.IsNullOrEmpty(tmpPath), "tmpPath and location must be set or null at the same time");

      const string packageZipFileName = "package.zip";
      if (!string.IsNullOrEmpty(location) && location.StartsWith(packageZipFileName, StringComparison.OrdinalIgnoreCase))
      {
        FileSystem.FileSystem.Local.Zip.UnpackZip(packagePath, tmpPath, packageZipFileName);
        packagePath = Path.Combine(tmpPath, packageZipFileName);
        location = location.Substring(packageZipFileName.Length).Trim('\\', '/');

        try
        {
          var inPackageFilePath = location + "/" + (sourceFileName.EmptyToNull() ?? fileName);
          FileSystem.FileSystem.Local.Zip.UnpackZip(packagePath, tmpPath, inPackageFilePath.Replace("\\", "/"));

          var source = Path.Combine(tmpPath, inPackageFilePath);
          var target = Path.Combine(databasesFolder, Path.GetFileName(inPackageFilePath));
          if (FileSystem.FileSystem.Local.File.Exists(source))
          {
            if (FileSystem.FileSystem.Local.File.Exists(target))
            {
              FileSystem.FileSystem.Local.File.Delete(target);
            }

            FileSystem.FileSystem.Local.File.Move(source, target);
            return;
          }
        }
        catch (IOException e)
        {
          throw new InvalidOperationException(string.Format("The installer can't extract the '{0}' file into the '{1}' folder. Check if there is already such a file or if the process has access rights", fileName, databasesFolder), e);
        }
      }

      // TODO: Ask user what he want if error
      using (var tempFolder = FileSystem.FileSystem.Local.Directory.GetTempFolder())
      {
        try
        {
          if (string.IsNullOrEmpty(sourceFileName))
          {
            FileSystem.FileSystem.Local.Zip.UnpackZip(packagePath, databasesFolder, fileName);
          }
          else
          {
            FileSystem.FileSystem.Local.Zip.UnpackZip(packagePath, tempFolder.Path, sourceFileName);
          }
        }
        catch (IOException e)
        {
          throw new InvalidOperationException(string.Format("The installer can't extract the '{0}' file into the '{1}' folder. Check if there is already such a file or if the process has access rights", fileName, databasesFolder), e);
        }

        if (!string.IsNullOrEmpty(sourceFileName))
        {
          var destFilePath = Path.Combine(databasesFolder, fileName);
          var sourceFilePath = Path.Combine(tempFolder.Path, sourceFileName);
          FileSystem.FileSystem.Local.File.Move(sourceFilePath, destFilePath);
        }
      }
    }

    private static void FillVariables(IEnumerable<XmlElement> sections, Dictionary<string, string> variables, Instance instance, IPipelineController controller)
    {
      var @params = sections.SingleOrDefault(s => s.Name.EqualsIgnoreCase("params"));
      if (@params != null)
      {
        var children = @params.ChildNodes.OfType<XmlElement>();
        foreach (XmlElement param in children)
        {
          string variableName = param.GetAttribute("name");
          if (variables.ContainsKey(variableName))
          {
            continue;
          }

          string variableTitle = param.GetAttribute("title");
          string defaultVariableValue = param.GetAttribute("defaultValue");
          string mode = param.GetAttribute("mode");
          var options = param.GetAttribute("options");
          var typeName = param.GetAttribute("getOptionsType");
          var methodName = param.GetAttribute("getOptionsMethod");
          var multiselect = mode.EqualsIgnoreCase("multiselect");
          string variableValue = controller != null
            ? (multiselect || mode.Equals("select")
              ? controller.Select(variableTitle, !string.IsNullOrEmpty(options)
                ? options.Split('|')
                : GetOptions(typeName, methodName, instance), multiselect, defaultVariableValue)
              : controller.Ask(variableTitle, defaultVariableValue))
            : defaultVariableValue;
          variables.Add(variableName, variableValue);
        }
      }
    }

    private static string GetDatabasesFolder(Instance instance, SqlConnectionStringBuilder connectionString, IPipelineController controller)
    {
      string databasesFolder;
      Database mainDatabase = GetMainDatabase(instance, connectionString);

      if (mainDatabase == null)
      {
        while (true)
        {
          databasesFolder = controller.Ask(
            "Can't find any local database of the " + instance +
            " instance to detect the Databases folder. Please specify it manually:", 
            instance.RootPath.TrimEnd('\\') + "\\Databases");
          if (string.IsNullOrEmpty(databasesFolder))
          {
            if (controller.Confirm("You didn't input anything - would you like to terminate this installation?"))
            {
              throw new Exception("Aborted.");
            }

            continue;
          }

          if (!FileSystem.FileSystem.Local.Directory.Exists(databasesFolder))
          {
            if (controller.Confirm("The " + databasesFolder + " doesn't exist. Would you like to create the folder?"))
            {
              FileSystem.FileSystem.Local.Directory.CreateDirectory(databasesFolder);
              break;
            }
          }
          else
          {
            break;
          }
        }
      }
      else
      {
        databasesFolder = Path.GetDirectoryName(mainDatabase.FileName).EmptyToNull();
      }

      return databasesFolder;
    }

    private static IEnumerable<XmlElement> GetItems(XmlDocument doc)
    {
      return doc.SelectNodes("/sitecore/item").OfType<XmlElement>();
    }

    private static IEnumerable<string> GetOptions(string typeName, string methodName, Instance instance)
    {
      var type = Type.GetType(typeName);
      var method = type.GetMethod(methodName);
      return (IEnumerable<string>)method.Invoke(null, new object[]
      {
        instance
      });
    }

    private static XmlDocument GetPlaceholders(Credentials credentials, VisualSitecoreServiceSoapClient client)
    {
      return GetXmlDocument(client.GetChildren("{1CE3B36C-9B0C-4EB5-A996-BFCB4EAA5287}", "master", credentials));
    }

    private static VisualSitecoreServiceSoapClient GetWebServiceClient(EndpointAddress remoteAddress)
    {
      return new VisualSitecoreServiceSoapClient(new BasicHttpBinding(), remoteAddress);
    }

    private static string GetWebServiceUrl(Instance instance)
    {
      return instance.GetUrl("/sitecore/shell/webservice/service.asmx");
    }

    private static XmlDocument GetXmlDocument(XElement fields)
    {
      var xmlDocument = new XmlDocument();
      using (XmlReader xmlReader = fields.CreateReader())
      {
        xmlDocument.Load(xmlReader);
      }

      return xmlDocument;
    }

    private static void PerformConfigChanges([NotNull] Instance instance, [NotNull] IEnumerable<XmlElement> instructions, [NotNull] Product module, [NotNull] XmlDocumentEx config, [NotNull] Dictionary<string, string> variables)
    {
      Assert.ArgumentNotNull(instance, "instance");
      Assert.ArgumentNotNull(instructions, "instructions");
      Assert.ArgumentNotNull(module, "module");
      Assert.ArgumentNotNull(config, "config");
      Assert.ArgumentNotNull(variables, "variables");

      foreach (XmlElement instruction in instructions)
      {
        string name = instruction.Name.ToLower();

        switch (name)
        {
          case "append":
          {
            string xpath = instruction.GetAttribute("xpath");
            Assert.IsNotNull(xpath.EmptyToNull(), "xpath");
            XmlNode parentNode = config.SelectSingleNode(xpath);
            if (parentNode == null)
            {
              Log.Warn("[InstallActions, Append] The {0} element isn't found", xpath);
              break;
            }

            parentNode.InnerXml += instruction.InnerXml;
            break;
          }

          case "include":
          {
            string filePath = instruction.GetAttribute("path");
            FileSystem.FileSystem.Local.Zip.UnpackZip(module.PackagePath, Path.Combine(instance.WebRootPath, "app_config\\include"), filePath);
            break;
          }

          case "change":
          {
            string xpath = instruction.GetAttribute("xpath");
            if (string.IsNullOrEmpty(xpath))
            {
              Log.Warn("The xpath attribute is missing in the {0} instruction (outer xml: {1})", instruction.Name, instruction.OuterXml);
              continue;
            }

            XmlElement targetElement = (XmlElement)config.SelectSingleNode(xpath);
            if (targetElement == null)
            {
              Log.Warn("Can't find the {0} element in the {1} file", xpath, config.FilePath);
              continue;
            }

            foreach (XmlElement element in instruction.ChildNodes.OfType<XmlElement>())
            {
              switch (element.Name.ToLower())
              {
                case "attribute":
                {
                  string attributeName = element.GetAttribute("name");
                  string attributeValue = element.GetAttribute("value");
                  targetElement.SetAttribute(attributeName, attributeValue);
                  break;
                }
              }
            }

            break;
          }

          case "delete":
          {
            string xpath = instruction.GetAttribute("xpath");
            if (string.IsNullOrEmpty(xpath))
            {
              Log.Warn("The xpath attribute is missing in the {0} instruction (outer xml: {1})", instruction.Name, instruction.OuterXml);
              continue;
            }

            XmlNodeList nodes = config.SelectNodes(xpath);
            if (nodes == null || nodes.Count == 0)
            {
              Log.Warn("Can't find the {0} nodes in the {1} file", xpath, config.FilePath);
              continue;
            }

            foreach (XmlElement targetElement in nodes.OfType<XmlElement>())
            {
              XmlNode parent = targetElement.ParentNode;
              if (parent == null)
              {
                Log.Warn("Can't find the parent node of the {0} element of the {1} file", xpath, config.FilePath);
                continue;
              }

              parent.RemoveChild(targetElement);
              break;
            }

            break;
          }
                    
          case "disable":
          {
            string fromFileName = instruction.GetAttribute("path");
            if (!string.IsNullOrEmpty(fromFileName))
            {
              var includeFolderPath = Path.Combine(instance.WebRootPath, "app_config\\include");
              var fromPath = Path.Combine(includeFolderPath, fromFileName);
              if (!File.Exists(fromPath))
              {
                Log.Warn("The {0} file not found", fromPath);

                break;
              }

              var toPath = fromPath + ".disabled";
              RenameFile(fromPath, toPath, true);
            }

            break;
          }

          case "enable":
          {
            string fromFileName = instruction.GetAttribute("path");
            if (!string.IsNullOrEmpty(fromFileName))
            {
              var includeFolderPath = Path.Combine(instance.WebRootPath, "app_config\\include");
              var fromPath = Path.Combine(includeFolderPath, fromFileName);
              if (!File.Exists(fromPath))
              {
                Log.Warn("The {0} file not found", fromPath);

                break;
              }

              var configPostfix = ".config";
              if (Path.GetExtension(fromPath).EqualsIgnoreCase(configPostfix))
              {
                break;
              }

              var fileName = Path.GetFileName(fromPath);
              var dir = Path.GetDirectoryName(fromPath);
              var index = fileName.LastIndexOf(configPostfix, StringComparison.OrdinalIgnoreCase);
              if (index > 0)
              {
                fileName = fileName.Substring(0, index + configPostfix.Length);
                RenameFile(fromPath, Path.Combine(dir, fileName), true);
                break;
              }

              foreach (var postfix in new[] { ".example", ".sample", ".disabled", ".remove", ".delete", ".ignore" })
              {
                var index2 = fileName.LastIndexOf(postfix, StringComparison.OrdinalIgnoreCase);
                if (index2 <= 0)
                {
                  continue;
                }

                fileName = fileName.Substring(0, index2 + postfix.Length);
                RenameFile(fromPath, Path.Combine(dir, fileName), true);
                break;
              }

              fileName += ".config";
              RenameFile(fromPath, Path.Combine(dir, fileName), true);
            }

            break;
          }

          case "rename":
          {
            var skipOnErrorText = instruction.GetAttribute("skipOnError");
            var skipOnError = skipOnErrorText.EqualsIgnoreCase("true");
            var fromFileName = instruction.GetAttribute("from");
            var toFileName = instruction.GetAttribute("to");

            if (!string.IsNullOrEmpty(fromFileName) && !string.IsNullOrEmpty(toFileName))
            {
              var includeFolderPath = Path.Combine(instance.WebRootPath, "app_config\\include");
              var fromPath = Path.Combine(includeFolderPath, fromFileName);
              var toPath = Path.Combine(includeFolderPath, toFileName);

              RenameFile(fromPath, toPath, skipOnError);
            }

            break;
          }
        }
      }

      config.Save();
    }

    private static void RenameFile(string fromPath, string toPath, bool skipOnError)
    {
      try
      {
        if (!FileSystem.FileSystem.Local.File.Exists(fromPath) && FileSystem.FileSystem.Local.File.Exists(toPath))
        {
          Log.Warn("The moving does not seem to be needed");
          return;
        }

        FileSystem.FileSystem.Local.File.Move(fromPath, toPath);
      }
      catch (Exception ex)
      {
        if (!skipOnError)
        {
          throw;
        }

        Log.Error(ex, "Cannot rename file {0} to {1}", fromPath, toPath);
      }
    }

    private static void ProcessActions(Instance instance, SqlConnectionStringBuilder connectionString, 
      IPipelineController controller, Product module, Dictionary<string, string> variables, 
      XmlElement actionsElement)
    {
      // made replacement
      actionsElement.InnerXml = variables.Aggregate(actionsElement.InnerXml, 
        (result, variable) => result.Replace(variable.Key, variable.Value))
        .Replace("{InstanceName}", instance.Name)
        .Replace("{InstanceHost}", instance.HostNames.First());

      var actions = actionsElement.ChildNodes.OfType<XmlElement>();
      var conditionEvaluator = new ConditionEvaluator(variables);
      actions = actions.Where(a => conditionEvaluator.ConditionIsTrueOrMissing(a));

      var webRootPath = instance.WebRootPath;
      List<string> ignoreCommands = new List<string>();
      foreach (XmlElement action in actions.Where(a => a.Name.EqualsIgnoreCase("patch")))
      {
        var commandText = action.GetAttribute("command").EmptyToNull().IsNotNull("The command attribute of <patch /> element must exist and must not be empty");
        var actionText = action.GetAttribute("action");
        if (actionText == "delete")
        {
          ignoreCommands.Add(commandText);
        }
      }

      // give extract more priority
      if (actions.Any(xml => xml.Name.EqualsIgnoreCase("extract")) && !ignoreCommands.Contains("extract"))
      {
        FileSystem.FileSystem.Local.Zip.UnpackZip(module.PackagePath, instance.WebRootPath);
      }

      foreach (XmlElement action in actions)
      {
        var children = action.ChildNodes.OfType<XmlElement>();
        string actionName = action.Name;
        if (ignoreCommands.Contains(actionName))
        {
          continue;
        }

        switch (actionName)
        {
          case "extract":
          {
            // give extract more priority
            // FileSystem.Instance.UnpackZip(module.PackagePath, instance.GetRootPath(webRootPath));
            break;
          }

          case "addSiteBinding":
          {
            AddSiteBinding(instance.Name, action);
           
            break;
          }

          case "addHostName":
          {
            Hosts.Append(action.GetAttribute("hostName"));

            break;
          }

          case "config":
          {
            string configPath = action.GetAttribute("path");
            try
            {
              XmlDocumentEx config = !string.IsNullOrEmpty(configPath)
                ? XmlDocumentEx.LoadFile(Path.Combine(webRootPath, configPath))
                : instance.GetWebConfig(webRootPath);
              PerformConfigChanges(instance, children, module, config, variables);
            }
            catch (XmlDocumentEx.FileIsMissingException ex)
            {
              Log.Warn(ex, "The path attribute is specified (path: {0}) but the file doesn't exist", configPath);
            }

            break;
          }

          case "databases":
          {
            AddDatabase(instance, children, module, connectionString, controller);
            break;
          }

          case "editfile":
          {
            EditFile(action.GetAttribute("path"), children, instance, variables);
            break;
          }

          case "deployfile":
          {
            DeployFile(action.GetAttribute("path"), action.GetAttribute("target"), instance);
            break;
          }


          case "setRestrictingPlaceholders":
          {
            InstanceHelper.StartInstance(instance);
            SetRestrictingPlaceholders(action.GetAttribute("names"), GetWebServiceUrl(instance));
            break;
          }

          case "custom":
          {
            var typeName = action.GetAttribute("type").EmptyToNull().IsNotNull("The type attribute is missing in the <custom> install action");
            var obj = (IPackageInstallActions)ReflectionUtil.CreateObject(typeName);
            obj.Execute(instance, module);
            break;
          }

          case "sql":
          {
            var db = action.GetAttribute("database");
            var file = action.GetAttribute("file").Replace("$(data)", instance.DataFolderPath).Replace("$(website)", instance.WebRootPath);
            if (!string.IsNullOrEmpty(file))
            {
              Assert.IsTrue(File.Exists(file), string.Format("The {0} file does not exist", file));
            }

            var sql = string.IsNullOrEmpty(file) ? action.InnerText : FileSystem.FileSystem.Local.File.ReadAllText(file);
            Assert.IsNotNullOrEmpty(sql.Trim(), "The SQL command is empty");

            var cstr = instance.Configuration.ConnectionStrings.FirstOrDefault(x => x.Name == db);
            Assert.IsNotNull(cstr, "The {0} connection string is not found".FormatWith(db));
            using (var conn = SqlServerManager.Instance.OpenConnection(new SqlConnectionStringBuilder(cstr.Value), false))
            {
              foreach (var command in sql.Split("GO"))
              {
                SqlServerManager.Instance.Execute(conn, command);
              }
            }

            break;
          }
        }
      }
    }

    private static void DeployFile(string path, string target, Instance instance)
    {
      string instanceRootPath = instance.RootPath;

      var sourcePath = Path.Combine(ApplicationManager.TempFolder, path.TrimStart("/"));
      var targetPath = Path.Combine(instanceRootPath, target.TrimStart("/"));

      File.Copy(sourcePath, targetPath);

    }

    private static void SetRestrictingPlaceholders(string names, string url)
    {
      foreach (var name in names.Split(','))
      {
        AddFormsToPlaceholder(name, url);
      }
    }

    #endregion
  }
}