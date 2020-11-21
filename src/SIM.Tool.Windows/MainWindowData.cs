namespace SIM.Tool.Windows
{
  using System;
  using System.IO;

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
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Refresh sites",
              Image = "/Images/$lg/refresh.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.RefreshButton("sites"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Refresh sites",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.RefreshButton("sites")
                },
                new ButtonDefinition
                {
                  Label = "Refresh installer",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.RefreshButton("installer")
                },
                new ButtonDefinition
                {
                  Label = "Refresh caches",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.RefreshButton("caches")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Refresh all",
                  Image = "/Images/$sm/refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.RefreshButton("all")
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "Install",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Install SC XP 9",
              Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.Install9InstanceButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Install Sitecore XP 9 and later",
                  Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.Install9InstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Install Sitecore XP 8.2 and earlier",
                  Image = "/Images/$lg/add_domain.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.InstallInstanceButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Import Solution",
              Image = "/Images/$lg/upload.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.ImportInstanceButton()
            }
          }
        },
        new GroupDefinition
        {
          Name = "Tools",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Get Sitecore",
              Image = "/Images/$lg/cloud_download.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.Download8Button()
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
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton(@"%APPDATA%\Sitecore\Sitecore Instance Manager\Logs")
                },
                new ButtonDefinition
                {
                  Label = "SIM Data Folder",
                  Image = "/Images/$sm/folder_window.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton(@"%APPDATA%\Sitecore\Sitecore Instance Manager")
                },
                new ButtonDefinition(),
                GetPatchButton(),
                GetHotfixButton(),
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Download Sitecore",
                  Image = "/Images/$sm/cloud_download.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.Download8Button()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Hosts Editor",
                  Image = "/Images/$sm/modem_earth.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenHostsButton()
                },
                new ButtonDefinition
                {
                  Label = "MS SQL Manager",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.DatabaseManagerButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Update Licenses",
                  Image = "/Images/$lg/scroll_refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.UpdateLicenseButton()
                },
                new ButtonDefinition
                {
                  Label = "Multiple Deletion",
                  Image = "/Images/$lg/delete.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.MultipleDeletionButton(),
                  Width = "55"
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Log Analyzer",
                  Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenLogsButton()
                },
                new ButtonDefinition
                {
                  Label = "SSPG",
                  Image = "/Images/$lg/package_edit.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenSSPGButton()
                },
                new ButtonDefinition
                {
                  Label = "Config Builder",
                  Image = "/Images/$sm/wrench.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ConfigBuilderButton()
                },
                new ButtonDefinition
                {
                  Label = "Install Solr",
                  Image = "/Images/$lg/install.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.InstallSolrButton()
                },
                new ButtonDefinition
                {
                  Label = "Install MongoDB",
                  Image = "/Images/$lg/scroll_refresh.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.InstallMongoDbButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Generate NuGet packages",
                  Image = "/Images/$sm/new_package.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.GenerateNuGetPackagesButton()
                },
                new ButtonDefinition
                {
                  Label = "Generate NuGet packages for selected instance",
                  Image = "/Images/$sm/new_package.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.GenerateNuGetPackagesButton("instance")
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "App",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Settings",
              Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.SettingsButton()
            },
          }
        },
      }
    };

    private static ButtonDefinition GetHotfixButton()
    {
      if (!File.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\Sitecore Instance Manager\\pss.txt")) && !Directory.Exists(Environment.ExpandEnvironmentVariables("%PROGRAMDATA%\\Sitecore\\NuGet")))
      {
        return null;
      }

      return new ButtonDefinition
      {                
        Label = "Create Core Patch",
        Image = "/Images/$sm/vs.png, SIM.Tool.Windows",
        Handler = new SIM.Tool.Windows.MainWindowComponents.CreateSupportPatchButton("%APPDATA%\\Sitecore\\HotfixCreator", $"http://dl.sitecore.net/updater/hc/HotfixCreator.application")
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
        Handler = new SIM.Tool.Windows.MainWindowComponents.CreateSupportPatchButton("%APPDATA%\\Sitecore\\PatchCreator", $"http://dl.sitecore.net/updater/pc/PatchCreator.application")
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
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Sitecore Client",
              Image = "/Images/$lg/Sitecore.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Front-end Site",
                  Image = "/Images/$sm/earth2.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Login",
                  Image = "/Images/$sm/log_out.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore/login")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Desktop",
                  Image = "/Images/$sm/Windows Table.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore/shell")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Content Editor",
                  Image = "/Images/$sm/Pencil.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore/shell/applications/content%20editor")
                },
                new ButtonDefinition
                {
                  Label = "Page Editor",
                  Image = "/Images/$sm/Paint.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/?sc_mode=edit")
                },
                new ButtonDefinition
                {
                  Label = "Launch Pad",
                  Image = "/Images/$sm/startpage.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore/client/Applications/Launch%20Pad")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Google Chrome",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:chrome")
                },
                new ButtonDefinition
                {
                  Label = "Google Chrome (Incognito)",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:chrome:/incognito")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:firefox")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox (Private)",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:firefox:-private-window")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:iexplore")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer (Private)",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:iexplore:-private")
                },
                new ButtonDefinition
                {
                  Label = "Apple Safari",
                  Image = "/Images/$lg/safari.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore:safari")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Admin",
                  Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenToolboxButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Log in admin",
              Image = "/Images/$lg/log_in.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore/"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Sitecore Desktop",
                  Image = "/Images/$sm/Windows Table.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore/shell")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Content Editor",
                  Image = "/Images/$sm/Pencil.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore/shell/applications/content%20editor")
                },
                new ButtonDefinition
                {
                  Label = "Page Editor",
                  Image = "/Images/$sm/Paint.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/?sc_mode=edit")
                },
                new ButtonDefinition
                {
                  Label = "Launch Pad",
                  Image = "/Images/$sm/startpage.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore/client/Applications/Launch%20Pad")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Google Chrome",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:chrome")
                },
                new ButtonDefinition
                {
                  Label = "Google Chrome (Incognito)",
                  Image = "/Images/$lg/chrome.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:chrome:/incognito")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:firefox")
                },
                new ButtonDefinition
                {
                  Label = "Mozilla Firefox (Private)",
                  Image = "/Images/$lg/firefox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:firefox:-private-window")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:iexplore")
                },
                new ButtonDefinition
                {
                  Label = "Internet Explorer (Private)",
                  Image = "/Images/$lg/ie.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:iexplore:-private")
                },
                new ButtonDefinition
                {
                  Label = "Apple Safari",
                  Image = "/Images/$lg/safari.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:safari")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Sitecore Admin",
                  Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenToolboxButton("bypass")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Copy URL",
                  Image = "/Images/$lg/astrologer.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("$(clipboard)")
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "File System",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Website Folder",
              Image = "/Images/$lg/folder_open.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton("$(website)"),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Root Folder",
                  Image = "/Images/$sm/folder_network.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton("$(root)")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Website Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton("$(website)")
                },
                new ButtonDefinition
                {
                  Label = "Data Folder",
                  Image = "/Images/$sm/folders.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberOpenFolderButton("$(data)")
                },
                new ButtonDefinition
                {
                  Label = "Databases Folder",
                  Image = "/Images/$sm/folders.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberOpenFolderButton("$(root)/Databases")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "App_Config Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton("$(website)/App_Config")
                },
                new ButtonDefinition
                {
                  Label = "Include Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberOpenFolderButton("$(website)/App_Config/Include")
                },
                new ButtonDefinition
                {
                  Label = "zzz Folder",
                  Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberOpenFolderButton("$(website)/App_Config/Include/zzz")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Config Files",
              Image = "/Images/$lg/copy.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.OpenWebConfigButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "web.config",
                  Image = "/Images/$sm/document_text.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenWebConfigButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Global.asax",
                  Image = "/Images/$sm/document_text.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFileButton("/Global.asax")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "ConnectionStrings.config",
                  Image = "/Images/$sm/data_copy.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFileButton("/App_Config/ConnectionStrings.config")
                },
                new ButtonDefinition
                {
                  Label = "Sitecore.config",
                  Image = "/Images/$sm/data_copy.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFileButton("/App_Config/Sitecore.config")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Showconfig.xml",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ConfigBuilderButton("/showconfig")
                },
                new ButtonDefinition
                {
                  Label = "Showconfig.xml (normalized)",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ConfigBuilderButton("/showconfig /normalized")
                },
                new ButtonDefinition
                {
                  Label = "web.config.result.xml",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ConfigBuilderButton("/webconfigresult")
                },
                new ButtonDefinition
                {
                  Label = "web.config.result.xml (normalized)",
                  Image = "/Images/$sm/document_gear.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ConfigBuilderButton("/webconfigresult /normalized")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Log Files",
              Image = "/Images/$lg/history2.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Current Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'Crawling.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("Crawling.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Search.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("Search.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Publishing.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("Publishing.log")
                },
                new ButtonDefinition
                {
                  Label = "Current 'Client.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("Client.log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Current 'EXM.log' Log file",
                  Image = "/Images/$sm/console.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton("Exm.log")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Entire Log files",
                  Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenLogsButton()
                },
              }
            },
          }
        },
        new GroupDefinition
        {
          Name = "Apps",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Visual Studio",
              Image = "/Images/$lg/vs.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.OpenVisualStudioButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Visual Studio",
                  Image = "/Images/$sm/vs.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.OpenVisualStudioButton()
                },
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
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Install Packages",
              Image = "/Images/$lg/install.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.InstallModulesButton()
            },
            new ButtonDefinition
            {
              Label = "Publishing Service",
              Image = "/Images/$lg/install.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.InstallSPSButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Install",
                  Image = "/Images/$lg/install.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.InstallSPSButton(),
                },
                new ButtonDefinition
                {
                  Label = "Uninstall",
                  Image = "/Images/$lg/uninstall.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.UninstallSPSButton(),
                }
              }
            },
          }
        },
        GetManageGroupDefinition(),
        new GroupDefinition
        {
          Name = "Backup",
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "Backup",
              Image = "/Images/$lg/floppy_disks.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.BackupButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Backup",
                  Image = "/Images/$sm/box_into.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.BackupInstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Export",
                  Image = "/Images/$lg/download.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ExportInstanceButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Restore",
              Image = "/Images/$lg/box_out.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.RestoreInstanceButton()
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
          Buttons = new[]
          {
            new ButtonDefinition
            {
              Label = "App State",
              Image = "/Images/$lg/gearwheels.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Start",
                  Image = "/Images/$sm/media_play.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("start")
                },
                new ButtonDefinition
                {
                  Label = "Stop",
                  Image = "/Images/$sm/media_stop.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("stop")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Recycle",
                  Image = "/Images/$sm/garbage.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("recycle")
                },
                new ButtonDefinition
                {
                  Label = "Kill process",
                  Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("kill")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Change mode",
                  Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("mode")
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Delete",
              Image = "/Images/$lg/uninstall.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.DeleteInstanceButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Delete",
                  Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.DeleteInstanceButton()
                },
                new ButtonDefinition
                {
                  Label = "Reinstall",
                  Image = "/Images/$sm/redo.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ReinstallInstanceButton()
                },
              }
            },
            new ButtonDefinition
            {
              Label = "Extra Actions",
              Image = "/Images/$lg/atom2.png, SIM.Tool.Windows",
              Handler = new SIM.Tool.Windows.MainWindowComponents.ExtraActionsButton(),
              Buttons = new[]
              {
                new ButtonDefinition
                {
                  Label = "Publish (Incremental)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("incremental")
                },
                new ButtonDefinition
                {
                  Label = "Publish (Smart)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("smart")
                },
                new ButtonDefinition
                {
                  Label = "Publish (Republish)",
                  Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("republish")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Cleanup logs and temp folder",
                  Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.CleanupInstanceButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach Reporting database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.AttachReportingDatabaseButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach Reporting.Secondary database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.AttachReportingSecondaryDatabaseButton()
                },
                new ButtonDefinition
                {
                  Label = "Copy marketing definition tables",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.CopyMarketingDefinitionTablesButton()
                },
                new ButtonDefinition
                {
                  Label = "Replace Reporting database with Reporting.Secondary",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ReplaceReportingDatabaseWithSecondaryButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Clear Properties of All Databases",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ClearPropertiesButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of All Databases",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton()
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Core Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("core")
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Master Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("master")
                },
                new ButtonDefinition
                {
                  Label = "Clear Event Queues of Web Database",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("web")
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Recycle All But This",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.RecycleAllButThisButton()
                },
                new ButtonDefinition
                {
                  Label = "Kill All w3wp But This",
                  Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.KillAllButThisButton()
                },
                new ButtonDefinition(),
                new ButtonDefinition
                {
                  Label = "Attach debugger to process",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.AttachDebuggerButton()
                },
                new ButtonDefinition
                {
                  Label = "Collect Memory Dump with MGAD",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.CollectMemoryDumpButton()
                },
                new ButtonDefinition
                {
                  Label = "Trace method call with MAT",
                  Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                  Handler = new SIM.Tool.Windows.MainWindowComponents.ManagedArgsTracerButton()
                },
              }
            },
          }
        };
      }

      return new GroupDefinition
      {
        Name = "Manage",
        Buttons = new[]
        {
          new ButtonDefinition
          {
            Label = "App State",
            Image = "/Images/$lg/gearwheels.png, SIM.Tool.Windows",
            Handler = new SIM.Tool.Windows.MainWindowComponents.SitecoreMemberButton(),
            Buttons = new[]
            {
              new ButtonDefinition
              {
                Label = "Start",
                Image = "/Images/$sm/media_play.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("start")
              },
              new ButtonDefinition
              {
                Label = "Stop",
                Image = "/Images/$sm/media_stop.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("stop")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Recycle",
                Image = "/Images/$sm/garbage.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("recycle")
              },
              new ButtonDefinition
              {
                Label = "Kill process",
                Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("kill")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Change mode",
                Image = "/Images/$sm/cpu.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ControlAppPoolButton("mode")
              },
            }
          },
          new ButtonDefinition
          {
            Label = "Delete",
            Image = "/Images/$lg/uninstall.png, SIM.Tool.Windows",
            Handler = new SIM.Tool.Windows.MainWindowComponents.DeleteInstanceButton(),
            Buttons = new[]
            {
              new ButtonDefinition
              {
                Label = "Delete",
                Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.DeleteInstanceButton()
              },
              new ButtonDefinition
              {
                Label = "Reinstall",
                Image = "/Images/$sm/redo.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ReinstallInstanceButton()
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
                Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("incremental")
              },
              new ButtonDefinition
              {
                Label = "Publish (Smart)",
                Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("smart")
              },
              new ButtonDefinition
              {
                Label = "Publish (Republish)",
                Image = "/Images/$sm/publish.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton("republish")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Cleanup logs and temp folder",
                Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.CleanupInstanceButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach Reporting database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.AttachReportingDatabaseButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach Reporting.Secondary database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.AttachReportingSecondaryDatabaseButton()
              },
              new ButtonDefinition
              {
                Label = "Copy marketing definition tables",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.CopyMarketingDefinitionTablesButton()
              },
              new ButtonDefinition
              {
                Label = "Replace Reporting database with Reporting.Secondary",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ReplaceReportingDatabaseWithSecondaryButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Clear Properties of All Databases",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ClearPropertiesButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Clear Event Queues of All Databases",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton()
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Core Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("core")
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Master Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("master")
              },
              new ButtonDefinition
              {
                Label = "Clear Event Queues of Web Database",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ClearEventQueueButton("web")
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Recycle All But This",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.RecycleAllButThisButton()
              },
              new ButtonDefinition
              {
                Label = "Kill All w3wp But This",
                Image = "/Images/$sm/data.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.KillAllButThisButton()
              },
              new ButtonDefinition(),
              new ButtonDefinition
              {
                Label = "Attach debugger to process",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.AttachDebuggerButton()
              },
              new ButtonDefinition
              {
                Label = "Collect Memory Dump with MGAD",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.CollectMemoryDumpButton()
              },
              new ButtonDefinition
              {
                Label = "Trace method call with MAT",
                Image = "/Images/$lg/wrench.png, SIM.Tool.Windows",
                Handler = new SIM.Tool.Windows.MainWindowComponents.ManagedArgsTracerButton()
              }
            }
          }
        }
      };
    }

    internal static ButtonDefinition[] MenuItems { get; } = 
    {
      new ButtonDefinition { Label = "Browse Sitecore Website", Image = "/Images/$sm/earth2.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton() },
      new ButtonDefinition { Label = "Browse Sitecore Client", Image = "/Images/$sm/Sitecore.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.BrowseButton("/sitecore") },
      new ButtonDefinition { Label = "Log in admin", Image = "/Images/$sm/log_in.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore"), Buttons = new [] {
        new ButtonDefinition { Label = "Google Chrome", Image = "/Images/$lg/chrome.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:chrome") },
        new ButtonDefinition { Label = "Mozilla Firefox", Image = "/Images/$lg/firefox.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:firefox") },
        new ButtonDefinition { Label = "Internet Explorer", Image = "/Images/$lg/ie.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:iexplore") },
        new ButtonDefinition { Label = "Apple Safari", Image = "/Images/$lg/safari.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("/sitecore:safari") },
        new ButtonDefinition(),
        new ButtonDefinition { Label = "Sitecore Admin", Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenToolboxButton("bypass") },
        new ButtonDefinition(),
        new ButtonDefinition { Label = "Copy URL", Image = "/Images/$lg/astrologer.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.LoginAdminButton("$(clipboard)") },
      }}, 
      new ButtonDefinition { Label = "Sitecore Admin", Image = "/Images/$lg/toolbox.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenToolboxButton() },
      new ButtonDefinition { Label = "Open Folder", Image = "/Images/$sm/folder_open.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenFolderButton("$(website)") },
      new ButtonDefinition { Label = "Open Visual Studio", Image = "/Images/$sm/vs.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenVisualStudioButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Analyze log files", Image = "/Images/$lg/zoom_vertical.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenLogsButton() },
      new ButtonDefinition { Label = "Open log file", Image = "/Images/$sm/console.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenCurrentLogButton() },
      new ButtonDefinition { Label = "Open web.config file", Image = "/Images/$sm/document_text.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.OpenWebConfigButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Publish Site", Image = "/Images/$sm/publish.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.PublishButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Backup", Image = "/Images/$sm/box_into.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.BackupInstanceButton() },
      new ButtonDefinition { Label = "Restore", Image = "/Images/$sm/box_out.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.RestoreInstanceButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Export", Image = "/Images/$sm/download.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.ExportInstanceButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Install modules", Image = "/Images/$sm/install.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.InstallModulesButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Reinstall instance", Image = "/Images/$sm/redo.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.ReinstallInstanceButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Delete", Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.DeleteInstanceButton() },
      new ButtonDefinition(),
      new ButtonDefinition { Label = "Install Publishing Service", Image = "/Images/$sm/install.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.InstallSPSButton() },
      new ButtonDefinition { Label = "Uninstall Publishing Service", Image = "/Images/$sm/uninstall.png, SIM.Tool.Windows", Handler = new SIM.Tool.Windows.MainWindowComponents.UninstallSPSButton() },
    };

    public static TabDefinition[] Tabs { get; } = {
      HomeTab,
      OpenTab,
      EditTab
    };
  }
}
