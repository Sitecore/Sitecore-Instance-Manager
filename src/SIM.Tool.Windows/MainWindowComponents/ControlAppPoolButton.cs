namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Windows;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  [UsedImplicitly]
  public class ControlAppPoolButton : IMainWindowButton
  {
    #region Constants

    protected const string Label32Bit = "32bit";
    protected const string LabelCurrent = "(current)";

    #endregion

    #region Fields

    protected readonly bool ChangeMode;
    protected readonly bool KillMode;
    protected readonly bool RecycleMode;

    protected readonly bool StartMode;
    protected readonly bool StopMode;

    #endregion

    // protected readonly bool FavoriteMode;
    // protected readonly bool DisabledMode;
    #region Constructors

    public ControlAppPoolButton(string param)
    {
      Assert.IsNotNullOrEmpty(param, "param");

      switch (param.ToLowerInvariant())
      {
        case "start":
          this.StartMode = true;
          return;
        case "stop":
          this.StopMode = true;
          return;
        case "recycle":
          this.RecycleMode = true;
          return;
        case "kill":
          this.KillMode = true;
          return;
        case "mode":
        case "change":
          this.ChangeMode = true;
          return;

          // case "favorite":
          // this.FavoriteMode = true;
          // return;
          // case "disabled":
          // this.DisabledMode = true;
          // return;
        default:
          Assert.IsTrue(false, "The {0} mode is not supported".FormatWith(param));
          return;
      }
    }

    #endregion

    #region Public methods

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      if (instance == null)
      {
        return false;
      }

      // performance optimization
      // if (this.StartMode && (instance.ApplicationPoolState == ObjectState.Started || instance.ApplicationPoolState == ObjectState.Starting))
      // {
      // return false;
      // }

      // if ((this.StopMode || this.KillMode || this.RecycleMode) && (instance.ApplicationPoolState == ObjectState.Stopped || instance.ApplicationPoolState == ObjectState.Stopping))
      // {
      // return false;
      // }

      // if ((this.KillMode || this.RecycleMode) && !instance.ProcessIds.Any())
      // {
      // return false;
      // }
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (instance != null)
      {
        if (this.StopMode)
        {
          instance.Stop();
          return;
        }

        if (this.StartMode)
        {
          instance.Start();
          return;
        }

        if (this.RecycleMode)
        {
          instance.Recycle();
          return;
        }

        if (this.KillMode)
        {
          MainWindowHelper.KillProcess(instance);
          return;
        }

        if (this.ChangeMode)
        {
          this.DoChangeMode(mainWindow, instance);
          return;
        }

        // if (this.FavoriteMode)
        // {
        // InstanceHelperEx.ToggleFavorite(mainWindow, instance);
        // return;
        // }

        // if (this.DisabledMode)
        // {
        // instance.IsDisabled = !instance.IsDisabled;
        // return;
        // }
        Assert.IsTrue(false, "Impossible");
      }
    }

    #endregion

    #region Private methods

    private void DoChangeMode(Window mainWindow, Instance instance)
    {
      var title = "Change App Pool Mode";
      var header = title;
      var message = "Change {0} instance's Application Pool mode".FormatWith(instance.Name);
      var options = new[]
      {
        this.GetLabel(instance, 2, false), 
        this.GetLabel(instance, 2, true), 
        this.GetLabel(instance, 4, false), 
        this.GetLabel(instance, 4, true)
      };

      var result = WindowHelper.AskForSelection(title, header, message, options, mainWindow);
      if (result == null)
      {
        return;
      }

      if (result.Contains(LabelCurrent))
      {
        return;
      }

      instance.SetAppPoolMode(result.Contains("4.0"), result.Contains(Label32Bit));
    }

    private string GetLabel(Instance instance, int version, bool is32bit)
    {
      var label = version + ".0 ";
      if (is32bit)
      {
        label += " " + Label32Bit;
      }

      if (instance.Is32Bit == is32bit && instance.IsNetFramework4 == (version == 4))
      {
        return label + " " + LabelCurrent;
      }

      return label;
    }

    #endregion
  }
}