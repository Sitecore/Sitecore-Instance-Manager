namespace SIM.Tool.Windows.MainWindowComponents
{
  using System;
  using System.IO;
  using System.Windows;
  using SIM.Instances;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Core;
  using SIM.Extensions;
  using SIM.Tool.Base.Plugins;

  [UsedImplicitly]
  public class ConfigBuilderButton : IMainWindowButton
  {
    #region Fields

    protected bool Normalize { get; }
    protected bool Showconfig { get; }
    protected bool WebConfigResult { get; }

    #endregion

    #region Constructors

    public ConfigBuilderButton()
    {
      Normalize = false;
      WebConfigResult = false;
      Showconfig = false;
    }

    public ConfigBuilderButton(string param)
    {
      Normalize = param.ContainsIgnoreCase("/normalize");
      Showconfig = param.ContainsIgnoreCase("/showconfig");
      WebConfigResult = param.ContainsIgnoreCase("/webconfigresult");
      Assert.IsTrue(!(Showconfig && WebConfigResult), "/showconfig and /webconfigresult params must not be used together");
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public bool IsVisible(Window mainWindow, Instance instance)
    {
      return MainWindowHelper.IsEnabledOrVisibleButtonForSitecoreMember(instance);
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (!Showconfig && !WebConfigResult)
      {
        CoreApp.RunApp("iexplore", "http://dl.sitecore.net/updater/scb/Sitecore.ConfigBuilder.Tool.application");

        return;
      }

      Assert.IsNotNull(instance, nameof(instance));

      var folder = Path.Combine(ApplicationManager.TempFolder, "configs", instance.Name);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      string path;
      if (Showconfig)
      {
        path = Path.Combine(folder, "showconfig.xml");
      }
      else if (WebConfigResult)
      {
        path = Path.Combine(folder, "web.config.result.xml");
      }
      else
      {
        throw new NotSupportedException("This is not supported");
      }

      if (Normalize)
      {
        path = Path.Combine(Path.GetDirectoryName(path), $"norm.{Path.GetFileName(path)}");
      }

      if (Showconfig)
      {
        instance.GetShowconfig(Normalize).Save(path);
      }
      else if (WebConfigResult)
      {
        instance.GetWebResultConfig(Normalize).Save(path);
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