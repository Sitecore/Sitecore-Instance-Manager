using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using JetBrains.Annotations;
using SIM.Instances;
using SIM.Tool.Base.Profiles;
using SIM.Extensions;
using Microsoft.Extensions.Logging;

namespace SIM.Tool.Base.Tools.License
{
  internal enum UpdateMode
  {
    UpdateAllInstances,
    UpdateSingleInstance
  }
  public class LicenseManager
  {
    private const string Title = "Update license";

    private ILogger Logger => new Core.Logging.SitecoreLogger(LogLevel.Information);

    private List<LicenseUpdaterBase> _licenseUpdaters;

    private List<LicenseUpdaterBase> LicenseUpdaters =>
      _licenseUpdaters ?? (_licenseUpdaters = new List<LicenseUpdaterBase>()
      {
        new Sitecore8LicenseUpdater(Logger),
        new Sitecore9LicenseUpdater(Logger),
        new DockerComposeLicenseUpdater(Logger)
      });

    public void Update(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        const string Current = "Selected instance";
        const string All = "All instances";
        var options = new[]
        {
          Current, All
        };

        var result = WindowHelper.AskForSelection(Title, null, "You have selected \"{0}\" Sitecore instance. \nWould you like to update the license file only there?".FormatWith(instance.Name), options, mainWindow);

        if (result == null)
        {
          return;
        }

        if (result == All)
        {
          instance = null;
        }
      }

      UpdateMode updateMode = instance == null ? UpdateMode.UpdateAllInstances : UpdateMode.UpdateSingleInstance;

      string filePath = ProfileManager.Profile?.License;

      const string Settings = "Defined in settings";
      const string Custom = "Another license file";
      var options2 = new[]
      {
        Settings, Custom
      };
      var result2 = WindowHelper.AskForSelection(Title, null, "Which license file would you like to use?", options2, mainWindow);

      if (result2 == null)
      {
        return;
      }

      if (result2 == Custom)
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

      if (string.IsNullOrEmpty(filePath))
      {
        Logger.Log(LogLevel.Error, $"LicenseManager: the selected license path is not valid(IsNullOrEmpty).");

        return;
      }

      WindowHelper.LongRunningTask(() => DoUpdateLicense(filePath, instance, updateMode), "Updating license...", mainWindow);
    }

    private void DoUpdateLicense(string licenseFilePath, [CanBeNull]Instance instance, UpdateMode updateMode)
    {
      foreach (var updater in LicenseUpdaters)
      {
        updater.Update(licenseFilePath, updateMode, instance);
      }
    }
  }
}