namespace SIM.Pipelines
{
  public static class PipelinesConfig
  {
    public const string Contents = @"<pipelines>
  <install title=""Installing the {InstanceName} instance"">
    <step>
      <processor type=""SIM.Pipelines.Install.CheckPackageIntegrity, SIM.Pipelines"" title=""Validating install package"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Install.GrantPermissions, SIM.Pipelines"" title=""Granting permissions"" />
      <processor type=""SIM.Pipelines.Install.Extract, SIM.Pipelines"" title=""Extracting files"">
        <processor type=""SIM.Pipelines.Install.CopyLicense, SIM.Pipelines"" title=""Copying license"" />
        <processor type=""SIM.Pipelines.Install.SetupWebsite, SIM.Pipelines"" title=""Configuring IIS website"" />
        <processor type=""SIM.Pipelines.Install.UpdateWebConfig, SIM.Pipelines"" title=""Setting data folder"" />
      </processor>
      <processor type=""SIM.Pipelines.Install.UpdateHosts, SIM.Pipelines"" title=""Updating hosts file"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Install.AttachDatabases, SIM.Pipelines"" title=""Attaching databases"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Install.Modules.InstallActions, SIM.Pipelines"" param=""archive""
                  title=""Modules: installing archive-based modules"" />
      <processor type=""SIM.Pipelines.Install.Modules.CopyAgentFiles, SIM.Pipelines""
                  title=""Modules: copying agent files"">
        <processor type=""SIM.Pipelines.Install.Modules.CopyPackages, SIM.Pipelines"" title=""Modules: copying packages"">
          <processor type=""SIM.Pipelines.Install.Modules.InstallActions, SIM.Pipelines"" param=""package|before""
                      title=""Modules: performing pre-install actions"">
            <processor type=""SIM.Pipelines.Install.Modules.StartInstance, SIM.Pipelines""
                        title=""Modules: starting instance"">
              <processor type=""SIM.Pipelines.Install.Modules.InstallPackages, SIM.Pipelines""
                          title=""Modules: installing packages"">
                <processor type=""SIM.Pipelines.Install.Modules.StartInstance, SIM.Pipelines""
                            title=""Modules: starting instance (again)"">
                  <processor type=""SIM.Pipelines.Install.Modules.PerformPostStepActions, SIM.Pipelines""
                              title=""Modules: performing post-step actions"">
                    <processor type=""SIM.Pipelines.Install.Modules.InstallActions, SIM.Pipelines""
                                param=""package|after"" title=""Modules: performing post-install actions"">
                      <processor type=""SIM.Pipelines.Install.Modules.DeleteAgentPages, SIM.Pipelines""
                                  title=""Modules: agent files"" />
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
    <step>
      <processor type=""SIM.Pipelines.MultipleDeletion.MultipleDeletion, SIM.Pipelines""
                  title=""Deleting the selected instances"" />
    </step>
  </multipleDeletion>
  <delete title=""Deleting the {InstanceName} instance"">
    <step>
      <processor type=""SIM.Pipelines.Delete.CollectData, SIM.Pipelines"" title=""Collecting data"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Delete.DeleteRegistryKey, SIM.Pipelines"" title=""Deleting registry key"" />
      <processor type=""SIM.Pipelines.Delete.StopInstance, SIM.Pipelines"" title=""Stopping application"" />
      <processor type=""SIM.Pipelines.Delete.DeleteDataFolder, SIM.Pipelines"" title=""Deleting data folder"" />
      <processor type=""SIM.Pipelines.Delete.DeleteDatabases, SIM.Pipelines"" title=""Deleting databases"" />
      <processor type=""SIM.Pipelines.Delete.DeleteMongoDatabases, SIM.Pipelines"" title=""Deleting databases"" />
      <processor type=""SIM.Pipelines.Delete.DeleteWebsiteFolder, SIM.Pipelines"" title=""Deleting website folder"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Delete.DeleteRootFolder, SIM.Pipelines"" title=""Deleting root folder"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Delete.DeleteWebsite, SIM.Pipelines"" title=""Deleting website"" />
      <processor type=""SIM.Pipelines.Delete.UpdateHosts, SIM.Pipelines"" title=""Updating the hosts file"" />
    </step>
  </delete>
  <reinstall title=""Reinstalling the {InstanceName} instance"">
    <step>
      <processor type=""SIM.Pipelines.Reinstall.CheckPackageIntegrity, SIM.Pipelines"" title=""Validating package"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Reinstall.StopInstance, SIM.Pipelines"" title=""Stopping application"" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteDataFolder, SIM.Pipelines"" title=""Deleting data folder"" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteDatabases, SIM.Pipelines"" title=""Deleting databases"" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteWebsite, SIM.Pipelines"" title=""Deleting IIS website"" />
      <processor type=""SIM.Pipelines.Reinstall.DeleteWebsiteFolder, SIM.Pipelines"" title=""Deleting website folder"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Reinstall.DeleteRootFolder, SIM.Pipelines"" title=""Recreating root folder"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Reinstall.Extract, SIM.Pipelines"" title=""Extracting files"">
        <processor type=""SIM.Pipelines.Reinstall.CopyLicense, SIM.Pipelines"" title=""Copying license"" />
        <processor type=""SIM.Pipelines.Reinstall.SetupWebsite, SIM.Pipelines"" title=""Configuring IIS website"" />
        <processor type=""SIM.Pipelines.Reinstall.UpdateWebConfig, SIM.Pipelines"" title=""Setting the data folder"" />
        <processor type=""SIM.Pipelines.Reinstall.DeleteTempFolder, SIM.Pipelines"" title=""Collecting garbage"" />
      </processor>
    </step>
    <step>
      <processor type=""SIM.Pipelines.Reinstall.AttachDatabases, SIM.Pipelines"" title=""Attaching databases"" />
    </step>
  </reinstall>
  <installmodules title=""Installing modules to the {InstanceName} instance"">
    <processor type=""SIM.Pipelines.InstallModules.InstallActions, SIM.Pipelines"" param=""archive""
                title=""Installing archive-based modules"" />
    <processor type=""SIM.Pipelines.InstallModules.CopyAgentFiles, SIM.Pipelines"" title=""Copying agent files"">
      <processor type=""SIM.Pipelines.InstallModules.CopyPackages, SIM.Pipelines""
                  title=""Copying packages into Website folder"">
        <processor type=""SIM.Pipelines.InstallModules.InstallActions, SIM.Pipelines"" param=""package|before""
                    title=""Performing pre-install actions"">
          <processor type=""SIM.Pipelines.InstallModules.StartInstance, SIM.Pipelines"" title=""Starting the instance"">
            <processor type=""SIM.Pipelines.InstallModules.InstallPackages, SIM.Pipelines""
                        title=""Installing the packages"">
              <processor type=""SIM.Pipelines.InstallModules.StartInstance, SIM.Pipelines""
                          title=""Starting the instance (again)"">
                <processor type=""SIM.Pipelines.InstallModules.PerformPostStepActions, SIM.Pipelines""
                            title=""Performing post-step actions"">
                  <processor type=""SIM.Pipelines.InstallModules.InstallActions, SIM.Pipelines"" param=""package|after""
                              title=""Performing post-install actions"">
                    <processor type=""SIM.Pipelines.InstallModules.DeleteAgentPages, SIM.Pipelines""
                                title=""Deleting agent files"" />
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
    <processor type=""SIM.Pipelines.Backup.BackupDatabases, SIM.Pipelines"" title=""Backing up databases"" />
    <processor type=""SIM.Pipelines.Backup.BackupMongoDatabases, SIM.Pipelines"" title=""Backing up MongoDB databases"" />
    <processor type=""SIM.Pipelines.Backup.BackupFiles, SIM.Pipelines"" title=""Backing up files"" />
  </backup>
  <restore title=""Restoring the {InstanceName} instance"">
    <processor type=""SIM.Pipelines.Restore.RestoreDatabases, SIM.Pipelines"" title=""Restoring databases"" />
    <processor type=""SIM.Pipelines.Restore.RestoreMongoDatabases, SIM.Pipelines"" title=""Restoring MongoDB databases"" />
    <processor type=""SIM.Pipelines.Restore.DeleteFiles, SIM.Pipelines"" title=""Deleting files"">
      <processor type=""SIM.Pipelines.Restore.RestoreFiles, SIM.Pipelines"" title=""Restoring files"" />
    </processor>
  </restore>
  <export title=""Exporting the {InstanceName} instance"">
    <step>
      <processor type=""SIM.Pipelines.Export.ExportDatabases, SIM.Pipelines"" title=""Exporting databases"" />
      <processor type=""SIM.Pipelines.Export.ExportMongoDatabases, SIM.Pipelines"" title=""Exporting MongoDB databases"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Export.ExportFiles, SIM.Pipelines"" title=""Exporting files"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Export.ExportSettings, SIM.Pipelines"" title=""Exporting settings"" />
    </step>
    <step>
      <processor type=""SIM.Pipelines.Export.ExportPostActions, SIM.Pipelines"" title=""Assembling zip package"" />
    </step>
  </export>
  <import title=""Importing instance"">
    <processor type=""SIM.Pipelines.Import.ImportInitialization, SIM.Pipelines"" title=""Initialization"">
      <processor type=""SIM.Pipelines.Import.ImportRestoreDatabases, SIM.Pipelines"" title=""Restore databases"" />
      <processor type=""SIM.Pipelines.Import.ImportRestoreMongoDatabases, SIM.Pipelines""
                  title=""Restore MongoDB databases"" />
      <processor type=""SIM.Pipelines.Import.ImportUnpackSolution, SIM.Pipelines"" title=""Unpack solution"">
        <processor type=""SIM.Pipelines.Import.UpdateConnectionStrings, SIM.Pipelines""
                    title=""Update connection strings"" />
        <processor type=""SIM.Pipelines.Import.UpdateDataFolder, SIM.Pipelines"" title=""Update data folder"" />
        <processor type=""SIM.Pipelines.Import.UpdateLicense, SIM.Pipelines"" title=""Update license"" />
      </processor>
      <processor type=""SIM.Pipelines.Import.ImportRegisterWebsite, SIM.Pipelines"" title=""Update IIS metabase"" />
      <processor type=""SIM.Pipelines.Import.ImportHostNames, SIM.Pipelines"" title=""Update hosts file"" />
    </processor>
  </import>
</pipelines>";
  }
}
