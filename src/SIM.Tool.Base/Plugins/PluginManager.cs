namespace SIM.Tool.Base.Plugins
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using System.Windows;
  using System.Xml;
  using SIM.Tool.Base.Profiles;

  public class PluginManager
  {
    #region Fields

    public static IEnumerable<Plugin> Plugins =
      
      // within two existing folders
      new[]
      {
        "Plugins", ApplicationManager.PluginsFolder
      }.Where(FileSystem.FileSystem.Local.Directory.Exists)
        
        // get all 1-level nested folders
        .SelectMany(FileSystem.FileSystem.Local.Directory.GetDirectories)
        
        // get all plugins by Plugin.Detect method and ignoring all its null results
        .Select(Plugin.Detect).NotNull();

    #endregion

    #region Public Methods and Operators

    #region Public methods

    public static void ExecuteInitProcessors()
    {
      using (new ProfileSection("Execute <init> processors for plugins", typeof(PluginManager)))
      {
        foreach (var plugin in PluginManager.GetEnabledPlugins())
        {
          ExecuteInitProcessors(plugin);
        }
      }
    }

    public static void ExecuteMainWindowLoadedProcessors(Window mainWindow)
    {
      foreach (var plugin in PluginManager.GetEnabledPlugins())
      {
        try
        {
          var xml = plugin.PluginXmlDocument;
          const string xpath = "/plugin[@version='1.3']/mainWindow/loaded/processor";
          foreach (XmlElement processorNode in xml.SelectElements(xpath))
          {
            var obj = (IMainWindowLoadedProcessor)Plugin.CreateInstance(processorNode);
            ExecuteAsync(() => obj.Process(mainWindow));
          }
        }
        catch (Exception ex)
        {
          PluginManager.HandleError(plugin, ex);
        }
      }
    }

    public static IEnumerable<Plugin> GetEnabledPlugins()
    {
      Profile profile = ProfileManager.Profile;
      return GetEnabledPlugins(profile);
    }

    public static IEnumerable<Plugin> GetEnabledPlugins(Profile profile)
    {
      char[] separator = new[]
      {
        '|', ';', ','
      };
      var workaround = "Plugins\\Support Workaround";
      var plugins = profile.Plugins.Replace(workaround, string.Empty);
      if (EnvironmentHelper.IsSitecoreMachine)
      {
        plugins += "|" + workaround;
      }

      var selected = plugins.Replace("||", "|").Trim(separator).Split(separator);
      return Plugins.Where(plugin => selected.Contains(plugin.PluginFolder));
    }

    public static void HandleError(Plugin plugin, Exception ex)
    {
      WindowHelper.HandleError(
        "The '{0}' plugin failed with exception while initialization. {1}"
          .FormatWith(plugin.PluginFolder, ex.InnerException != null ? ex.InnerException.Message : string.Empty), 
        true, ex, typeof(PluginManager));
    }

    // For every enabled plugin executes the MainWindow:Loaded event.
    public static void Initialize()
    {
      using (new ProfileSection("Initializing plugins", typeof(PluginManager)))
      {
        EnableStockPluginsOnce();
        LoadEnabledPlugins();
        ExecuteInitProcessors();
      }
    }

    #endregion

    #region Private methods

    private static void EnableStockPluginsOnce()
    {
      // Check plugins were already enabled
      try
      {
        var agreementAcceptedFilePath = Path.Combine(ApplicationManager.TempFolder, "stock-plugins-enabled.txt");
        if (!File.Exists(agreementAcceptedFilePath))
        {
          var arr = @"Plugins\Log Analyzer".Split('|');
          var profile = ProfileManager.Profile;
          var plugins = profile.Plugins.Split("|,;".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          foreach (var plugin in arr)
          {
            if (!plugins.Any(x => x.EqualsIgnoreCase(plugin)))
            {
              profile.Plugins += "|" + plugin;
            }
          }

          ProfileManager.SaveChanges(profile);
          File.WriteAllText(agreementAcceptedFilePath, string.Empty);
        }
      }
      catch (Exception ex)
      {
        Log.Error("Enable stock plugins failed", typeof(PluginManager), ex);
      }
    }

    #endregion

    #endregion

    #region Methods

    private static void ExecuteAsync(ThreadStart threadStart)
    {
      var thread = new Thread(threadStart);
      thread.SetApartmentState(ApartmentState.STA);
      thread.Start();
    }

    private static void ExecuteInitProcessor(XmlElement processorNode)
    {
      using (new ProfileSection("Execute <init> processor", typeof(PluginManager)))
      {
        ProfileSection.Argument("processorNode", processorNode);

        var isAsync = processorNode.GetAttribute("mode").EqualsIgnoreCase("async");
        var obj = (IInitProcessor)Plugin.CreateInstance(processorNode);
        if (!isAsync)
        {
          obj.Process();

          ProfileSection.Result("Done");
          return;
        }

        ExecuteAsync(obj.Process);

        ProfileSection.Result("Started async");
      }
    }

    private static void ExecuteInitProcessors(Plugin plugin)
    {
      using (new ProfileSection("Execute <init> processors for plugins", typeof(PluginManager)))
      {
        ProfileSection.Argument("plugin", plugin);

        try
        {
          var xml = plugin.PluginXmlDocument;
          const string xpath = "/plugin[@version='1.3']/init/processor";

          foreach (XmlElement processorNode in xml.SelectElements(xpath))
          {
            ExecuteInitProcessor(processorNode);
          }

          ProfileSection.Result("Done");
        }
        catch (Exception ex)
        {
          HandleError(plugin, ex);

          ProfileSection.Result("Failed");
        }
      }
    }

    private static void LoadEnabledPlugin(Plugin plugin)
    {
      using (new ProfileSection("Loading enabled plugin", typeof(PluginManager)))
      {
        ProfileSection.Argument("plugin", plugin);

        try
        {
          plugin.Load();
          ProfileSection.Result("Loaded");
        }
        catch (Exception ex)
        {
          WindowHelper.HandleError("Error while loading {0} plugin".FormatWith(plugin.PluginFolder), true, ex);
          ProfileSection.Result("Failed");
        }
      }
    }

    private static void LoadEnabledPlugins()
    {
      using (new ProfileSection("Load enabled plugins", typeof(PluginManager)))
      {
        foreach (var plugin in GetEnabledPlugins())
        {
          LoadEnabledPlugin(plugin);
        }
      }
    }

    #endregion
  }
}