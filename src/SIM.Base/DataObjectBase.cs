namespace SIM
{
  #region

  using System.ComponentModel;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  #endregion

  public class DataObjectBase : INotifyPropertyChanged
  {
    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion

    #region Methods

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