using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SIM.Base;
using SIM.Pipelines.Processors;

namespace SIM.Tool.Windows.Pipelines.Agreement
{
  public class AcceptAgreement : Processor
  {
    protected override void Process(ProcessorArgs args)
    {
      File.WriteAllText(Path.Combine(ApplicationManager.TempFolder, "agreement-accepted.txt"), @"agreement accepted");
    }
  }
}
