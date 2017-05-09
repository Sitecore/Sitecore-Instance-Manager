namespace SIM.FileSystem.AutoTests
{
  using System;
  using System.IO;
  using SIM.IO;
  using SIM.IO.Real;
  using Xunit;

  public class RealFileTests : IDisposable
  {
    private DirectoryInfo Root { get; } = new DirectoryInfo($"C:\\TMP\\{Guid.NewGuid():N}");

    private RealFileSystem FileSystem { get; } = new RealFileSystem();

    public void Dispose()
    {
      Root.Delete(true);
    }

    private DirectoryInfo CreateUniqueDir(DirectoryInfo dir = null)
    {
      return (dir ?? Root).CreateSubdirectory(Guid.NewGuid().ToString("N"));
    }

    private FileInfo CreateUniqueFile(DirectoryInfo dir = null, string contents = null)
    {
      return CreateFile(dir ?? Root, Guid.NewGuid().ToString("N"), contents);
    }

    private FileInfo CreateFile(DirectoryInfo dir, string fileName, string contents = null)
    {
      dir.Create();
      var path = Path.Combine(dir.FullName, fileName);
      File.WriteAllText(path, contents ?? "");

      return new FileInfo(path);
    }

    [Fact]
    public void Ctor_NoStateChange_Exists()
    {
      // arrange
      var contents = new Guid().ToString("N");
      var file = CreateUniqueFile(null, contents);
      var before = file.Exists;

      // act
      FileSystem.ParseFile(file);

      // assert
      var after = file.Exists;
      Assert.True(before == after);
      Assert.Equal(contents, File.ReadAllText(file.FullName));
    }

    [Fact]
    public void Ctor_NoStateChange_NotExists()
    {
      // arrange
      var file = CreateUniqueFile();
      file.Delete();

      var before = file.Exists;

      // act
      FileSystem.ParseFile(file);

      // assert
      var after = file.Exists;
      Assert.True(before == after);
    }

    [Fact]
    public void FullName()
    {
      // arrange
      var file = CreateUniqueFile();
      var expected = file.FullName;

      var sut = FileSystem.ParseFile(file);

      // act
      var name = sut.FullName;

      // assert
      Assert.Equal(expected, name);
    }

    [Fact]
    public void Name()
    {
      // arrange
      var file = CreateUniqueFile();

      var sut = FileSystem.ParseFile(file);

      // act
      var name = sut.Name;

      // assert
      Assert.Equal(file.Name, name);
    }

    [Fact]
    public void Equals_Same()
    {
      // arrange
      var file = CreateUniqueFile();
      var sutA = FileSystem.ParseFile(file);
      var sutB = FileSystem.ParseFile(file);

      // act
      var result = sutA.Equals(sutB);

      // assert
      Assert.True(result);
    }

    [Fact]
    public void Equals_Different()
    {
      // arrange
      var dir = CreateUniqueDir();
      var file0 = CreateUniqueFile(dir);
      var file1 = CreateUniqueFile(dir);
      var sut1 = FileSystem.ParseFile(file0);
      var sut2 = FileSystem.ParseFile(file1);

      // act
      var result = sut1.Equals(sut2);

      // assert
      Assert.True(!result);
    }

    [Fact]
    public void Equals_Object()
    {
      // arrange
      var dir = CreateUniqueDir();
      var file = CreateUniqueFile(dir);
      var sut = FileSystem.ParseFile(file);
      
      // act
      Assert.Throws<InvalidCastException>(() => sut.Equals(new object()));
    }

    [Fact]
    public void Exists_Yes()
    {
      // arrange
      var file = CreateUniqueFile();      
      var sut = FileSystem.ParseFile(file);

      // act
      var exists = sut.Exists;

      // assert
      Assert.True(exists);
    }

    [Fact]
    public void Exists_No()
    {
      // arrange
      var file = CreateUniqueFile();
      file.Delete();
      
      var sut = FileSystem.ParseFile(file);

      // act
      var exists = sut.Exists;

      // assert
      Assert.True(!exists);
    }

    [Fact]
    public void Create()
    {
      // arrange
      var file = CreateUniqueFile();
      file.Delete();

      var sut = FileSystem.ParseFile(file);

      // act
      sut.Create();

      // assert
      var exists = file.Exists;
      Assert.True(exists);
    }

    [Fact]
    public void MoveTo_Exists_Plain()
    {
      // arrange
      var dir = CreateUniqueDir();
      var folderA = CreateUniqueDir(dir);
      var folderB = CreateUniqueDir(dir);
      var fileA = CreateUniqueFile(folderA);

      var sutA = FileSystem.ParseFile(fileA);
      var dirB = FileSystem.ParseFolder(folderB);

      // act
      var moved = sutA.MoveTo(dirB);

      // assert
      Assert.True(moved.Exists);
      Assert.True(!sutA.Exists);
    }
    }

    [Fact]
    public void MoveTo_FullName_Plain()
    {
      // arrange
      var dir = CreateUniqueDir();
      var folderA = CreateUniqueDir(dir);
      var folderB = CreateUniqueDir(dir);
      var fileA = CreateUniqueFile(folderA);

      var sutA = FileSystem.ParseFile(fileA);
      var dirB = FileSystem.ParseFolder(folderB);

      // act
      var moved = sutA.MoveTo(dirB);

      // assert
      Assert.Equal($"{dir.FullName}\\{folderB.Name}\\{fileA.Name}", moved.FullName);
    }

    [Fact]
    public void MoveTo_Conflict()
    {
      // arrange
      var dir = CreateUniqueDir();
      var folderA = CreateUniqueDir(dir);
      var fileA = CreateUniqueFile(folderA);

      var outerB = CreateUniqueDir(dir);
      var innerB = outerB.CreateSubdirectory(folderA.Name);
      new FileInfo(Path.Combine(innerB.FullName, fileA.Name)).OpenWrite().Close();

      var sutA = FileSystem.ParseFile(fileA);
      var sutB = FileSystem.ParseFolder(outerB);

      // act
      sutA.MoveTo(sutB);
      
      // assert
      Assert.True(File.Exists($"{dir.FullName}\\{outerB.Name}\\{folderA.Name}\\{fileA.Name}"));
    }
  }
}
