namespace SIM.Tool.Windows
{
  using SIM.Tool.Base.Plugins;

  public class ButtonDefinition
  {
    public IMainWindowButton Handler { get; set; }

    public string Label { get; set; }

    public string Image { get; set; }

    public ButtonDefinition[] Buttons { get; set; }

    public string Width { get; set; }
  }
}