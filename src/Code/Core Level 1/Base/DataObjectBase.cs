#region Usings

using System.ComponentModel;

#endregion

namespace SIM.Base
{
  #region

  

  #endregion

  /// <summary>
  ///   The data object base.
  /// </summary>
  public class DataObjectBase : INotifyPropertyChanged
  {
    #region Events

    /// <summary>
    ///   The property changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Methods

    /// <summary>
    /// The notify property changed.
    /// </summary>
    /// <param name="name">
    /// The name. 
    /// </param>
    protected void NotifyPropertyChanged([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, "name");

      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(name));
      }
    }

    #endregion
  }
}