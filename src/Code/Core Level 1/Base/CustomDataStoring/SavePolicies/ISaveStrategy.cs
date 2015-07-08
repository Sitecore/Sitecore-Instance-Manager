using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Base.CustomDataStoring.SavePolicies
{
  public interface ISaveStrategy
  {
    #region Public Methods

    void HandleBoxChange();

    #endregion
  }
}