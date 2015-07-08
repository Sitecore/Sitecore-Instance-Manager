#region Usings

using System;

#endregion

namespace SIM.Tool.Wizards
{
  #region

  

  #endregion

  /// <summary>
  ///   The step info.
  /// </summary>
  public class StepInfo
  {
    #region Fields

    /// <summary>
    ///   The control.
    /// </summary>
    public readonly Type Control;

    /// <summary>
    ///   The param.
    /// </summary>
    public readonly string Param;

    /// <summary>
    ///   The title.
    /// </summary>
    public readonly string Title;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StepInfo"/> class.
    /// </summary>
    /// <param name="title">
    /// The title. 
    /// </param>
    /// <param name="control">
    /// The control. 
    /// </param>
    /// <param name="param">
    /// The param. 
    /// </param>
    public StepInfo(string title, Type control, string param = null)
    {
      this.Title = title;
      this.Control = control;
      this.Param = param ?? string.Empty;
    }

    #endregion
  }
}