namespace SIM.Tool.Base.Wizards
{
  using System;

  #region

  #endregion

  public class StepInfo
  {
    #region Fields

    public readonly Type Control;

    public readonly string Param;

    public readonly string Title;

    #endregion

    #region Constructors

    public StepInfo(string title, Type control, string param = null)
    {
      this.Title = title;
      this.Control = control;
      this.Param = param ?? string.Empty;
    }

    #endregion
  }
}