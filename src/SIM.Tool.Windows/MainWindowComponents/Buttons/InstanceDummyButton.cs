using System.Windows;
using JetBrains.Annotations;
using SIM.Instances;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  // This button is needed only for visualization in UI without performing any actions
  [UsedImplicitly]
  public class InstanceDummyButton : InstanceOnlyButton
  {
    // This is dummy button, so it doesn't contain any code in the "OnClick" method
    public override void OnClick(Window mainWindow, Instance instance)
    {
    }
  }
}
