namespace SIM.Core.Commands
{
  using System.IO;
  using Sitecore.Diagnostics.Base;
  using SIM.Core.Common;
  using SIM.Extensions.DirectoryInfo;
  using SIM.Extensions.FileInfo;

  public class CreatePipeCommand : AbstractCommand<string>
  {
    public virtual string Name { get; set; }

    public virtual bool? Force { get; set; }

    protected override void DoExecute(CommandResult<string> result)
    {
      Assert.ArgumentNotNull(Name, nameof(Name));

      var dir = new DirectoryInfo(".sim");
      if (!dir.Exists)
      {
        dir.Create();
        dir.Attributes |= FileAttributes.Hidden;
      }

      var file = dir.GetFile(Name + ".pipe");

      if (Force == true && file.Exists)
      {
        file.Delete();
      }

      Ensure.IsTrue(!file.Exists || file.GetAllText()?.Trim() == "pipe::disposed", "The pipe already exists");

      file.WriteAllText("pipe::created");
    }
  }
}