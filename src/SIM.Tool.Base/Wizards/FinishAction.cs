namespace SIM.Tool.Base.Wizards
{
  using System.Reflection;

  #region

  #endregion

  public class FinishAction
  {
    #region Fields

    public MethodInfo Method { get; }

    public string Text { get; }

    #endregion

    #region Constructors

    public FinishAction(string text, MethodInfo method)
    {
      this.Text = text;
      this.Method = method;
    }

    #endregion
  }
}