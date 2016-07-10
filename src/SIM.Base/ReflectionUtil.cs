namespace SIM
{
  using System;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;
  using System.Reflection;

  public static class ReflectionUtil
  {
    #region Public methods

    [NotNull]
    public static object CreateObject(Type type, params object[] objects)
    {
      Assert.ArgumentNotNull(type, "type");

      using (new ProfileSection("Create object"))
      {
        ProfileSection.Argument("type", type);
        ProfileSection.Argument("objects", objects);

        return ProfileSection.Result(Activator.CreateInstance(type, objects));
      }
    }

    [NotNull]
    public static object CreateObject(string typeName, params object[] objects)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");

      var type = GetType(typeName);

      return CreateObject(type, objects);
    }

    [NotNull]
    public static Type GetType(string typeName)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, "typeName");

      var type = Type.GetType(typeName);
      Assert.IsNotNull(type, "The {0} type cannot be found in the assemblies".FormatWith(typeName));

      return type;
    }

    [NotNull]
    public static Type GetType(Assembly assembly, string typeName)
    {
      Assert.ArgumentNotNull(assembly, "assembly");
      Assert.ArgumentNotNull(typeName, "typeName");

      Type type = assembly.GetType(typeName);
      Assert.IsNotNull(type,
        "The {0} type cannot be loaded in the {1} assembly.".FormatWith(typeName, assembly.FullName));

      return type;
    }

    [NotNull]
    public static Assembly GetAssembly(string dllPath)
    {
      Assert.ArgumentNotNullOrEmpty(dllPath, "dllPath");

      Assembly assembly = Assembly.LoadFrom(dllPath);
      Assert.IsNotNull(assembly, "The assembly cannot be loaded from the path '{0}'.".FormatWith(dllPath));

      return assembly;
    }

    [CanBeNull]
    public static object InvokeMethod(object obj, string method, params object[] parameters)
    {
      Assert.ArgumentNotNull(obj, "obj");
      Assert.ArgumentNotNullOrEmpty(method, "method");

      Type type = obj.GetType();

      MethodInfo methodInfo = type.GetMethod(method);
      Assert.IsNotNull(methodInfo,
        "The {0} method cannot be loaded from the {1} type.".FormatWith(method, type.FullName));

      return methodInfo.Invoke(obj, parameters);
    }

    #endregion
  }
}