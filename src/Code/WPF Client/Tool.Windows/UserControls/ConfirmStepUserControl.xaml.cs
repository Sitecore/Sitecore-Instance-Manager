#region Usings



#endregion

namespace SIM.Tool.Windows.UserControls
{
  /// <summary>
  ///   The confirm step user control.
  /// </summary>
  public partial class ConfirmStepUserControl
  {
    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfirmStepUserControl"/> class.
    /// </summary>
    /// <param name="param">
    /// The param. 
    /// </param>
    public ConfirmStepUserControl(string param)
    {
      this.InitializeComponent();
      this.TextBlock.Text = param;
    }

    #endregion
  }
}