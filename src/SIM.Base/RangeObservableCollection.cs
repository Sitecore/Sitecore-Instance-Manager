using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM
{
  public class RangeObservableCollection<T> : ObservableCollection<T>
  {
    public RangeObservableCollection(IEnumerable<T> list) : base(list)
    {
      
    }

    public RangeObservableCollection():base()
    {
      
    }
    private bool _suppressNotification = false;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (!_suppressNotification)
        base.OnCollectionChanged(e);
    }

    public void AddRange(IEnumerable<T> list)
    {
      if (list == null)
        throw new ArgumentNullException("list");

      _suppressNotification = true;

      foreach (T item in list)
      {
        Add(item);
      }
      _suppressNotification = false;
      OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
  }
}
