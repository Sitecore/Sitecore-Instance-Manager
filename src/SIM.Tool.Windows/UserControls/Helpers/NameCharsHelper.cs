using SIM.Tool.Base;
using System.IO;
using System.Linq;
using System.Windows;

namespace SIM.Tool.Windows.UserControls.Helpers
{
  public static class NameCharsHelper
  {
    private static char[] _InvalidChars;

    static NameCharsHelper()
    {
      _InvalidChars = Path.GetInvalidFileNameChars();
    }

    public static bool IsValidNameChar(string nameChar, string fieldName)
    {
      if (nameChar.Any(c => _InvalidChars.Contains(c)))
      {
        WindowHelper.ShowMessage($"The enetered '{nameChar}' character is invalid for the {fieldName}.",
          messageBoxImage: MessageBoxImage.Warning,
          messageBoxButton: MessageBoxButton.OK
        );
        return false;
      }
      return true;
    }
  }
}