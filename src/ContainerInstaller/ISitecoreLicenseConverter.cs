using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerInstaller
{
  public interface ISitecoreLicenseConverter
  {
    string Convert(string licenseFilePath);
  }
}
