namespace SIM
{
  using System;
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;
  using System.Reflection;
  using SIM.Extensions;

  public static class ReflectionUtil
  {
    #region Public methods

    [NotNull]
    public static object CreateObject(Type type, params object[] objects)
    {
      Assert.ArgumentNotNull(type, nameof(type));

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
      Assert.ArgumentNotNullOrEmpty(typeName, nameof(typeName));

      var type = GetType(typeName);

      return CreateObject(type, objects);
    }

    [NotNull]
    public static Type GetType(string typeName)
    {
      Assert.ArgumentNotNullOrEmpty(typeName, nameof(typeName));

      var type = Type.GetType(typeName);
      Assert.IsNotNull(type, "The {0} type cannot be found in the assemblies".FormatWith(typeName));

      return type;
    }

    [NotNull]
    public static Type GetType(Assembly assembly, string typeName)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(typeName, nameof(typeName));

      Type type = assembly.GetType(typeName);
      Assert.IsNotNull(type,
        "The {0} type cannot be loaded in the {1} assembly.".FormatWith(typeName, assembly.FullName));

      return type;
    }

    /// <summary>
    /// Assembly is loaded from file stream to prevent
    /// maintaining a lock on DLL.
    /// </summary>
    [NotNull]
    public static Assembly GetAssembly(string dllPath)
    {
      Assert.ArgumentNotNullOrEmpty(dllPath, nameof(dllPath));

      FileStream stream = File.OpenRead(dllPath);

      Assert.IsNotNull(stream, nameof(stream));

      using (stream)
      {
        byte[] data = new byte[stream.Length];
        stream.Read(data, 0, data.Length);
        return Assembly.Load(data);
      }
    }

    [CanBeNull]
    public static object InvokeMethod(object obj, string method, params object[] parameters)
    {
      Assert.ArgumentNotNull(obj, nameof(obj));
      Assert.ArgumentNotNullOrEmpty(method, nameof(method));

      Type type = obj.GetType();

      MethodInfo methodInfo = type.GetMethod(method);
      Assert.IsNotNull(methodInfo,
        "The {0} method cannot be loaded from the {1} type.".FormatWith(method, type.FullName));

      return methodInfo.Invoke(obj, parameters);
    }

    #endregion
  }
}