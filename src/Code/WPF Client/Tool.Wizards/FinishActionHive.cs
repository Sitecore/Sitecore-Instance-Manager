#region Usings

using System;
using System.Collections.Generic;
using SIM.Tool.Base.Wizards;

#endregion

namespace SIM.Tool.Wizards
{
  #region

  

  #endregion

  /// <summary>
  ///   The finish action hive.
  /// </summary>
  public abstract class FinishActionHive
  {
    #region Fields

    /// <summary>
    ///   The wizard arguments type.
    /// </summary>
    protected readonly Type WizardArgumentsType;

    #endregion

    #region Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="FinishActionHive"/> class.
    /// </summary>
    /// <param name="type">
    /// The type. 
    /// </param>
    public FinishActionHive(Type type)
    {
      this.WizardArgumentsType = type;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// The get finish actions.
    /// </summary>
    /// <param name="wizardArgs">
    /// The wizard args. 
    /// </param>
    /// <returns>
    /// </returns>
    public abstract IEnumerable<FinishAction> GetFinishActions(WizardArgs wizardArgs);

    #endregion
  }
}