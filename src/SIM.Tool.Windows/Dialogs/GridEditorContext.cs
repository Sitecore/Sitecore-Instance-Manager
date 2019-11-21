using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace SIM.Tool.Windows.Dialogs
{
  class GridEditorContext
  {
    public ObservableCollection<object> GridItems { get; }
    public string Description { get; }

    public GridEditorContext(IEnumerable<object> itemsSource, string description)
    {
      Assert.ArgumentNotNull(itemsSource, nameof(itemsSource));
      Assert.ArgumentNotNullOrEmpty(description, nameof(description));
      this.GridItems = new ObservableCollection<object>(itemsSource);
      this.Description = description;
    }
  }
}
