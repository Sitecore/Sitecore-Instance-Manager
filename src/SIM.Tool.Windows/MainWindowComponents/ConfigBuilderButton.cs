namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Core;

  [UsedImplicitly]
  public class ConfigBuilderButton : AbstractDownloadAndRunButton, IMainWindowButton
  {
    #region Fields

    protected readonly bool Normalize;
    protected readonly bool Showconfig;
    protected readonly bool WebConfigResult;

    #endregion

    #region Constructors

    public ConfigBuilderButton()
    {
      this.Normalize = false;
      this.WebConfigResult = false;
      this.Showconfig = false;
    }

    public ConfigBuilderButton(string param)
    {
      this.Normalize = param.ContainsIgnoreCase("/normalize");
      this.Showconfig = param.ContainsIgnoreCase("/showconfig");
      this.WebConfigResult = param.ContainsIgnoreCase("/webconfigresult");
      Assert.IsTrue(!(this.Showconfig && this.WebConfigResult), "/showconfig and /webconfigresult params must not be used together");
    }

    #endregion

    protected override string BaseUrl
    {
      get
      {
        return "http://dl.sitecore.net/updater/1.1/scb/";
      }
    }

    protected override string AppName
    {
      get
      {
        return "Config Builder";
      }
    }

    protected override string ExecutableName
    {
      get
      {
        return "Sitecore.ConfigBuilder.Tool.exe";
      }
    }

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (!this.Showconfig && !this.WebConfigResult)
      {
        var param = instance != null ? Path.Combine(instance.WebRootPath, "web.config") : null;
        RunApp(mainWindow, param);

        return;
      }

      Assert.IsNotNull(instance, "instance");

      var folder = Path.Combine(ApplicationManager.TempFolder, "configs", instance.Name);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      string path;
      if (this.Showconfig)
      {
        path = Path.Combine(folder, "showconfig.xml");
      }
      else if (this.WebConfigResult)
      {
        path = Path.Combine(folder, "web.config.result.xml");
      }
      else
      {
        throw new NotSupportedException("This is not supported");
      }

      if (this.Normalize)
      {
        path = Path.Combine(Path.GetDirectoryName(path), "norm." + Path.GetFileName(path));
      }

      if (this.Showconfig)
      {
        instance.GetShowconfig(this.Normalize).Save(path);
      }
      else if (this.WebConfigResult)
      {
        instance.GetWebResultConfig(this.Normalize).Save(path);
      }
      else
      {
        throw new NotImplementedException("This is not supported #2");
      }

      CoreApp.OpenFile(path);
    }

    #endregion
  }
}