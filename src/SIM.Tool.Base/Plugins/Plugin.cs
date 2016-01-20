namespace SIM.Tool.Base.Plugins
{
  using System;
  using System.Linq;
  using System.Windows.Media;
  using System.Xml;
  using Sitecore.Diagnostics.Base;

  public static class Plugin
  {
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

    #endregion
  }
}