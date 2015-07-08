#region Usings

using System.Reflection;

#endregion

namespace SIM.Tool.Wizards
{
  #region

  

  #endregion

  /// <summary>
  ///   TODO: Update summary.
  /// </summary>
  public class FinishAction
  {
    #region Fields

    /// <summary>
    ///   The method.
    /// </summary>
    public readonly MethodInfo Method;

    /// <summary>
    ///   The text.
    /// </summary>
    public readonly string Text;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FinishAction"/> class.
    /// </summary>
    /// <param name="text">
    /// The text. 
    /// </param>
    /// <param name="method">
    /// The method. 
    /// </param>
    public FinishAction(string text, MethodInfo method)
    {
      this.Text = text;
      this.Method = method;
    }

    #endregion
  }
}