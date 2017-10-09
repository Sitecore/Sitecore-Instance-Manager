namespace SIM.FileSystem.AutoTests
{
  using SIM.IO.Real;
  using Xunit;

  public class RealZipFileTests 
  {
    private RealFileSystem FileSystem { get; } = new RealFileSystem();
    
    [Fact]
    public void Entries_Contains_File_Backslash_7Zip()
    {
      // arrange
      var zip = new RealZipFile(FileSystem.ParseFile("data\\7zip-zip.zip"));
      
      // act
      var result = zip.Entries.Contains("Folder\\File.txt");

      // assert
      Assert.Equal(true, result);
    }

    [Fact]
    public void Entries_Contains_File_Backslash_WindowsZip()
    {
      // arrange
      var zip = new RealZipFile(FileSystem.ParseFile("data\\windows-zip.zip"));

      // act
      var result = zip.Entries.Contains("Folder\\File.txt");

      // assert
      Assert.Equal(true, result);
    }
    [Fact]
    public void Entries_Contains_File_Slash_7Zip()
    {
      // arrange
      var zip = new RealZipFile(FileSystem.ParseFile("data\\7zip-zip.zip"));

      // act
      var result = zip.Entries.Contains("Folder/File.txt");

      // assert
      Assert.Equal(true, result);
    }

    [Fact]
    public void Entries_Contains_File_Slash_WindowsZip()
    {
      // arrange
      var zip = new RealZipFile(FileSystem.ParseFile("data\\windows-zip.zip"));

      // act
      var result = zip.Entries.Contains("Folder/File.txt");

      // assert
      Assert.Equal(true, result);
    }
  }
}