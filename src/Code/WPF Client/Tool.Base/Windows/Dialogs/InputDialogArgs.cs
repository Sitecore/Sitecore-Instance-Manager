#region Usings

using SIM.Base;

#endregion

namespace SIM.Tool.Base.Windows.Dialogs
{
  public class InputDialogArgs
  {
    #region Public properties

    /// <summary>
    ///   Gets or sets DefaultValue.
    /// </summary>
    public string DefaultValue
    {
      [UsedImplicitly]
      get;
      set;
    }

    /// <summary>
    ///   Gets or sets Title.
    /// </summary>
    public string Title
    {
      [UsedImplicitly]
      get;
      set;
    }

    #endregion
  }
}