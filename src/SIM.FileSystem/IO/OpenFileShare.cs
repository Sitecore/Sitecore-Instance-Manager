namespace SIM.IO
{
  public enum OpenFileShare
  {
    None = 0,
    Read = 1,
    Write = 2,
    ReadWrite = Write | Read,
    Delete = 4,
    Inheritable = 16,
  }
}