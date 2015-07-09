namespace SIM.CustomDataStoring
{
  public interface IBucketStorage
  {
    #region Public Methods and Operators

    Bucket GetRootBucket();
    bool SaveRootBucket(Bucket bucketToSave);

    #endregion
  }
}