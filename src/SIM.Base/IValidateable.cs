using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIM
{
  public interface IValidateable
  {
    string ValidateAndGetError();
  }
}
