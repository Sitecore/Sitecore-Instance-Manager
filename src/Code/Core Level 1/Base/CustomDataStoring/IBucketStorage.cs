using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Base.CustomDataStoring
{
  public interface IBucketStorage
  {
    #region Public Methods and Operators

    Bucket GetRootBucket();
    bool SaveRootBucket(Bucket bucketToSave);

    #endregion
  }
}