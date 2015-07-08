using System.Windows;

namespace SIM.Tool.Base.Plugins
{
  public interface IMainWindowLoadedProcessor
  {
    void Process(Window mainWindow);
  }
}
