using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using SIM.Pipelines.Processors;

namespace SIM.Pipelines.Install
{
  public class Run9Installation : Processor
  {
    protected override void Process([NotNull] ProcessorArgs args)
    {
      Thread.Sleep(10000);
    }

    public override long EvaluateStepsCount([NotNull] ProcessorArgs args)
    {
      Install9Args arguments = (Install9Args)args;
      return arguments.Tasker.Tasks.Count(t => t.ShouldRun);
    }

    
  }
}
