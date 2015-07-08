using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SIM.Base.CustomDataStoring
{
  [DataContract(Namespace = "")]
  public class Bucket
  {
    #region Constructors and Destructors

    public Bucket()
      : this(false, null)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="createDefault">used during deserialization (through the default ctor) to prevent properties from creating</param>
    /// <param name="name"></param>
    public Bucket(bool createDefault, string name)
    {
      this.IsAlreadyDeleted = false;
      if (!createDefault)
        return;
      Name = name;
      BoolEntries = new Dictionary<string, bool>();
      DoubleEntries = new Dictionary<string, double>();
      IntEntries = new Dictionary<string, int>();
      StringEntries = new Dictionary<string, string>();
      LongEntries = new Dictionary<string, long>();
      InnerBuckets = new List<Bucket>();
    }

    #endregion

    #region Public Properties

    [DataMember]
    public Dictionary<string, bool> BoolEntries { get; set; }

    [DataMember]
    public Dictionary<string, double> DoubleEntries { get; set; }

    [DataMember]
    public List<Bucket> InnerBuckets { get; set; }

    [DataMember]
    public Dictionary<string, int> IntEntries { get; set; }

    /// <summary>
    /// This property exists to indicate that current bucket from already deleted.
    /// This prevents users from mistakes when they might work with already deleted bucket. 
    /// </summary>
    public bool IsAlreadyDeleted { get; protected set; }

    [DataMember]
    public Dictionary<string, long> LongEntries { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public Dictionary<string, string> StringEntries { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void DeleteInnerBucket(string name)
    {
      Bucket innerBucket =
        this.InnerBuckets.FirstOrDefault(bucket => bucket.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (innerBucket == null)
        return;
      this.DeleteInnerBucket(innerBucket);
    }

    public virtual void DeleteInnerBucket(Bucket bucket)
    {
      this.InnerBuckets.Remove(bucket);
      bucket.NotifyIsOrphan();
    }

    #endregion

    #region Methods

    /// <summary>
    /// This method sets the IsAlreadyDeleted flag and notify inner buckets that they are removed.
    /// </summary>
    protected virtual void NotifyIsOrphan()
    {
      this.IsAlreadyDeleted = true;
      foreach (Bucket innerBucket in InnerBuckets)
      {
        innerBucket.NotifyIsOrphan();
      }
    }

    #endregion
  }
}