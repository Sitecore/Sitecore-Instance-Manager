using System;
using System.Collections.Generic;
using System.Linq;
using SIM.CustomDataStoring.SavePolicies;

namespace SIM.CustomDataStoring
{
  using Sitecore.Diagnostics.Logging;

  public class StorageBox : IDataBox
  {
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
      get
      {
        return this.UnderlyingBucket.IsAlreadyDeleted;
      }
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
        {
          throw new InvalidOperationException("SubBox with name '{0}' doesn't exist.".FormatWith(name));
        }

        return;
      }

      try
      {
        PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
        if (this.InnerBoxes.ContainsKey(normalizedName))
        {
          StorageBox boxToDelete = this.InnerBoxes[normalizedName];
          this.InnerBoxes.Remove(normalizedName);
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
        this.HandleSaveRequest();
      }
    }

    public virtual bool GetBool(string key, bool defaultValue)
    {
      return this.GetValue(this.UnderlyingBucket.BoolEntries, key, defaultValue);
    }

    public Bucket GetBucket()
    {
      return this.UnderlyingBucket;
    }

    public virtual double GetDouble(string key, double defaultValue)
    {
      return this.GetValue(this.UnderlyingBucket.DoubleEntries, key, defaultValue);
    }

    public virtual int GetInt(string key, int defaultValue)
    {
      return this.GetValue(this.UnderlyingBucket.IntEntries, key, defaultValue);
    }

    public virtual long GetLong(string key, long defaultValue)
    {
      return this.GetValue(this.UnderlyingBucket.LongEntries, key, defaultValue);
    }

    public virtual IDataBox GetOrCreateSubBox(string name, ISaveStrategy saveStrategy)
    {
      return this.ResolveNamedSubContainer(name, saveStrategy);
    }

    public virtual IDataBox GetOrCreateSubBox(string name)
    {
      return this.GetOrCreateSubBox(name, new ImmediateSaveStrategy());
    }

    public virtual IEnumerable<string> GetPresentSubBoxNames()
    {
      return this.UnderlyingBucket.InnerBuckets.Select(x => x.Name);
    }

    public virtual string GetString(string key, string defaultValue)
    {
      return this.GetValue(this.UnderlyingBucket.StringEntries, key, defaultValue);
    }

    public virtual void SetBool(string key, bool value)
    {
      this.SetValue(this.UnderlyingBucket.BoolEntries, key, value);
    }

    public virtual void SetDouble(string key, double value)
    {
      this.SetValue(this.UnderlyingBucket.DoubleEntries, key, value);
    }

    public virtual void SetInt(string key, int value)
    {
      this.SetValue(this.UnderlyingBucket.IntEntries, key, value);
    }

    public virtual void SetLong(string key, long value)
    {
      this.SetValue(this.UnderlyingBucket.LongEntries, key, value);
    }

    public virtual void SetString(string key, string value)
    {
      this.SetValue(this.UnderlyingBucket.StringEntries, key, value);
    }

    public virtual bool SubBoxExists(string name)
    {
      string normalizedName = this.NomalizeName(name);
      if (this.InnerBoxes.ContainsKey(normalizedName))
      {
        return true;
      }

      return
        this.UnderlyingBucket.InnerBuckets.Any(
          bucket => bucket.Name.Equals(normalizedName, StringComparison.OrdinalIgnoreCase));
    }

    public virtual bool TryGetBool(string key, out bool value)
    {
      return this.TryGetValue(this.UnderlyingBucket.BoolEntries, key, out value);
    }

    public virtual bool TryGetDouble(string key, out double value)
    {
      return this.TryGetValue(this.UnderlyingBucket.DoubleEntries, key, out value);
    }

    public virtual bool TryGetInt(string key, out int value)
    {
      return this.TryGetValue(this.UnderlyingBucket.IntEntries, key, out value);
    }

    public virtual bool TryGetLong(string key, out long value)
    {
      return this.TryGetValue(this.UnderlyingBucket.LongEntries, key, out value);
    }

    #endregion

    #region Methods

    protected virtual void AssertBoxIsNotDeleted()
    {
      if (this.IsDeleted)
      {
        throw new InvalidOperationException("Operation cannot be performed on deleted node");
      }
    }

    protected virtual Bucket GetOrCreateInnerBucket(string name)
    {
      Bucket existingBucket =
        this.UnderlyingBucket.InnerBuckets.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
      if (existingBucket != null)
      {
        return existingBucket;
      }

      // Need to manage bucket and add inner bucket. Bucket will not take care of itself.
      var newBucket = new Bucket(true, name);

      // Access sync
      PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
      this.UnderlyingBucket.InnerBuckets.Add(newBucket);
      PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
      return newBucket;
    }

    protected virtual T GetValue<T>(Dictionary<string, T> dictionary, string key, T defaultValue)
    {
      T value;
      return this.TryGetValue(dictionary, key, out value) ? value : defaultValue;
    }

    protected virtual bool HandleSaveRequest()
    {
      if (this.SaveStrategy == null)
      {
        return this.PerformActualSaveOperation();
      }

      this.SaveStrategy.HandleBoxChange();
      return true;
    }

    protected virtual string NomalizeName(string name)
    {
      return name.ToLowerInvariant();
    }

    protected virtual bool PerformActualSaveOperation()
    {
      if (!this.SaveHandler())
      {
        Log.Error("Unable to save setting from StorageBox");
      }

      return true;
    }

    protected virtual StorageBox ResolveNamedSubContainer(string name, ISaveStrategy saveStrategy)
    {
      this.AssertBoxIsNotDeleted();
      name = this.NomalizeName(name);
      if (this.InnerBoxes.ContainsKey(name))
      {
        return this.InnerBoxes[name];
      }

      try
      {
        PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
        var appropriateBucket = this.GetOrCreateInnerBucket(name);
        var box = new StorageBox(appropriateBucket, saveStrategy, this.HandleSaveRequest);
        this.InnerBoxes[name] = box;
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

      // Access sync
      PermanentInfoStorage.ModificationsSyncRoot.EnterReadLock();
      dictionary[key] = value;
      PermanentInfoStorage.ModificationsSyncRoot.ExitReadLock();
      this.HandleSaveRequest();
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

    #region Constructors

    public StorageBox(Bucket bucket, ISaveStrategy saveStrategy, SaveRequestHandler saver)
    {
      this.Name = bucket.Name;
      this.UnderlyingBucket = bucket;
      this.SaveStrategy = saveStrategy;
      this.SaveHandler = saver;
      this.InnerBoxes = new Dictionary<string, StorageBox>();
    }

    public StorageBox(Bucket bucket, SaveRequestHandler saver)
      : this(bucket, new ImmediateSaveStrategy(), saver)
    {
    }

    #endregion
  }
}