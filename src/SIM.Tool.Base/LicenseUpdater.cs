namespace SIM.Tool.Base
{
  using System;
  using System.Windows;
  using System.Windows.Forms;
  using SIM.Instances;
  using SIM.Tool.Base.Profiles;
  using Sitecore.Diagnostics.Logging;

  public static class LicenseUpdater
  {
    #region Constants

    private const string Title = "Update license";

    #endregion

    #region Public methods

    public static void Update(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        const string current = "Selected instance";
        const string all = "All instances";
        var options = new[]
        {
          current, all
        };

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
      var options2 = new[]
      {
        settings, custom
      };
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

    #endregion

    #region Private methods

    private static void DoUpdateLicense(string licenseFilePath, Instance instance)
    {
      if (instance != null)
      {
        try
        {
          FileSystem.FileSystem.Local.File.Copy(licenseFilePath, instance.LicencePath, true);
        }
        catch (Exception ex)
        {
          Log.Error(ex.Message);
        }
      }
      else
      {
        foreach (Instance inst in InstanceManager.Instances)
        {
          try
          {
            FileSystem.FileSystem.Local.File.Copy(licenseFilePath, inst.LicencePath, true);
          }
          catch (Exception ex)
          {
            Log.Error(ex.Message);
          }
        }
      }
    }

    #endregion
  }
}