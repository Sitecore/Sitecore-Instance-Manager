using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace SIM.CustomDataStoring
{
  [DataContract(Namespace = "")]
  public class Bucket
  {
    #region Constructors

    public Bucket()
      : this(false, null)
    {
    }

    public Bucket(bool createDefault, string name)
    {
      this.IsAlreadyDeleted = false;
      if (!createDefault)
      {
        return;
      }

      this.Name = name;
      this.BoolEntries = new Dictionary<string, bool>();
      this.DoubleEntries = new Dictionary<string, double>();
      this.IntEntries = new Dictionary<string, int>();
      this.StringEntries = new Dictionary<string, string>();
      this.LongEntries = new Dictionary<string, long>();
      this.InnerBuckets = new List<Bucket>();
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
    ///   This property exists to indicate that current bucket from already deleted.
    ///   This prevents users from mistakes when they might work with already deleted bucket.
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
      {
        return;
      }

      this.DeleteInnerBucket(innerBucket);
    }

    public virtual void DeleteInnerBucket(Bucket bucket)
    {
      this.InnerBuckets.Remove(bucket);
      bucket.NotifyIsOrphan();
    }

    #endregion

    #region Methods

    protected virtual void NotifyIsOrphan()
    {
      this.IsAlreadyDeleted = true;
      foreach (Bucket innerBucket in this.InnerBuckets)
      {
        innerBucket.NotifyIsOrphan();
      }
    }

    #endregion
  }
}