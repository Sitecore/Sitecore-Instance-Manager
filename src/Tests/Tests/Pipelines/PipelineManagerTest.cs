using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIM.Base;
using SIM.Pipelines;
using SIM.Pipelines.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SIM.Tests.Pipelines
{
  [TestClass]
  public class PipelineManagerTest
  {
    static int processorsCount;
    static readonly List<string> actions = new List<string>();
    [TestMethod]
    public void Test()
    {
      var pipelineName = "test";
      var pipelineTitle = "Some variable value is {SomeProperty}";
      var type = typeof(TestProcessor).AssemblyQualifiedName;
      var processorTitle = "Some title of the processor";
      var args = new TestArgs();
      var controller = new TestController();
      processorsCount = 0;
      actions.Clear();

      var str = @"<pipelines>
  <{0} title=""{1}"">
    <step>
      <processor type=""{2}"" title=""{3}"" param=""1-1"">
        <processor type=""{2}"" title=""{3}"" param=""1-1-1"" />
      </processor>
      <processor type=""{2}"" title=""{3}"" param=""1-2""/>
    </step>
    <step>
      <processor type=""{2}"" title=""{3}"" param=""2-1"" />
    </step>
  </{0}>
</pipelines>".FormatWith(pipelineName, pipelineTitle, type, processorTitle);
      var xml = XmlDocumentEx.LoadXml(str);
      PipelineManager.Initialize(xml.DocumentElement);
      PipelineManager.StartPipeline(pipelineName, args, controller, false);
      var expected =
@"Start|Some variable value is SomeValue
ProcessorStarted|Some title of the processor
Process|1-1|1
ProcessorDone|Some title of the processor
IncrementProgress|1
ProcessorStarted|Some title of the processor
Process|1-1-1|2
ProcessorDone|Some title of the processor
IncrementProgress|1
ProcessorStarted|Some title of the processor
Process|1-2|3
ProcessorDone|Some title of the processor
IncrementProgress|1
ProcessorStarted|Some title of the processor
Process|2-1|4
ProcessorDone|Some title of the processor
IncrementProgress|1
Finish|Done.|True";
      Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, actions.Join(Environment.NewLine));
    }

    class TestArgs : ProcessorArgs
    {
      public string SomeProperty
      {
        get
        {
          return "SomeValue";
        }
      }
    }

    private class TestController : IPipelineController
    {
      double IPipelineController.Maximum
      {
        get;
        set;
      }

      Pipeline IPipelineController.Pipeline
      {
        get;
        set;
      }

      string IPipelineController.Ask(string title, string defaultValue)
      {
        actions.Add("Ask|" + title + "|" + defaultValue);
        return defaultValue;
      }

      bool IPipelineController.Confirm(string message)
      {
        actions.Add("Confirm|" + message);
        return true;
      }

      void IPipelineController.Execute(string path, string args)
      {
        actions.Add("Execute|" + path + "|" + args);
      }

      void IPipelineController.Finish(string message, bool closeInterface)
      {
        actions.Add("Finish|" + message + "|" + closeInterface);
      }

      void IPipelineController.IncrementProgress()
      {
        ((IPipelineController)this).IncrementProgress(1);
      }

      void IPipelineController.IncrementProgress(long progress)
      {
        actions.Add("IncrementProgress|" + progress);           
      }

      void IPipelineController.Pause()
      {
        actions.Add("Pause");    
      }

      void IPipelineController.ProcessorCrashed(string error)
      {
        actions.Add("ProcessorCrashed|" + error);            
      }

      void IPipelineController.ProcessorDone(string title)
      {
        actions.Add("ProcessorDone|" + title);   
      }

      void IPipelineController.ProcessorSkipped(string processorName)
      {
        actions.Add("ProcessorSkipped|" + processorName);   
      }

      void IPipelineController.ProcessorStarted(string title)
      {
        actions.Add("ProcessorStarted|" + title);   
      }

      void IPipelineController.Resume()
      {
        actions.Add("Resume");  
      }

      string IPipelineController.Select(string message, IEnumerable<string> options, bool allowMultipleSelection, string defaultValue)
      {
        actions.Add("Select|" + message + "|" + allowMultipleSelection + "|" + defaultValue);  
        return defaultValue;
      }

      void IPipelineController.Start(string replaceVariables, List<Step> steps)
      {
        actions.Add("Start|" + replaceVariables);                  
      }

      public void SetProgress(long progress)
      {
        actions.Add("SetProgress|" + progress);
      }
    }

    private class TestProcessor : Processor
    {
      private readonly int number;

      public TestProcessor()
      {
        this.number = ++processorsCount;
      }
      public override long EvaluateStepsCount(ProcessorArgs args)
      {
        return this.number;
      }

      protected override void Process(ProcessorArgs args)
      {
        actions.Add("Process|{0}|{1}".FormatWith(this.ProcessorDefinition.Param, this.number));
      }      
    }

    [TestMethod]
    public void RealTest()
    {
      string path = Path.GetFullPath(@"..\..\..\..\Code\Core Level 4\Pipelines\Pipelines.config");
      var actual = XmlDocumentEx.LoadXml(PipelineManager.Initialize(path).OuterXml);
      var expected = XmlDocumentEx.LoadXml(ExpectedPipelinesXml);
      TestHelper.AreEqual(actual, expected);
    }

    private const string ExpectedPipelinesXml = @"<pipelines>
  <install title=""Installing the {InstanceName} instance"">
    <step args="""">
      <processor type=""SIM.Pipelines.Install.CheckPackageIntegrity"" title=""Validating install package"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Install.GrantPermissions"" title=""Granting permissions"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Install.Extract"" title=""Extracting files"" process=""once"" param="""">
        <processor type=""SIM.Pipelines.Install.MoveData"" title=""Moving files"" process=""once"" param="""">
          <processor type=""SIM.Pipelines.Install.CopyLicense"" title=""Copying license"" process=""once"" param="""" />
          <processor type=""SIM.Pipelines.Install.SetupWebsite"" title=""Configuring IIS website"" process=""once"" param="""" />
          <processor type=""SIM.Pipelines.Install.UpdateDataFolder"" title=""Setting data folder"" process=""once"" param="""" />
        </processor>
      </processor>
      <processor type=""SIM.Pipelines.Install.UpdateHosts"" title=""Updating hosts file"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Install.MoveDatabases"" title=""Moving database files"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Install.AttachDatabases"" title=""Attaching databases"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Install.Modules.InstallActions"" title=""Modules: installing archive-based modules"" process=""once"" param=""archive"" />
      <processor type=""SIM.Pipelines.Install.Modules.CopyAgentFiles"" title=""Modules: copying agent files"" process=""once"" param="""">
        <processor type=""SIM.Pipelines.Install.Modules.CopyPackages"" title=""Modules: copying packages"" process=""once"" param="""">
          <processor type=""SIM.Pipelines.Install.Modules.InstallActions"" title=""Modules: performing pre-install actions"" process=""once"" param=""package|before"">
            <processor type=""SIM.Pipelines.Install.Modules.StartInstance"" title=""Modules: starting instance"" process=""once"" param="""">
              <processor type=""SIM.Pipelines.Install.Modules.InstallPackages"" title=""Modules: installing packages"" process=""once"" param="""">
                <processor type=""SIM.Pipelines.Install.Modules.StartInstance"" title=""Modules: starting instance (again)"" process=""once"" param="""">
                  <processor type=""SIM.Pipelines.Install.Modules.PerformPostStepActions"" title=""Modules: performing post-step actions"" process=""once"" param="""">
                    <processor type=""SIM.Pipelines.Install.Modules.InstallActions"" title=""Modules: performing post-install actions"" process=""once"" param=""package|after"">
                      <processor type=""SIM.Pipelines.Install.Modules.DeleteAgentPages"" title=""Modules: agent files"" process=""once"" param="""" />
                    </processor>
                  </processor>
                </processor>
              </processor>
            </processor>
          </processor>
        </processor>
      </processor>
    </step>
  </install>
  <multipleDeletion title=""Multiple deletion"">
    <step args="""">
      <processor type=""SIM.Pipelines.MultipleDeletion.MultipleDeletion"" title=""Deleting the selected instances"" process=""once"" param="""" />
    </step>
  </multipleDeletion>
  <delete title=""Deleting the {InstanceName} instance"">
    <step args="""">
      <processor type=""SIM.Pipelines.Delete.CollectData"" title=""Collecting data"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Delete.StopInstance"" title=""Stopping application"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Delete.DeleteDataFolder"" title=""Deleting data folder"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Delete.DeleteDatabases"" title=""Deleting databases"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Delete.DeleteWebsiteFolder"" title=""Deleting website folder"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Delete.DeleteRootFolder"" title=""Deleting root folder"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Delete.DeleteWebsite"" title=""Deleting website"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Delete.UpdateHosts"" title=""Updating the hosts file"" process=""once"" param="""" />
    </step>
  </delete>
  <reinstall title=""Reinstalling the {InstanceName} instance"">
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.CheckPackageIntegrity"" title=""Validating package"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.StopInstance"" title=""Stopping application"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteDataFolder"" title=""Deleting data folder"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteDatabases"" title=""Deleting databases"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteWebsite"" title=""Deleting IIS website"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteWebsiteFolder"" title=""Deleting website folder"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.DeleteRootFolder"" title=""Recreating root folder"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.Extract"" title=""Extracting files"" process=""once"" param="""">
        <processor type=""SIM.Pipelines.Reinstall.MoveData"" title=""Moving files"" process=""once"" param="""">
          <processor type=""SIM.Pipelines.Reinstall.CopyLicense"" title=""Copying license"" process=""once"" param="""" />
          <processor type=""SIM.Pipelines.Reinstall.SetupWebsite"" title=""Configuring IIS website"" process=""once"" param="""" />
          <processor type=""SIM.Pipelines.Reinstall.UpdateDataFolder"" title=""Setting the data folder"" process=""once"" param="""" />
          <processor type=""SIM.Pipelines.Reinstall.DeleteTempFolder"" title=""Collecting garbage"" process=""once"" param="""" />
        </processor>
      </processor>
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.MoveDatabases"" title=""Moving database files"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Reinstall.AttachDatabases"" title=""Attaching databases"" process=""once"" param="""" />
    </step>
  </reinstall>
  <installmodules title=""Installing modules to the {InstanceName} instance"">
    <processor type=""SIM.Pipelines.InstallModules.InstallActions"" title=""Installing archive-based modules"" process=""once"" param=""archive"" />
    <processor type=""SIM.Pipelines.InstallModules.CopyAgentFiles"" title=""Copying agent files"" process=""once"" param="""">
      <processor type=""SIM.Pipelines.InstallModules.CopyPackages"" title=""Copying packages into Website folder"" process=""once"" param="""">
        <processor type=""SIM.Pipelines.InstallModules.InstallActions"" title=""Performing pre-install actions"" process=""once"" param=""package|before"">
          <processor type=""SIM.Pipelines.InstallModules.StartInstance"" title=""Starting the instance"" process=""once"" param="""">
            <processor type=""SIM.Pipelines.InstallModules.InstallPackages"" title=""Installing the packages"" process=""once"" param="""">
              <processor type=""SIM.Pipelines.InstallModules.StartInstance"" title=""Starting the instance (again)"" process=""once"" param="""">
                <processor type=""SIM.Pipelines.InstallModules.PerformPostStepActions"" title=""Performing post-step actions"" process=""once"" param="""">
                  <processor type=""SIM.Pipelines.InstallModules.InstallActions"" title=""Performing post-install actions"" process=""once"" param=""package|after"">
                    <processor type=""SIM.Pipelines.InstallModules.DeleteAgentPages"" title=""Deleting agent files"" process=""once"" param="""" />
                  </processor>
                </processor>
              </processor>
            </processor>
          </processor>
        </processor>
      </processor>
    </processor>
  </installmodules>
  <backup title=""Backing up the {InstanceName} instance"">
    <processor type=""SIM.Pipelines.Backup.BackupDatabases"" title=""Backing up databases"" process=""once"" param="""" />
    <processor type=""SIM.Pipelines.Backup.BackupFiles"" title=""Backing up files"" process=""once"" param="""" />
  </backup>
  <restore title=""Restoring the {InstanceName} instance"">
    <processor type=""SIM.Pipelines.Restore.RestoreDatabases"" title=""Restoring databases"" process=""once"" param="""" />
    <processor type=""SIM.Pipelines.Restore.DeleteFiles"" title=""Deleting files"" process=""once"" param="""">
      <processor type=""SIM.Pipelines.Restore.RestoreFiles"" title=""Restoring files"" process=""once"" param="""" />
    </processor>
  </restore>
  <export title=""Exporting the {InstanceName} instance"">
    <step args="""">
      <processor type=""SIM.Pipelines.Export.ExportDatabases"" title=""Exporting databases"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Export.ExportFiles"" title=""Exporting files"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Export.ExportSettings"" title=""Exporting settings"" process=""once"" param="""" />
    </step>
    <step args="""">
      <processor type=""SIM.Pipelines.Export.ExportPostActions"" title=""Assembling zip package"" process=""once"" param="""" />
    </step>
  </export>
  <import title=""Importing instance"">
    <processor type=""SIM.Pipelines.Import.ImportInitialization"" title=""Initialization"" process=""once"" param="""">
      <processor type=""SIM.Pipelines.Import.ImportRestoreDatabases"" title=""Restore databases"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Import.ImportUnpackSolution"" title=""Unpack solution"" process=""once"" param="""">
        <processor type=""SIM.Pipelines.Import.UpdateConnectionStrings"" title=""Update connection strings"" process=""once"" param="""" />
        <processor type=""SIM.Pipelines.Import.UpdateDataFolder"" title=""Update data folder"" process=""once"" param="""" />
        <processor type=""SIM.Pipelines.Import.UpdateLicense"" title=""Update license"" process=""once"" param="""" />
      </processor>
      <processor type=""SIM.Pipelines.Import.ImportRegisterWebsite"" title=""Update IIS metabase"" process=""once"" param="""" />
      <processor type=""SIM.Pipelines.Import.ImportHostNames"" title=""Update hosts file"" process=""once"" param="""" />
    </processor>
  </import>
</pipelines>";
  }
}
