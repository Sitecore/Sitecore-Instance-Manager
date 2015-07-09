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
      this.InitializeComponent();
      this.TextBlock.Text = param;
    }

    #endregion
  }
}