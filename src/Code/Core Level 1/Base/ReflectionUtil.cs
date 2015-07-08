using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace SIM.Base
{
  public static class ReflectionUtil
  {
    [NotNull]
    public static object CreateObject(Type type, params object[] objects)
    {
      Assert.ArgumentNotNull(type, "type");

      using (new ProfileSection("Create object", typeof(ReflectionUtil)))
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
  }
}
