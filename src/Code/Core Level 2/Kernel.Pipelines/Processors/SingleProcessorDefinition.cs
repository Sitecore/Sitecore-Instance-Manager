using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;

namespace SIM.Pipelines.Processors
{
  public class SingleProcessorDefinition : ProcessorDefinition
  {
    #region Public Methods and Operators

    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      return new[] {(Processor) ReflectionUtil.CreateObject(this.Type)};
    }

    #endregion
  }
}