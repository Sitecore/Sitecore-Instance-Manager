using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using SIM.Base.CustomDataStoring.SavePolicies;

namespace SIM.Base.CustomDataStoring
{
  public class StorageBox : IDataBox
  {
    #region Constructors and Destructors

    public StorageBox(Bucket bucket, ISaveStrategy saveStrategy, SaveRequestHandler saver)
    {
      Name = bucket.Name;
      UnderlyingBucket = bucket;
      SaveStrategy = saveStrategy;
      SaveHandler = saver;
      InnerBoxes = new Dictionary<string, StorageBox>();
    }

    public StorageBox(Bucket bucket, SaveRequestHandler saver)
      : this(bucket, new ImmediateSaveStrategy(), saver)
    {
    }

    #endregion

    #region Delegates

    public delegate bool SaveRequestHandler();

    #endregion

    #region Public Properties

    public string Name { get; protected set; }
    public Bucket UnderlyingBucket { get; set; }

    #endregion

    #region Properties

    protected Dictionary<string, StorageBox> InnerBoxes { get; set; }

    protected bool IsDeleted
    {
      get { return this.UnderlyingBucket.IsAlreadyDeleted; }
    }

    protected SaveRequestHandler SaveHandler { get; set; }
    protected ISaveStrategy SaveStrategy { get; set; }

    #endregion

    #region Public Methods and Operators

    public virtual void DeleteSubBox(string name, bool throwExceptionIfNotExists)
    {
      var normalizedName = this.NomalizeName(name);
      if (!this.SubBoxExists(normalizedName))
      {
        if (throwExceptionIfNotExists)
          throw new InvalidOperationException("SubBox with name '{0}' doesn't exist.".FormatWith(name));
        return;
      }
      try
      {
        PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
        if (this.InnerBoxes.ContainsKey(normalizedName))
        {
          StorageBox boxToDelete = InnerBoxes[normalizedName];
          InnerBoxes.Remove(normalizedName);
          this.UnderlyingBucket.DeleteInnerBucket(boxToDelete.UnderlyingBucket);
        }
        else
        {
          this.UnderlyingBucket.DeleteInnerBucket(normalizedName);
        }
      }
      finally
      {
        PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
        HandleSaveRequest();
      }
    }

    public virtual bool GetBool(string key, bool defaultValue)
    {
      return GetValue(UnderlyingBucket.BoolEntries, key, defaultValue);
    }

    public Bucket GetBucket()
    {
      return UnderlyingBucket;
    }

    public virtual double GetDouble(string key, double defaultValue)
    {
      return GetValue(UnderlyingBucket.DoubleEntries, key, defaultValue);
    }

    public virtual int GetInt(string key, int defaultValue)
    {
      return GetValue(UnderlyingBucket.IntEntries, key, defaultValue);
    }

    public virtual long GetLong(string key, long defaultValue)
    {
      return GetValue(UnderlyingBucket.LongEntries, key, defaultValue);
    }

    public virtual IDataBox GetOrCreateSubBox(string name, ISaveStrategy saveStrategy)
    {
      return ResolveNamedSubContainer(name, saveStrategy);
    }

    public virtual IDataBox GetOrCreateSubBox(string name)
    {
      return GetOrCreateSubBox(name, new ImmediateSaveStrategy());
    }

    public virtual IEnumerable<string> GetPresentSubBoxNames()
    {
      return UnderlyingBucket.InnerBuckets.Select(x => x.Name);
    }

    public virtual string GetString(string key, string defaultValue)
    {
      return GetValue(UnderlyingBucket.StringEntries, key, defaultValue);
    }

    public virtual void SetBool(string key, bool value)
    {
      SetValue(UnderlyingBucket.BoolEntries, key, value);
    }

    public virtual void SetDouble(string key, double value)
    {
      SetValue(UnderlyingBucket.DoubleEntries, key, value);
    }

    public virtual void SetInt(string key, int value)
    {
      SetValue(UnderlyingBucket.IntEntries, key, value);
    }

    public virtual void SetLong(string key, long value)
    {
      SetValue(UnderlyingBucket.LongEntries, key, value);
    }

    public virtual void SetString(string key, string value)
    {
      SetValue(UnderlyingBucket.StringEntries, key, value);
    }

    public virtual bool SubBoxExists(string name)
    {
      string normalizedName = this.NomalizeName(name);
      if (this.InnerBoxes.ContainsKey(normalizedName))
        return true;
      return
        this.UnderlyingBucket.InnerBuckets.Any(
          bucket => bucket.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));
    }

    public virtual bool TryGetBool(string key, out bool value)
    {
      return TryGetValue(UnderlyingBucket.BoolEntries, key, out value);
    }

    public virtual bool TryGetDouble(string key, out double value)
    {
      return TryGetValue(UnderlyingBucket.DoubleEntries, key, out value);
    }

    public virtual bool TryGetInt(string key, out int value)
    {
      return TryGetValue(UnderlyingBucket.IntEntries, key, out value);
    }

    public virtual bool TryGetLong(string key, out long value)
    {
      return TryGetValue(UnderlyingBucket.LongEntries, key, out value);
    }

    #endregion

    #region Methods

    protected virtual void AssertBoxIsNotDeleted()
    {
      if (this.IsDeleted)
        throw new InvalidOperationException("Operation cannot be performed on deleted node");
    }

    protected virtual Bucket GetOrCreateInnerBucket(string name)
    {
      Bucket existingBucket =
        UnderlyingBucket.InnerBuckets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (existingBucket != null)
        return existingBucket;
      //Need to manage bucket and add inner bucket. Bucket will not take care of itself.
      var newBucket = new Bucket(true, name);
      //Access sync
      PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
      UnderlyingBucket.InnerBuckets.Add(newBucket);
      PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
      return newBucket;
    }

    protected virtual T GetValue<T>(Dictionary<string, T> dictionary, string key, T defaultValue)
    {
      T value;
      return TryGetValue(dictionary, key, out value) ? value : defaultValue;
    }

    protected virtual bool HandleSaveRequest()
    {
      if (SaveStrategy == null)
        return PerformActualSaveOperation();
      SaveStrategy.HandleBoxChange();
      return true;
    }

    protected virtual string NomalizeName(string name)
    {
      return name.ToLowerInvariant();
    }

    protected virtual bool PerformActualSaveOperation()
    {
      if (!SaveHandler())
        Log.Error("Unable to save setting from StorageBox", this);
      return true;
    }

    protected virtual StorageBox ResolveNamedSubContainer(string name, ISaveStrategy saveStrategy)
    {
      this.AssertBoxIsNotDeleted();
      name = NomalizeName(name);
      if (InnerBoxes.ContainsKey(name))
        return InnerBoxes[name];
      try
      {
        PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
        var appropriateBucket = GetOrCreateInnerBucket(name);
        var box = new StorageBox(appropriateBucket, saveStrategy, HandleSaveRequest);
        InnerBoxes[name] = box;
        return box;
      }
      finally
      {
        PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
        this.HandleSaveRequest();
      }
    }

    protected virtual void SetValue<T>(Dictionary<string, T> dictionary, string key, T value)
    {
      this.AssertBoxIsNotDeleted();
      //Access sync
      PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
      dictionary[key] = value;
      PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
      HandleSaveRequest();
    }

    protected virtual bool TryGetValue<T>(Dictionary<string, T> dictionary, string key, out T value)
    {
      this.AssertBoxIsNotDeleted();
      if (dictionary.ContainsKey(key))
      {
        value = dictionary[key];
        return true;
      }
      else
      {
        value = default(T);
        return false;
      }
    }

    #endregion
  }
}