using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Base;

namespace SIM.Pipelines.Processors
{
  public class MultipleProcessorDefinition : ProcessorDefinition
  {
    #region Public Methods and Operators

    public override IEnumerable<Processor> CreateProcessors(ProcessorArgs args)
    {
      var hive = (ProcessorHive) ReflectionUtil.CreateObject(this.Type);
      return hive.CreateProcessors(args);
    }

    #endregion
  }
}