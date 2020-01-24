using SIM.Tool.Base.Pipelines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Tool.Windows.Pipelines.Install
{
  public class OpenScriptsFolderFinishAction
  {
    public static void Run(Install9WizardArgs args)
    {
      System.Diagnostics.Process.Start("explorer.exe", Path.Combine(args.ScriptRoot, "generated_scripts"));
    }    
  }
}
