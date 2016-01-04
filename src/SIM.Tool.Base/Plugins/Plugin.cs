namespace SIM.Tool.Base.Plugins
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Windows.Media;
  using System.Xml;
  using Sitecore.Diagnostics.Base;

  public class Plugin
  {
    #region Constants

    private const string PluginXmlFileName = "Plugin.xml";

    #endregion

    #region Fields

    public readonly IEnumerable<string> AssemblyFilePaths;
    public readonly string PluginFilePath;
    public readonly string PluginFolder;
    public readonly XmlDocumentEx PluginXmlDocument;

    #endregion

    #region Constructors

    private Plugin(string pluginFilePath)
    {
      this.PluginFilePath = pluginFilePath;
      this.PluginFolder = Path.GetDirectoryName(pluginFilePath);
      this.AssemblyFilePaths = FileSystem.FileSystem.Local.File.GetNeighbourFiles(pluginFilePath, "*.dll");
      this.PluginXmlDocument = XmlDocumentEx.LoadFile(pluginFilePath);
    }

    #endregion

    #region Public Methods and Operators

    public static object CreateInstance(XmlElement element)
    {
      var typeFullName = element.GetAttribute("type");
      if (string.IsNullOrEmpty(typeFullName))
      {
        return null;
      }

      var param = element.GetAttribute("param");
      return CreateInstance(typeFullName, element.Name, param.EmptyToNull());
    }

    public static object CreateInstance(string typeFullName, string reference = null, string param = null)
    {
      var type = GetType(typeFullName, reference);

      if (!string.IsNullOrEmpty(param))
      {
        return ReflectionUtil.CreateObject(type, param);
      }

      return ReflectionUtil.CreateObject(type);
    }

    public static Plugin Detect(string folder)
    {
      var path = Path.Combine(folder, PluginXmlFileName);
      return FileSystem.FileSystem.Local.File.Exists(path) ? new Plugin(path) : null;
    }

    public static ImageSource GetImage(string imageSource, string pluginFilePath)
    {
      var arr = imageSource.Split(',');
      Assert.IsTrue(arr.Length == 2, "The {0} file contains incorrect image source format \"{1}\" when the correct one is \"ImageFilePath, AssemblyName\"".FormatWith(pluginFilePath, imageSource));
      return WindowHelper.GetImage(arr[0], arr[1]);
    }

    public static Type GetType(string typeFullName, string reference = null)
    {
      // locate type within already loaded assemblies
      Type type = Type.GetType(typeFullName);

      // type was not found so we need to load necessary assemlby
      if (type == null)
      {
        var arr = typeFullName.Split(',');
        Assert.IsTrue(arr.Length <= 2, 
          string.IsNullOrEmpty(reference) ? "Wrong type identifier (no comma), must be like Namespace.Type, Assembly" : "The type attribute value of the <{0}> element has wrong format".FormatWith(reference));

        // format: type, assembly
        var typeName = arr[0].Trim();
        var assemblyName = arr[1].Trim();

        // find the assembly file by specified assembly name
        var assembly =
          AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(ass => ass.FullName.StartsWith(assemblyName + ","));
        Assert.IsNotNull(assembly, "Cannot find the {0} assembly".FormatWith(assemblyName));

        type = assembly.GetType(typeName);
        Assert.IsNotNull(type, "Cannot locate the {0} type within the {1} assembly".FormatWith(typeFullName, assemblyName));
      }

      return type;
    }

    public override bool Equals(object obj)
    {
      var plugin = obj as Plugin;
      return plugin != null ? this.PluginFolder.EqualsIgnoreCase(plugin.PluginFolder) : base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return this.PluginFolder.GetHashCode();
    }

    public ImageSource GetImage(string imageSource)
    {
      return GetImage(imageSource, this.PluginFilePath);
    }

    public void Load()
    {
      foreach (var path in this.AssemblyFilePaths)
      {
        var assemblyPath = path.Length > 2 && path[1] == ':' ? path : Path.Combine(Environment.CurrentDirectory, path);
        try
        {
          Assembly.LoadFile(assemblyPath);
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("There was a problem for loading the {0} assembly of the {1} plugin".FormatWith(assemblyPath, this.PluginFolder), true, ex);
        }
      }

      /*
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
      {
        var file = AssemblyFilePaths.FirstOrDefault(fi => Path.GetFileNameWithoutExtension(fi) == args.Name);
        return file.With(Assembly.Load);
      };*/
    }

    public override string ToString()
    {
      return this.PluginFolder;
    }

    #endregion
  }
}