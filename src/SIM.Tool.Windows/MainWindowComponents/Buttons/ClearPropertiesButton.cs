namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class ClearPropertiesButton : ClearDatabasesButton
  {
    public ClearPropertiesButton()
      : this("")
    {
    }

    public ClearPropertiesButton(string databases)
      : base(databases)
    {      
    }

    public override string DatabaseName { get; } = "Properties";
  }
}