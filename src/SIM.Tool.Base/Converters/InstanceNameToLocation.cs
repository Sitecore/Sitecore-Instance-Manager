using SIM.Tool.Base.Profiles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace SIM.Tool.Base.Converters
{
  [ValueConversion(typeof(string), typeof(string))]
  public class InstanceNameToLocation : IValueConverter
  {
    public static InstanceNameToLocation Instance { get; } = new InstanceNameToLocation(); 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string folder = value as string;
      if (ProfileManager.Profile?.InstancesFolder == null||folder==null)
      {
        return null;
      }
      return Path.Combine(ProfileManager.Profile.InstancesFolder, folder);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
