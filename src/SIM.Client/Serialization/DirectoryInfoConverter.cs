namespace SIM.Client.Serialization
{
  using System;
  using System.IO;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public class DirectoryInfoConverter : JsonConverter
  {
    public override void WriteJson([NotNull] JsonWriter writer, [CanBeNull] object value, [NotNull] JsonSerializer serializer)
    {
      Assert.ArgumentNotNull(writer, "writer");
      Assert.ArgumentNotNull(serializer, "serializer");

      var dir = value as DirectoryInfo;
      serializer.Serialize(writer, dir != null ? dir.FullName : null);
    }

    [CanBeNull]
    public override object ReadJson([CanBeNull] JsonReader reader, [CanBeNull] Type objectType, [CanBeNull] object existingValue, [CanBeNull] JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override bool CanConvert([CanBeNull] Type objectType)
    {
      return objectType == typeof(DirectoryInfo);
    }
  }
}