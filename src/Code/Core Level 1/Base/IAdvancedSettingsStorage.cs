using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Base
{
  public interface IAdvancedSettingsStorage
  {
    void Initialize();
    string ReadSetting(string key, string defaultValue);
    void WriteSetting(string key, string value);
  }
}
