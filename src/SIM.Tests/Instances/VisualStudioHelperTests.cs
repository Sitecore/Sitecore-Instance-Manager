namespace SIM.Tests.Instances
{
  using System;
  using System.IO;
  using System.Linq;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using SIM.Instances;

  [TestClass]
  public class VisualStudioHelperTests
  {
    #region Public methods

    [TestMethod]
    public void GetVisualStudioSolutionFilesInnerTests()
    {
      {
        // 1
        var tmpFolder = GetTempFolder();
        var project1 = this.CreateProject(tmpFolder, "Project1");
        var project2 = this.CreateProject(tmpFolder, "Project2");
        var project3 = this.CreateProject(tmpFolder, "Project3");

        var actual = VisualStudioHelper.GetVisualStudioSolutionFiles(tmpFolder, tmpFolder).ToArray();
        this.Assert(actual, new[]
        {
          project1, project2, project3
        });
      }
      {
        // 2
        var tmpFolder = GetTempFolder();
        var project1 = this.CreateProject(tmpFolder, "Project1");
        var project2 = this.CreateProject(tmpFolder, "Project2");
        var project3 = this.CreateProject(tmpFolder, "Project3");
        var solution1 = this.CreateSolution(tmpFolder, "Solution1", project1);
        var solution2 = this.CreateSolution(tmpFolder, "Solution2", project1, project2);

        var actual = VisualStudioHelper.GetVisualStudioSolutionFiles(tmpFolder, tmpFolder).ToArray();
        this.Assert(actual, new[]
        {
          solution1, solution2, project3
        });
      }
    }

    #endregion

    #region Private methods

    private static string GetTempFolder()
    {
      var tmpFolder = Path.GetTempFileName() + "dir";
      FileSystem.FileSystem.Local.Directory.Ensure(tmpFolder);
      return tmpFolder;
    }

    private void Assert(string[] actual, string[] expected)
    {
      Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected.Length, actual.Length, "Array Length");
      for (int i = 0; i < actual.Length; ++i)
      {
        var a = actual[i];
        var e = expected[i];
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(a, e, "Array[{0}]".FormatWith(i));
      }
    }

    private string CreateProject(string tmpFolder, string name)
    {
      var path = Path.Combine(tmpFolder, name + ".csproj");
      FileSystem.FileSystem.Local.File.WriteAllText(path, string.Empty);
      return path;
    }

    private string CreateSolution(string tmpFolder, string name, params string[] projects)
    {
      var path = Path.Combine(tmpFolder, name + ".sln");
      var lines = projects.Select(p => @"Project(""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}"") = ""ProjectX"", ""{0}"", ""{{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}}""".FormatWith(p));
      FileSystem.FileSystem.Local.File.WriteAllText(path, string.Join(Environment.NewLine, lines));
      return path;
    }

    #endregion
  }
}