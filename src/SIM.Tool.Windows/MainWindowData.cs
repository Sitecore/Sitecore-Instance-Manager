namespace SIM.Tool.Windows
{
  using System;
  using System.IO;
  using SIM.Tool.Windows.MainWindowComponents.Buttons;
  using SIM.Tool.Windows.MainWindowComponents.Groups;

  public static class MainWindowData
  {
    private static TabDefinition HomeTab { get; } = new TabDefinition
    {
      Name = "Home",
      Groups = new[]
      {
        new GroupDefinition
        {
          Name = "Refresh",
          Handler = new WindowOnlyGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Refresh sites",
              Image = "/Images/$lg/refresh.png, SIM.Tool.Windows",
              Handler = new RefreshButton("sites"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Refresh sites",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new RefreshButton("sites")
                },
                new ButtonDefinition
                {
                  Label = "Refresh installer",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new RefreshButton("installer")
                },
                new ButtonDefinition
                {
                  Label = "Refresh caches",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new RefreshButton("caches")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Refresh all",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new RefreshButton("all")
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "Install",
          Handler = new WindowOnlyGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Install on-prem",
              Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
              Handler = new Install9InstanceButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Install Sitecore XP 9 and later",
                  Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
                  Handler = new Install9InstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Install Sitecore XP 8 and earlier",
                  Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
                  Handler = new InstallInstanceButton()
                }
              }
            },
            new ButtonDefinition
            {
              Label = "Deploy to Docker",
              Image = "/Images/$lg/docker_logo.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.InstallContainerButton()
            },
            new ButtonDefinition
            {
              Label = "Import Solution",
              Image = "/Images/$lg/upload.png, SIM.Tool.Windows",
              Handler = new ImportInstanceButton()
            }
          }
        },
        new GroupDefinition
        {
          Name = "Tools",
          Handler = new WindowOnlyGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Get Sitecore",
              Image = "/Images/$lg/cloud_download.png, SIM.Tool.Windows",
              Handler = new Download8Button()
            },
            new ButtonDefinition
            {
              Label = "Bundled Tools",
              Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows",
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "SIM Logs",
                  Image = "/Images/$sm/folder_document2.png, SIM.Tool.Windows",
                  Handler = new OpenSimFolderButton(@"%APPDATA%\Sitecore\Sitecore Instance Manager\Logs")
                },
                new ButtonDefinition
                {
                  Label = "SIM Data Folder",
                  Image = "/Images/$sm/folder_window.png, SIM.Tool.Windows",
                  Handler = new OpenSimFolderButton(@"%APPDATA%\Sitecore\Sitecore Instance Manager")
                },
                new ButtonDefinition(),
                GetVisualStudioButton(),
                GetPatchButton(),
                GetHotfixButton(),
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Download Sitecore",
                  Image = "/Images/$sm/cloud_download.png, SIM.Tool.Windows",
                  Handler = new Download8Button()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Hosts Editor",
                  Image = "/Images/$sm/modem_earth.png, SIM.Tool.Windows",
                  Handler = new OpenHostsButton()
                },
                new ButtonDefinition
                {
                  Label = "MS SQL Manager",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new DatabaseManagerButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Update Licenses",
                  Image = "/Images/$lg/scroll_refresh.png, SIM.Tool.Windows",
                  Handler = new UpdateLicenseButton()
                },
                new ButtonDefinition
                {
                  Label = "Multi deletion Sitecore 8 and earlier",
                  Image = "/Images/$lg/delete.png, SIM.Tool.Windows",
                  Handler = new MultipleDeletionButton(),
                  Width = "55"
                },
                new ButtonDefinition
                {
                  Label = "Multi deletion Sitecore 9 and later",
                  Image = "/Images/$lg/delete.png, SIM.Tool.Windows",
                  Handler = new MultipleDeletionForSitecore9AndLaterButton(),
                  Width = "55"
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Log Analyzer",
                  Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows",
                  Handler = new OpenLogsButton()
                },
                new ButtonDefinition
                {
                  Label = "SSPG",
                  Image = "/Images/$lg/package_edit.png, SIM.Tool.Windows",
                  Handler = new OpenSSPGButton()
                },
                new ButtonDefinition
                {
                  Label = "Config Builder",
                  Image = "/Images/$sm/wrench.png, SIM.Tool.Windows",
                  Handler = new ConfigBuilderButton()
                },
                new ButtonDefinition
                {
                  Label = "Install Solr",
                  Image = "/Images/$lg/install.png, SIM.Tool.Windows",
                  Handler = new InstallSolrButton()
                },
                new ButtonDefinition
                {
                  Label = "Install MongoDB",
                  Image = "/Images/$lg/scroll_refresh.png, SIM.Tool.Windows",
                  Handler = new InstallMongoDbButton()
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "App",
          Handler = new WindowOnlyGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Settings",
              Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
              Handler = new SettingsButton()
            },
          }
        },
      }
    };

    private static ButtonDefinition GetVisualStudioButton()
    {
      return new ButtonDefinition
      {
        Label = "Visual Studio",
        Image = "/Images/$sm/vs.png, SIM.Tool.Windows",
        Handler = new OpenVisualStudioButton()
      };
    }

    private static ButtonDefinition GetHotfixButton()
    {
      if (!File.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\Sitecore Instance Manager\\pss.txt")) && !Directory.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\NuGet")))
      {
        return null;
      }

      return new ButtonDefinition
      {                
        Label = "Create Hotfix",
        Image = "/Images/$sm/vs.png, SIM.Tool.Windows",
        Handler = new CreateSupportHotfixButton("%APPDATA%\\Sitecore\\HotfixCreator", $"http://dl.sitecore.net/updater/hc/HotfixCreator.application")
      };
    }

    private static ButtonDefinition GetPatchButton()
    {
      if (!File.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\Sitecore Instance Manager\\pss.txt")) && !Directory.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\NuGet")))
      {
        return null;
      }

      return new ButtonDefinition
      {
        Label = "Create Support Patch",
        Image = "/Images/$sm/vs.png, SIM.Tool.Windows",
        Handler = new CreateSupportPatchButton("%APPDATA%\\Sitecore\\PatchCreator", $"http://dl.sitecore.net/updater/pc/PatchCreator.application")
      };
    }

    private static TabDefinition OpenTab { get; } = new TabDefinition
    {
      Name = "Open",
      Groups = new[]
      {
        new GroupDefinition
        {
          Name = "Page",
          Handler = new PageGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Sitecore Client",
              Image = "/Images/$lg/Sitecore.png, SIM.Tool.Windows",
              Handler = new BrowseButton("/sitecore"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Front-end Site",
                  Image = "/Images/$sm/earth2.png, SIM.Tool.Windows",
                  Handler = new BrowseHomePageButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Login",
                  Image = "/Images/$sm/log_out.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore/login")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Desktop",
                  Image = "/Images/$sm/Windows Table.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore/shell")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Content Editor",
                  Image = "/Images/$sm/Pencil.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore/shell/applications/content%20editor")
                },
                new ButtonDefinition
                {
                  Label = "Page Editor",
                  Image = "/Images/$sm/Paint.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/?sc_mode=edit")
                },
                new ButtonDefinition
                {
                  Label = "Launch Pad",
                  Image = "/Images/$sm/startpage.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore/client/Applications/Launch%20Pad")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Google Chrome",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:chrome")
                },
                new ButtonDefinition
                {
                  Label = "Google Chrome (Incognito)",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:chrome:/incognito")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:firefox")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox (Private)",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:firefox:-private-window")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:iexplore")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer (Private)",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:iexplore:-private")
                },
                new ButtonDefinition
                {
                  Label = "Apple Safari",
                  Image = "/Images/$lg/safari.png, SIM.Tool.Windows",
                  Handler = new BrowseButton("/sitecore:safari")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Admin",
                  Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows",
                  Handler = new OpenToolboxButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Log in admin",
              Image = "/Images/$lg/log_in.png, SIM.Tool.Windows",
              Handler = new LoginAdminButton("/sitecore/"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Sitecore Desktop",
                  Image = "/Images/$sm/Windows Table.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore/shell")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Content Editor",
                  Image = "/Images/$sm/Pencil.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore/shell/applications/content%20editor")
                },
                new ButtonDefinition
                {
                  Label = "Page Editor",
                  Image = "/Images/$sm/Paint.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/?sc_mode=edit")
                },
                new ButtonDefinition
                {
                  Label = "Launch Pad",
                  Image = "/Images/$sm/startpage.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore/client/Applications/Launch%20Pad")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Google Chrome",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:chrome")
                },
                new ButtonDefinition
                {
                  Label = "Google Chrome (Incognito)",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:chrome:/incognito")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:firefox")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox (Private)",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:firefox:-private-window")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:iexplore")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer (Private)",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:iexplore:-private")
                },
                new ButtonDefinition
                {
                  Label = "Apple Safari",
                  Image = "/Images/$lg/safari.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("/sitecore:safari")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Admin",
                  Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows",
                  Handler = new OpenToolboxButton("bypass")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Copy URL",
                  Image = "/Images/$lg/astrologer.png, SIM.Tool.Windows",
                  Handler = new LoginAdminButton("$(clipboard)")
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "File System",
          Handler = new FileSystemGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Container Folder",
              Image = "/Images/$lg/folder_open.png, SIM.Tool.Windows",
              Handler = new OpenContainerFolderButton()
            },
            new ButtonDefinition
            {
              Label = "Website Folder",
              Image = "/Images/$lg/folder_open.png, SIM.Tool.Windows",
              Handler = new SitecoreMemberOpenFolderButton("$(website)"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Root Folder",
                  Image = "/Images/$sm/folder_network.png, SIM.Tool.Windows",
                  Handler = new SitecoreMemberOpenFolderButton("$(root)")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Website Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SitecoreMemberOpenFolderButton("$(website)")
                },
                new ButtonDefinition
                {
                  Label = "Data Folder",
                  Image = "/Images/$sm/folders.png, SIM.Tool.Windows",
                  Handler = new OpenFolderButton("$(data)")
                },
                new ButtonDefinition
                {
                  Label = "Databases Folder",
                  Image = "/Images/$sm/folders.png, SIM.Tool.Windows",
                  Handler = new OpenFolderButton("$(root)/Databases")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "bin Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new OpenBinFolderButton("$(website)/bin")
                },
                new ButtonDefinition
                {
                  Label = "App_Config Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SitecoreMemberOpenFolderButton("$(website)/App_Config")
                },
                new ButtonDefinition
                {
                  Label = "Include Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new OpenFolderButton("$(website)/App_Config/Include")
                },
                new ButtonDefinition
                {
                  Label = "zzz Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new OpenFolderButton("$(website)/App_Config/Include/zzz")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Config Files",
              Image = "/Images/$lg/copy.png, SIM.Tool.Windows",
              Handler = new ConfigFilesButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "web.config",
                  Image = "/Images/$sm/document_text.png, SIM.Tool.Windows",
                  Handler = new OpenWebConfigButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Global.asax",
                  Image = "/Images/$sm/document_text.png, SIM.Tool.Windows",
                  Handler = new OpenFileButton("/Global.asax")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "ConnectionStrings.config",
                  Image = "/Images/$sm/data_copy.png, SIM.Tool.Windows",
                  Handler = new OpenFileButton("/App_Config/ConnectionStrings.config")
                },
                new ButtonDefinition
                {
                  Label = "Sitecore.config",
                  Image = "/Images/$sm/data_copy.png, SIM.Tool.Windows",
                  Handler = new OpenFileButton("/App_Config/Sitecore.config")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Showconfig.xml",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new ConfigBuilderButton("/showconfig")
                },
                new ButtonDefinition
                {
                  Label = "Showconfig.xml (normalized)",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new ConfigBuilderButton("/showconfig /normalized")
                },
                new ButtonDefinition
                {
                  Label = "web.config.result.xml",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new ConfigBuilderButton("/webconfigresult")
                },
                new ButtonDefinition
                {
                  Label = "web.config.result.xml (normalized)",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new ConfigBuilderButton("/webconfigresult /normalized")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Log Files",
              Image = "/Images/$lg/history2.png, SIM.Tool.Windows",
              Handler = new LogFilesButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Current Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'Crawling.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("Crawling.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Search.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("Search.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Publishing.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("Publishing.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Client.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("Client.log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'EXM.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new OpenCurrentLogButton("Exm.log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Entire Log files",
                  Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows",
                  Handler = new OpenLogsButton()
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "Apps",
          Handler = new AppsGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Visual Studio",
              Image = "/Images/$lg/vs.png, SIM.Tool.Windows",
              Handler = new OpenVisualStudioButton(),
              Buttons = new[]
              {
                GetVisualStudioButton(),
                GetPatchButton(),
                GetHotfixButton()
              }
            },
          }
        },
      }
    };

    private static TabDefinition EditTab { get; } = new TabDefinition
    {
      Name = "Edit",
      Groups = new[]
      {
        new GroupDefinition
        {
          Name = "Install",
          Handler = new InstallGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Install Packages",
              Image = "/Images/$lg/install.png, SIM.Tool.Windows",
              Handler = new InstallModulesButton()
            },
            new ButtonDefinition
            {
              Label = "Install Packages",
              Image = "/Images/$lg/install.png, SIM.Tool.Windows",
              Handler = new InstallModulesForSitecore9AndLaterButton()
            },
          }
        },
        GetManageGroupDefinition(),
        new GroupDefinition
        {
          Name = "Backup",
          Handler = new BackupGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Backup",
              Image = "/Images/$lg/floppy_disks.png, SIM.Tool.Windows",
              Handler = new BackupInstanceButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Backup",
                  Image = "/Images/$sm/box_into.png, SIM.Tool.Windows",
                  Handler = new BackupInstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Export",
                  Image = "/Images/$lg/download.png, SIM.Tool.Windows",
                  Handler = new ExportInstanceButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Restore",
              Image = "/Images/$lg/box_out.png, SIM.Tool.Windows",
              Handler = new RestoreInstanceButton()
            },
          }
        },
      }
    };

    private static GroupDefinition GetManageGroupDefinition()
    {
      if (!File.Exists(
        Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\Sitecore Instance Manager\\extra.txt")))
      {
        return new GroupDefinition
        {
          Name = "Manage",
          Handler = new ManageGroup(),
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "App State",
              Image = "/Images/$lg/gearwheels.png, SIM.Tool.Windows",
              Handler = new AppStateButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Start",
                  Image = "/Images/$sm/media_play.png, SIM.Tool.Windows",
                  Handler = new ControlAppPoolButton("start")
                },
                new ButtonDefinition
                {
                  Label = "Stop",
                  Image = "/Images/$sm/media_stop.png, SIM.Tool.Windows",
                  Handler = new ControlAppPoolButton("stop")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Recycle",
                  Image = "/Images/$sm/garbage.png, SIM.Tool.Windows",
                  Handler = new ControlAppPoolButton("recycle")
                },
                new ButtonDefinition
                {
                  Label = "Kill process",
                  Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                  Handler = new ControlAppPoolButton("kill")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Change mode",
                  Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                  Handler = new ControlAppPoolButton("mode")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Delete",
              Image = "/Images/$lg/uninstall.png, SIM.Tool.Windows",
              Handler = new DeleteInstanceButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Delete",
                  Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                  Handler = new DeleteInstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Reinstall",
                  Image = "/Images/$sm/redo.png, SIM.Tool.Windows",
                  Handler = new ReinstallInstanceButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Extra Actions",
              Image = "/Images/$lg/atom2.png, SIM.Tool.Windows",
              Handler = new ExtraActionsButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Publish (Incremental)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new PublishButton("incremental")
                },
                new ButtonDefinition
                {
                  Label = "Publish (Smart)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new PublishButton("smart")
                },
                new ButtonDefinition
                {
                  Label = "Publish (Republish)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new PublishButton("republish")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Cleanup logs and temp folder",
                  Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                  Handler = new CleanupInstanceButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach Reporting database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new AttachReportingDatabaseButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach Reporting.Secondary database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new AttachReportingSecondaryDatabaseButton()
                },
                new ButtonDefinition
                {
                  Label = "Copy marketing definition tables",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new CopyMarketingDefinitionTablesButton()
                },
                new ButtonDefinition
                {
                  Label = "Replace Reporting database with Reporting.Secondary",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ReplaceReportingDatabaseWithSecondaryButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Clear Properties of All Databases",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ClearPropertiesButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of All Databases",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ClearEventQueueButton()
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Core Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ClearEventQueueButton("core")
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Master Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ClearEventQueueButton("master")
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Web Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new ClearEventQueueButton("web")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Recycle All But This",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new RecycleAllButThisButton()
                },
                new ButtonDefinition
                {
                  Label = "Kill All w3wp But This",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new KillAllButThisButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach debugger to process",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new AttachDebuggerButton()
                },
                new ButtonDefinition
                {
                  Label = "Collect Memory Dump with MGAD",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new CollectMemoryDumpButton()
                },
                new ButtonDefinition
                {
                  Label = "Trace method call with MAT",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new ManagedArgsTracerButton()
                },
              }
            },
          }
        };
      }

      return new GroupDefinition
      {
        Name = "Manage",
        Handler = new ManageGroup(),
        Buttons = new[]
        {
          new ButtonDefinition
          {
            Label = "App State",
            Image = "/Images/$lg/gearwheels.png, SIM.Tool.Windows",
            Handler = new AppStateButton(),
            Buttons = new[]
            {
              new ButtonDefinition
              {
                Label = "Start",
                Image = "/Images/$sm/media_play.png, SIM.Tool.Windows",
                Handler = new ControlAppPoolButton("start")
              },
              new ButtonDefinition
              {
                Label = "Stop",
                Image = "/Images/$sm/media_stop.png, SIM.Tool.Windows",
                Handler = new ControlAppPoolButton("stop")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Recycle",
                Image = "/Images/$sm/garbage.png, SIM.Tool.Windows",
                Handler = new ControlAppPoolButton("recycle")
              },
              new ButtonDefinition
              {
                Label = "Kill process",
                Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                Handler = new ControlAppPoolButton("kill")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Change mode",
                Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                Handler = new ControlAppPoolButton("mode")
              },
            }
          },
          new ButtonDefinition
          {
            Label = "Delete",
            Image = "/Images/$lg/uninstall.png, SIM.Tool.Windows",
            Handler = new DeleteInstanceButton(),
            Buttons = new[]
            {
              new ButtonDefinition
              {
                Label = "Delete",
                Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                Handler = new DeleteInstanceButton()
              },
              new ButtonDefinition
              {
                Label = "Reinstall",
                Image = "/Images/$sm/redo.png, SIM.Tool.Windows",
                Handler = new ReinstallInstanceButton()
              },
            }
          },
          new ButtonDefinition
          {
            Label = "Extra Actions",
            Image = "/Images/$lg/atom2.png, SIM.Tool.Windows",
            Buttons = new[]
            {
              new ButtonDefinition
              {
                Label = "Publish (Incremental)",
                Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                Handler = new PublishButton("incremental")
              },
              new ButtonDefinition
              {
                Label = "Publish (Smart)",
                Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                Handler = new PublishButton("smart")
              },
              new ButtonDefinition
              {
                Label = "Publish (Republish)",
                Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                Handler = new PublishButton("republish")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Cleanup logs and temp folder",
                Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                Handler = new CleanupInstanceButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach Reporting database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new AttachReportingDatabaseButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach Reporting.Secondary database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new AttachReportingSecondaryDatabaseButton()
              },
              new ButtonDefinition
              {
                Label = "Copy marketing definition tables",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new CopyMarketingDefinitionTablesButton()
              },
              new ButtonDefinition
              {
                Label = "Replace Reporting database with Reporting.Secondary",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ReplaceReportingDatabaseWithSecondaryButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Clear Properties of All Databases",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ClearPropertiesButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Clear Event Queues of All Databases",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ClearEventQueueButton()
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Core Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ClearEventQueueButton("core")
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Master Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ClearEventQueueButton("master")
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Web Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new ClearEventQueueButton("web")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Recycle All But This",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new RecycleAllButThisButton()
              },
              new ButtonDefinition
              {
                Label = "Kill All w3wp But This",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new KillAllButThisButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach debugger to process",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new AttachDebuggerButton()
              },
              new ButtonDefinition
              {
                Label = "Collect Memory Dump with MGAD",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new CollectMemoryDumpButton()
              },
              new ButtonDefinition
              {
                Label = "Trace method call with MAT",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new ManagedArgsTracerButton()
              }
            }
          }
        }
      };
    }

    internal static ButtonDefinition[] MenuItems { get; } = 
    {
      new ButtonDefinition { Label = "Browse Sitecore Container Website", Image = "/Images/$sm/earth2.png, SIM.Tool.Windows", Handler = new BrowseSitecoreContainerWebsiteButton() },
      new ButtonDefinition { Label = "Browse Website", Image = "/Images/$sm/earth2.png, SIM.Tool.Windows", Handler = new BrowseHomePageButton() },
      new ButtonDefinition { Label = "Browse Sitecore Client", Image = "/Images/$sm/Sitecore.png, SIM.Tool.Windows", Handler = new BrowseButton("/sitecore") },
      new ButtonDefinition { Label = "Browse Sitecore Desktop", Image = "/Images/$sm/Windows Table.png, SIM.Tool.Windows", Handler = new BrowseButton("/sitecore/shell") },
      new ButtonDefinition { Label = "Log in admin", Image = "/Images/$sm/log_in.png, SIM.Tool.Windows", Handler = new LoginAdminButton("/sitecore"), Buttons = new [] {
        new ButtonDefinition { Label = "Google Chrome", Image = "/Images/$lg/chrome.png, SIM.Tool.Windows", Handler = new LoginAdminButton("/sitecore:chrome") },
        new ButtonDefinition { Label = "Mozilla Firefox", Image = "/Images/$lg/firefox.png, SIM.Tool.Windows", Handler = new LoginAdminButton("/sitecore:firefox") },
        new ButtonDefinition { Label = "Internet Explorer", Image = "/Images/$lg/ie.png, SIM.Tool.Windows", Handler = new LoginAdminButton("/sitecore:iexplore") },
        new ButtonDefinition { Label = "Apple Safari", Image = "/Images/$lg/safari.png, SIM.Tool.Windows", Handler = new LoginAdminButton("/sitecore:safari") },
        new ButtonDefinition { Handler = new OpenToolboxButton() },
        new ButtonDefinition { Label = "Sitecore Admin", Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows", Handler = new OpenToolboxButton("bypass") },
        new ButtonDefinition { Handler = new LoginAdminButton() },
        new ButtonDefinition { Label = "Copy URL", Image = "/Images/$lg/astrologer.png, SIM.Tool.Windows", Handler = new LoginAdminButton("$(clipboard)") },
      }}, 
      new ButtonDefinition { Label = "Sitecore Admin", Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows", Handler = new OpenToolboxButton() },
      new ButtonDefinition { Label = "Open Folder", Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows", Handler = new SitecoreMemberOpenFolderButton("$(website)") },
      new ButtonDefinition { Label = "Open Container Folder", Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows", Handler = new OpenContainerFolderButton() },
      new ButtonDefinition { Label = "Open Visual Studio", Image = "/Images/$sm/vs.png, SIM.Tool.Windows", Handler = new OpenVisualStudioButton() },
      new ButtonDefinition { Handler = new OpenWebConfigButton() },
      new ButtonDefinition { Label = "Analyze log files", Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows", Handler = new OpenLogsButton() },
      new ButtonDefinition { Label = "Open log file", Image = "/Images/$sm/console.png, SIM.Tool.Windows", Handler = new OpenCurrentLogButton() },
      new ButtonDefinition { Label = "Open web.config file", Image = "/Images/$sm/document_text.png, SIM.Tool.Windows", Handler = new OpenWebConfigButton() },
      new ButtonDefinition { Handler = new PublishButton() },
      new ButtonDefinition { Label = "Publish Site", Image = "/Images/$sm/publish.png, SIM.Tool.Windows", Handler = new PublishButton() },
      new ButtonDefinition { Handler = new BackupInstanceButton() },
      new ButtonDefinition { Label = "Backup", Image = "/Images/$sm/box_into.png, SIM.Tool.Windows", Handler = new BackupInstanceButton() },
      new ButtonDefinition { Label = "Restore", Image = "/Images/$sm/box_out.png, SIM.Tool.Windows", Handler = new RestoreInstanceButton() },
      new ButtonDefinition { Handler = new ExportInstanceButton() },
      new ButtonDefinition { Label = "Export", Image = "/Images/$sm/download.png, SIM.Tool.Windows", Handler = new ExportInstanceButton() },
      new ButtonDefinition { Handler = new InstallModulesButton() },
      new ButtonDefinition { Label = "Install packages", Image = "/Images/$sm/install.png, SIM.Tool.Windows", Handler = new InstallModulesButton() },
      new ButtonDefinition { Handler = new InstallModulesButton() },
      new ButtonDefinition { Label = "Install packages", Image = "/Images/$sm/install.png, SIM.Tool.Windows", Handler = new InstallModulesForSitecore9AndLaterButton() },
      new ButtonDefinition { Handler = new ReinstallInstanceButton() },
      new ButtonDefinition { Label = "Reinstall instance", Image = "/Images/$sm/redo.png, SIM.Tool.Windows", Handler = new ReinstallInstanceButton() },
      new ButtonDefinition { Handler = new DeleteInstanceButton() },
      new ButtonDefinition { Label = "Delete", Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows", Handler = new DeleteInstanceButton() },
    };

    public static TabDefinition[] Tabs { get; } = {
      HomeTab,
      OpenTab,
      EditTab
    };
  }
}
