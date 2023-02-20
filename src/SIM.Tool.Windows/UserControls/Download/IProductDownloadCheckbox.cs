using System;

namespace SIM.Tool.Windows.UserControls.Download
{
  public interface IProductDownloadCheckBox
  {
    bool IsChecked { get; set; }

    bool IsEnabled { get; }

    string Name { get; }

    Uri Value { get; }

    string ToString();
  }
}