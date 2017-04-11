namespace SIM.Tool.Base.Wizards
{
  using System;

  #region

  #endregion

  public class StepInfo
  {
    #region Fields

    public Type Control { get; }

    public string Param { get; }

    public string Title { get; }

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