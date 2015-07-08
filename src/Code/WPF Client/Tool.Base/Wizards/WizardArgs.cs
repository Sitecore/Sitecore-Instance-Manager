#region Usings

using System.Windows;
using SIM.Base;
using SIM.Pipelines.Processors;

#endregion

namespace SIM.Tool.Base.Wizards
{
  #region

  

  #endregion

  /// <summary>
  ///   The wizard args.
  /// </summary>
  public abstract class WizardArgs : AbstractArgs
  {
    public Window WizardWindow { get; set; }
       
    #region Public Methods

    /// <summary>
    ///   Converts the <see cref="WizardArgs" /> to a <see cref="ProcessorArgs" /> .
    /// </summary>
    /// <returns> The <see cref="ProcessorArgs" /> . </returns>
    public virtual ProcessorArgs ToProcessorArgs()
    {
      return null;
    }

    #endregion
  }
}