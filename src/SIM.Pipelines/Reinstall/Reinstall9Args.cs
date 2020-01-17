using SIM.Pipelines.Install;
using SIM.Pipelines.Processors;
using SIM.Sitecore9Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Pipelines.Reinstall
{
  public class Reinstall9Args: Install9Args
  {
    public Reinstall9Args(Tasker tasker) : base(tasker)
    {
      this.PipelineName = "reinstall9";
    }
  }
}
