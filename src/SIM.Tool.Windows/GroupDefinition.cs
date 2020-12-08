using SIM.Tool.Base.Plugins;

namespace SIM.Tool.Windows
{
  public class GroupDefinition
  {
    public IMainWindowGroup Handler { get; set; }

    public ButtonDefinition[] Buttons { get; set; }

    public string Name { get; set; }
  }
}