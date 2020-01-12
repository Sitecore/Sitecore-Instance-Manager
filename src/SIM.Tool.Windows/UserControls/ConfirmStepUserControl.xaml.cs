using SIM.Sitecore9Installer;
using SIM.Tool.Base;
using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Profiles;
using SIM.Tool.Base.Wizards;
using System.IO;
using System.Linq;

namespace SIM.Tool.Windows.UserControls
{
  /// <summary>
  ///   The confirm step user control.
  /// </summary>
  public partial class ConfirmStepUserControl
  {
    #region Constructors

    public ConfirmStepUserControl(string param)
    {
      InitializeComponent();
      TextBlock.Text = param;
    } 
    #endregion
  }
}