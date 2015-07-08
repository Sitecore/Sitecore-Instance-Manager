namespace SIM.Tool.Base
{
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.IO;
  using System.Linq;
  using System.Runtime.Serialization.Json;
  using SIM.Base;

  //public static class FavoriteManager
  //{
  //  [NotNull]
  //  private static readonly string FilePath = Path.Combine(ApplicationManager.DataFolder, "favorites.xml");

  //  [NotNull]
  //  private static readonly IList<string> List = Initialize();

  //  [NotNull]
  //  private static IList<string> Initialize()
  //  {
  //    ApplicationManager.AttemptToClose += ApplicationManagerOnAttemptToClose;
  //    if (!FileSystem.Local.File.Exists(FilePath))
  //    {
  //      return new List<string>();
  //    }

  //    var ser = new DataContractJsonSerializer(typeof(List<string>));
  //    using (var fileStream = File.OpenRead(FilePath))
  //    {
  //      var arr = (List<string>)ser.ReadObject(fileStream);
  //      return arr == null ? new List<string>() : arr.ToList();
  //    }
  //  }

  //  private static void ApplicationManagerOnAttemptToClose([NotNull] object sender, [NotNull] CancelEventArgs cancelEventArgs)
  //  {
  //    Assert.ArgumentNotNull(sender, "sender");
  //    Assert.ArgumentNotNull(cancelEventArgs, "cancelEventArgs");
  //    Serialize();
  //  }

  //  private static void Serialize()
  //  {
  //    using (var write = File.Open(FilePath, FileMode.Create))
  //    {
  //      var ser = new DataContractJsonSerializer(typeof(List<string>));
  //      ser.WriteObject(write, List);
  //    }
  //  }

  //  public static bool IsFavorite(string name)
  //  {
  //    return List.Contains(name);
  //  }

  //  public static void ToggleFavorite(string name)
  //  {
  //    if (List.Contains(name))
  //    {
  //      List.Remove(name);
  //    }
  //    else
  //    {
  //      List.Add(name);
  //    }

  //    Serialize();
  //  }
  //}
}