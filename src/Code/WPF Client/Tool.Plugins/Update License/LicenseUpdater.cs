using SIM.Tool.Base.Profiles;

namespace SIM.Tool.Plugins.UpdateLicense
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Forms;
  using SIM.Base;
  using SIM.Instances;
  using SIM.Tool.Base;

  public static class LicenseUpdater
  {
    const string Title = "Update license";
    
    public static void Update(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        const string current = "Selected instance";
        const string all = "All instances";
        var options = new[] { current, all };

        var result = WindowHelper.AskForSelection(Title, null, "You have selected \"{0}\" Sitecore instance. \nWould you like to update the license file only there?".FormatWith(instance.Name), options, mainWindow);

        if (result == null)
        {
          return;
        }

        if (result == all)
        {
          instance = null;
        }
      }

      string filePath = ProfileManager.Profile.License;

      const string settings = "Definied in settings";
      const string custom = "Another license file";
      var options2 = new[] { settings, custom };
      var result2 = WindowHelper.AskForSelection(Title, null, "Which license file would you like to use?", options2, mainWindow);

      if (result2 == null)
      {
        return;
      }

      if (result2 == custom)
      {
        var openDialog = new OpenFileDialog
          {
            Filter = @"License files|*.xml"
          };

        if (openDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK)
        {
          return;
        }

        filePath = openDialog.FileName;
      }


      WindowHelper.LongRunningTask(() => DoUpdateLicense(filePath, instance), "Updating license...", mainWindow);
    }

    private static void DoUpdateLicense(string licenseFilePath, Instance instance)
    {
      if (instance != null)
      {
        try
        {
          FileSystem.Local.File.Copy(licenseFilePath, instance.LicencePath, true);
        }
        catch (Exception ex)
        {
          Log.Error(ex.Message, typeof(LicenseUpdater), ex);
        }
      }
      else
      {
        foreach (Instance inst in InstanceManager.Instances)
        {
          try
          {
            FileSystem.Local.File.Copy(licenseFilePath, inst.LicencePath, true);
          }
          catch (Exception ex)
          {
            Log.Error(ex.Message, typeof(LicenseUpdater), ex);
          }
        }
      }
    }
  }
}