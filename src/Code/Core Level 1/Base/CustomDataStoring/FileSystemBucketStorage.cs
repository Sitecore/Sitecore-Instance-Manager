using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace SIM.Base.CustomDataStoring
{
  /// <summary>
  /// File system storage for bucket. It isn't thread safe, so you should sync access before accessing it.
  /// </summary>
  public class FileSystemBucketStorage : IBucketStorage
  {
    #region Public Methods and Operators

    public virtual Bucket GetRootBucket()
    {
      return TryDeserialize() ?? new Bucket(true, "root");
    }

    public bool SaveRootBucket(Bucket bucketToSave)
    {
      string filePath = GetFilePath();
      try
      {
        DataContractSerializer serializer = GetBucketSerializer();
        using (var fileStream = GetXmlWriter(filePath))
        {
          serializer.WriteObject(fileStream, bucketToSave);
        }
        Log.Debug("Successfully saved root bucket");
        return true;
      }
      catch (Exception ex)
      {
        Log.Error("Unable to serialize custom data", this, ex);
        FileSystem.Local.File.Delete(filePath);
        return false;
      }
    }

    #endregion

    #region Methods

    protected virtual DataContractSerializer GetBucketSerializer()
    {
      return new DataContractSerializer(typeof (Bucket));
    }

    protected virtual string GetFilePath()
    {
      return Path.Combine(ApplicationManager.DataFolder, "PermanentData.xml");
    }

    protected XmlWriter GetXmlWriter(string filePath)
    {
      return XmlWriter.Create(filePath, new XmlWriterSettings() {Indent = true});
    }

    protected virtual Bucket TryDeserialize()
    {
      string filePath = GetFilePath();
      try
      {
        if (!FileSystem.Local.File.Exists(filePath))
          return null;
        DataContractSerializer serializer = GetBucketSerializer();
        using (var fileStream = FileSystem.Local.File.OpenRead(filePath))
        {
          var bucket = serializer.ReadObject(fileStream) as Bucket;
          Assert.IsNotNull(bucket, "Deserialized bucket cannot be null");
          return bucket;
        }
      }
      catch (Exception ex)
      {
        Log.Error("Unable to deserialize custom data", this, ex);
        FileSystem.Local.File.Delete(filePath);
        return null;
      }
    }

    #endregion
  }
}