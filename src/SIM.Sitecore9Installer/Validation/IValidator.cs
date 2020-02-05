using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIM.Sitecore9Installer.Tasks;

namespace SIM.Sitecore9Installer.Validation
{
  public interface IValidator
  {
    ValidationResult Evaluate(IEnumerable<Task> tasks);
  }
}
