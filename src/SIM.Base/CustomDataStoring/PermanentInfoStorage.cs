using System.Threading;
using Sitecore.Diagnostics;

namespace SIM.CustomDataStoring
{
  public class PermanentInfoStorage
  {
    #region Fields

    protected static StorageBox rootBox = null;

    #endregion

    #region Constructors

    static PermanentInfoStorage()
    {
      BucketStorage = new FileSystemBucketStorage();
      ModificationsSyncRoot = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   When you want to modify something in tree (e.g. any box or subbox) you should acquire ReadLock.
    ///   WriteLock is acquired internally to ensure that nobody modifies tree when saving is performed.
    /// </summary>
    public static ReaderWriterLockSlim ModificationsSyncRoot { get; protected set; }

    #endregion

    #region Properties

    protected static IBucketStorage BucketStorage { get; set; }


    protected static StorageBox RootBox
    {
      get
      {
        if (rootBox != null)
        {
          return rootBox;
        }

        try
        {
          ModificationsSyncRoot.EnterWriteLock();
          if (rootBox != null)
          {
            return rootBox;
          }

          rootBox = GetBoxFromStorage();
        }
        finally
        {
          ModificationsSyncRoot.ExitWriteLock();
        }

        return rootBox;
      }
    }

    #endregion

    #region Public Methods and Operators

    public static bool ForceSave()
    {
      return PerformSave();
    }

    public static IDataBox GetRootDataBox()
    {
      return RootBox;
    }

    #endregion

    #region Methods

    protected static StorageBox GetBoxFromStorage()
    {
      Bucket bucket = BucketStorage.GetRootBucket();
      Assert.IsNotNull(bucket, "Returned bucket cannot be null");
      return new StorageBox(bucket, ForceSave);
    }

    protected static bool PerformSave()
    {
      if (rootBox == null)
      {
        return true;
      }

      try
      {
        ModificationsSyncRoot.EnterWriteLock();
        return BucketStorage.SaveRootBucket(rootBox.UnderlyingBucket);
      }
      finally
      {
        ModificationsSyncRoot.ExitWriteLock();
      }
    }

    #endregion
  }
}