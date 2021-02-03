using SIM.Tool.Base.Pipelines;
using SIM.Tool.Base.Wizards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Windows.Pipelines.Install
{
  public class Install9ActionsHive : FinishActionHive
  {
    public Install9ActionsHive(Type type) : base(type)
    {

    }
    public override IEnumerable<FinishAction> GetFinishActions(WizardArgs wizardArgs)
    {
      var args = (Install9WizardArgs)wizardArgs;
      List<FinishAction> actions = new List<FinishAction>();
      if (args.ScriptsOnly)
      {
        MethodInfo method = Type.GetType("SIM.Tool.Windows.Pipelines.Install.OpenScriptsFolderFinishAction, SIM.Tool.Windows").GetMethod("Run", BindingFlags.Static | BindingFlags.Public);
        var action = new FinishAction("Open scripts folder", method);
        actions.Add(action);
      }

      return actions;
    }
  }
}
