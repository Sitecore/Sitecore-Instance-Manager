namespace SIM.Tool.Base.Wizards
{
  using System.Reflection;

  #region

  #endregion

  public class FinishAction
  {
    #region Fields

    public readonly MethodInfo Method;

    public readonly string Text;

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