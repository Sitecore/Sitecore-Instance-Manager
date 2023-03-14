using Sitecore.Diagnostics.InfoService.Client.Model;
using System.Collections.Generic;

namespace SIM.Tool.Windows.UserControls.Download
{
  public interface ICheckBoxCreator
  {
    IEnumerable<IProductDownloadCheckBox> Create(IRelease release);
  }
}