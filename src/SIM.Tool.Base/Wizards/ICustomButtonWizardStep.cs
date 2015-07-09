namespace SIM.Tool.Base.Wizards
{
  public interface ICustomButton
  {
    #region Public properties

    string CustomButtonText { get; }

    #endregion

    #region Public methods

    void CustomButtonClick();

    #endregion
  }
}