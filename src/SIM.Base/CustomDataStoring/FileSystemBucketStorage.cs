using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Sitecore.Diagnostics.Base;

namespace SIM.CustomDataStoring
{
  using Sitecore.Diagnostics.Logging;

  /// <summary>
  ///   File system storage for bucket. It isn't thread safe, so you should sync access before accessing it.
  /// </summary>
  public class FileSystemBucketStorage : IBucketStorage
  {
    #region Public Methods and Operators

    public virtual Bucket GetRootBucket()
    {
      return this.TryDeserialize() ?? new Bucket(true, "root");
    }

    public bool SaveRootBucket(Bucket bucketToSave)
    {
      string filePath = this.GetFilePath();
      try
      {
        DataContractSerializer serializer = this.GetBucketSerializer();
        using (var fileStream = this.GetXmlWriter(filePath))
        {
          serializer.WriteObject(fileStream, bucketToSave);
        }

        Log.Debug("Successfully saved root bucket");
        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Unable to serialize custom data");
        FileSystem.FileSystem.Local.File.Delete(filePath);
        return false;
      }
    }

    #endregion

    #region Methods

    protected virtual DataContractSerializer GetBucketSerializer()
    {
      return new DataContractSerializer(typeof(Bucket));
    }

    protected virtual string GetFilePath()
    {
      return Path.Combine(ApplicationManager.DataFolder, "PermanentData.xml");
    }

    protected XmlWriter GetXmlWriter(string filePath)
    {
      return XmlWriter.Create(filePath, new XmlWriterSettings()
      {
        Indent = true
      });
    }

    protected virtual Bucket TryDeserialize()
    {
      string filePath = this.GetFilePath();
      try
      {
        if (!FileSystem.FileSystem.Local.File.Exists(filePath))
        {
          return null;
        }

        DataContractSerializer serializer = this.GetBucketSerializer();
        using (var fileStream = FileSystem.FileSystem.Local.File.OpenRead(filePath))
        {
          var bucket = serializer.ReadObject(fileStream) as Bucket;
          Assert.IsNotNull(bucket, "Deserialized bucket cannot be null");
          return bucket;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Unable to deserialize custom data");
        FileSystem.FileSystem.Local.File.Delete(filePath);
        return null;
      }
    }

    #endregion
  }
}