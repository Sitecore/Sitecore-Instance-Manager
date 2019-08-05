using Sitecore.Diagnostics.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Windows.Dialogs
{
  class GridEditorContext 
  {
    public IEnumerable<object> GridItems { get; }
    public string Description { get; }

    public GridEditorContext(IEnumerable<object> itemsSource, string description)
    {
      Assert.ArgumentNotNull(itemsSource, nameof(itemsSource));
      Assert.ArgumentNotNullOrEmpty(description, nameof(description));
      this.GridItems = itemsSource;
      this.Description = description;
    }
  }
}
