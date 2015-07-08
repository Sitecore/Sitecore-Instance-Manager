using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Pipelines.Processors
{
  public abstract class ProcessorHive
  {
    #region Public Methods and Operators

    public abstract IEnumerable<Processor> CreateProcessors(ProcessorArgs args);

    #endregion
  }
}