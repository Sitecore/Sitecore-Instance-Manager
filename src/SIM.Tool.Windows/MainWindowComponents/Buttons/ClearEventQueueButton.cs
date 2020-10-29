namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class ClearEventQueueButton : ClearDatabasesButton
  {
    public ClearEventQueueButton()
      : this("")
    {
    }

    public ClearEventQueueButton(string databases)
      : base(databases)
    {      
    }

    public override string DatabaseName { get; } = "EventQueue";
  }
}
