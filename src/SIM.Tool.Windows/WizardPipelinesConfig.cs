namespace SIM.Tool.Windows
{
  public static class WizardPipelinesConfig
  {
    public const string Contents = @"<configuration>
  <pipelines>
    <download title=""Downloading Sitecore"">
      <processor type=""SIM.Tool.Windows.Pipelines.Download.DownloadProcessor, SIM.Tool.Windows""
                  title=""Downloading packages"" />
    </download>
  </pipelines>
  <wizardPipelines>
    <agreement title=""License Agreement"" startButton=""Accept"" finishText=""Thank you"">
      <steps afterLastStep=""SIM.Tool.Windows.SaveAgreement, SIM.Tool.Windows"">
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""You can see this wizard because it is the first time Sitecore Instance Manager was executed in this user account after installation or update. By using this product you agree with following terms of use: https://www.sitecore.com/legal/terms-of-use"" />
      </steps>
    </agreement>
    <setup title=""Initial Configuration Wizard"" startButton=""Next""
           finishText=""Congratulations! The installation was successfully completed and you can start using it out of the box. If you don't have any Sitecore zip files in the local repository then you may download them from dev.sitecore.net via the Get Sitecore button on the Ribbon or do it manually""
           cancelButton=""Exit"">
      <steps afterLastStep=""SIM.Tool.Windows.Pipelines.Setup.SetupProcessor, SIM.Tool.Windows"">
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""You can see this wizard because it is the first time Sitecore Instance Manager (SIM) was executed in this user account. By using this product you agree with following terms of use: https://www.sitecore.com/legal/terms-of-use

You should set your preferences before you can use it, this wizard will help you. Before you being please make sure that you have an IIS and SQL Server installed on your PC and you have access to them. 
              
PLEASE NOTE that all your application settings and log files are stored in your personal folder (%APPDATA%\Sitecore\Sitecore Instance Manager) so that other users of this PC will not see, use or change your setting details."" />
        <step name=""Instances Root Folder"" type=""SIM.Tool.Windows.UserControls.Setup.InstancesRoot, SIM.Tool.Windows"" />
        <step name=""Local Repository and Sitecore License""
              type=""SIM.Tool.Windows.UserControls.Setup.LocalRepository, SIM.Tool.Windows"" />
        <step name=""SQL Server Connection String""
              type=""SIM.Tool.Windows.UserControls.Setup.ConnectionString, SIM.Tool.Windows"" />
        <step name=""File System permissions"" type=""SIM.Tool.Windows.UserControls.Setup.Permissions, SIM.Tool.Windows"" />
      </steps>
    </setup>

    <download title=""Download Sitecore Wizard"" startButton=""Next"" finishText=""Done"" cancelButton=""Exit"">
      <steps>
        <step name=""Welcome message"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""By using this product you agree with following terms of use:
https://www.sitecore.com/legal/terms-of-use

This wizard helps you to download packages ('ZIP archive of the Sitecore site root') of the Sitecore versions that Sitecore Instance Manager will be able to install for you.
              
Note that due to the large size of each installation package the whole download operation may require much time.

In addition, you may download these files from dev.sitecore.net to your local repository folder manually using your browser and/or any download program."" />
        <step name=""Choose versions to download""
              type=""SIM.Tool.Windows.UserControls.Download.Downloads, SIM.Tool.Windows"" />
      </steps>
    </download>

    <installSolr title=""Installing new Solr instance"" startButton=""Install""
                 finishText=""The installation was successfully completed"">
          <args type=""SIM.Tool.Base.Pipelines.Install9WizardArgs, SIM.Tool.Base""/>
          <steps>
            <step name=""STEP 1 of 2 - DETAILS"" 
                  type=""SIM.Tool.Windows.UserControls.Install.SolrDetails, SIM.Tool.Windows"" />
            <step name=""STEP 2 of 2 - SELECT INSTALLATION TASKS"" 
                  type=""SIM.Tool.Windows.UserControls.Install.Instance9SelectTasks, SIM.Tool.Windows"" />
         </steps> 
      <finish>
         <action text=""Add this solr to SIM"" type=""SIM.Tool.Windows.Pipelines.Solr.AddSolrToSimConfig, SIM.Tool.Windows"" method=""Run"" />
      </finish>
     </installSolr>

<delete9 title=""Deleting instances"" startButton=""Delete""
             finishText=""The uninstallation was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.Install9WizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 2 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Delete9Details, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 2 - SELECT UNINSTALLATION TASKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9SelectTasks, SIM.Tool.Windows"" />        
     </steps>     
     <finish>
         <hive type=""SIM.Tool.Windows.Pipelines.Install.Install9ActionsHive, SIM.Tool.Windows"" />
     </finish>
    </delete9>
    <multipleDeletion9 title=""Multiple deletion Sitecore 9 and later"" startButton=""Delete""
                      finishText=""Detailed information about deletion can be found below"">
      <steps>
        <step name=""Select the Sitecore environments that you want to delete""
              type=""SIM.Tool.Windows.UserControls.MultipleDeletion.SelectSitecore9Environments, SIM.Tool.Windows"" />
        <step name=""Confirmation"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""Are you sure you want to delete the selected Sitecore environments?"" />
      </steps>
    </multipleDeletion9>
    <reinstall9 title=""Reinstalling {InstanceName}"" startButton=""Reinstall""
             finishText=""The re-installation was successfully completed"">    
      <args type=""SIM.Tool.Base.Pipelines.ReinstallWizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""Confirmation"" 
              type=""SIM.Tool.Windows.UserControls.Reinstall.Reinstall9Confirmation, SIM.Tool.Windows"" />       
     </steps>  
    </reinstall9>
    <install9 title=""Installing new instance"" startButton=""Install""
             finishText=""The installation was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.Install9WizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 3 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9Details, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 3 - SELECT INSTALLATION TASKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9SelectTasks, SIM.Tool.Windows"" />
<step name=""STEP 3 of 3 - PARAMETERS VALIDATION"" 
              type=""SIM.Tool.Windows.UserControls.Install.Instance9Validation, SIM.Tool.Windows"" />
     </steps>   
     <finish>
         <hive type=""SIM.Tool.Windows.Pipelines.Install.Install9ActionsHive, SIM.Tool.Windows"" />
     </finish>
    </install9>
    <installContainer title=""Installing new instance"" startButton=""Install""
             finishText=""The installation was successfully completed"">
      <steps>
        <step name=""STEP 1 of 3 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.Containers.ContainerDetails, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 3 - SELECT TOPOLOGY AND TAG"" 
              type=""SIM.Tool.Windows.UserControls.Install.Containers.SelectTag, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 3 - SELECT MODULES"" 
              type=""SIM.Tool.Windows.UserControls.Install.Containers.SelectModules, SIM.Tool.Windows"" />
     </steps>   
    </installContainer>
    <deleteContainer title=""Deleting the environment"" startButton=""Delete""
             finishText=""The deleting was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.DeleteContainersWizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 1 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Delete.Containers.DeleteDetails, SIM.Tool.Windows"" />
     </steps>           
    </deleteContainer>
    <reinstallContainer title=""Reinstalling the instance"" startButton=""Reinstall""
             finishText=""The reinstallation was successfully completed"">
      <args type=""SIM.Tool.Base.Pipelines.DeleteContainersWizardArgs, SIM.Tool.Base""/>
      <steps>
        <step name=""STEP 1 of 1 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Reinstall.Containers.ReinstallDetails, SIM.Tool.Windows"" />
     </steps>   
    </reinstallContainer>
    <install title=""Installing new instance"" startButton=""Install""
             finishText=""The installation was successfully completed"">
      <steps>
        <step name=""STEP 1 of 7 - DETAILS"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceDetails, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 7 - TWEAKS"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceSettings, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 7 - CONFIGURATION ROLES"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceRole, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 7 - CONFIGURATION ROLES"" 
              type=""SIM.Tool.Windows.UserControls.Install.InstanceRole9, SIM.Tool.Windows"" />
        <step name=""STEP 4 of 7 - OFFICIAL SITECORE MODULES"" 
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ModulesDetails, SIM.Tool.Windows"" />
        <step name=""STEP 5 of 7 - CUSTOM PACKAGES AND ZIP FILES""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.FilePackages, SIM.Tool.Windows"" />
        <step name=""STEP 6 of 7 - CONFIGURATION PRESETS""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ConfigurationPackages, SIM.Tool.Windows"" />
        <step name=""STEP 7 of 7 - REVIEW, REARRANGE INSTALLATION ORDER OR ADD CUSTOM PACKAGE""
          type=""SIM.Tool.Windows.UserControls.Install.Modules.ReorderPackages, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenBrowser"" />
        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />
        <action text=""Open in Browser (Sitecore Client; Log in as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""LoginAdmin"" />
        <action text=""Open folder"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenWebsiteFolder"" />
        <action text=""Open Visual Studio"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""OpenVisualStudio"" />
        <action text=""Make a back up"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows""
                method=""BackupInstance"" />
        <action text=""Publish Site"" type=""SIM.Tool.Windows.Pipelines.Install.InstallActions, SIM.Tool.Windows"" method=""PublishSite"" />
        <hive type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesFinishActionHive, SIM.Tool.Windows"" />
      </finish>
    </install>
    <delete title=""Deleting the {InstanceName} instance"" startButton=""Delete""
            finishText=""The deleting was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""The selected Sitecore instance will be deleted. These items will be deleted automatically:               
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:
    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
      </steps>
    </delete>
    <multipleDeletion title=""Multiple deletion Sitecore 8 and earlier"" startButton=""Delete""
                      finishText=""The deleting was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""These items will be deleted automatically for each of selected instances:               
    
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:

    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
        <step name=""Select the instances that you want to delete""
              type=""SIM.Tool.Windows.UserControls.MultipleDeletion.SelectInstances, SIM.Tool.Windows"" />
        <step name=""Confirmation"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""Are you sure you want to delete the selected instances?"" />
      </steps>
    </multipleDeletion>
    <backup title=""Backing up the {InstanceName} instance"" startButton=""Backup""
            finishText=""The backup was successfully created"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""A back up of the selected instance will be created. "" />
        <step name=""Specify the backup name and and select the necessary resources""
              type=""SIM.Tool.Windows.UserControls.Backup.BackupSettings, SIM.Tool.Windows"" />
      </steps>
    </backup>
    <restore title=""Restoring up the {InstanceName} instance"" startButton=""Restore""
             finishText=""The instance was successfully restored from the backup"">
      <steps>
        <step name=""Choose backup"" type=""SIM.Tool.Windows.UserControls.Backup.ChooseBackup, SIM.Tool.Windows"" />
      </steps>
    </restore>
    <export title=""Exporting the {InstanceName} instance"" startButton=""Export""
            finishText=""The export was successfully performed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""An export of the selected instance will be performed:
							
• exporting databases
• exporting files (Data and WebRoot folders)
• exporting instance settings
• assembling zip package"" />
        <step name=""Choose the databases for exporting""
              type=""SIM.Tool.Windows.UserControls.Export.ExportDatabases, SIM.Tool.Windows"" />
        <step name=""Specify path and name for the export file""
              type=""SIM.Tool.Windows.UserControls.Export.ExportFile, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open folder"" type=""SIM.Tool.Windows.UserControls.Export.FinishActions, SIM.Tool.Windows""
                method=""OpenExportFolder"" />
      </finish>
    </export>
    <import title=""Importing Sitecore instance"" startButton=""Import"" finishText=""The import was successfully performed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""An import of the Sitecore instance will be performed:
							
• importing databases
• importing files (Data and WebRoot folders)
• moving your license to the Data
• importing IIS website and Application Pool "" />
        <step name=""Specify root path and website name""
              type=""SIM.Tool.Windows.UserControls.Import.ImportWebsite, SIM.Tool.Windows"" />
        <step name=""Change site bindings if needed""
              type=""SIM.Tool.Windows.UserControls.Import.SetWebsiteBindings, SIM.Tool.Windows"" />
      </steps>
    </import>
    <reinstall title=""Reinstalling the {InstanceName} instance"" startButton=""Reinstall""
               finishText=""The reinstalling was successfully completed"">
      <steps>
        <step name=""Information"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""The selected Sitecore instance will be re-installed without any modules. 
        
These items will be deleted automatically: 
    • the root folder
    • the databases located inside the instance's root folder
    • the IIS website and application pool
    • the appropriate record in the hosts file

But the confirmation will be required if the databases are attached to:
    • the local SQL Server* and located out of the instance root folder        
    • any other SQL Server instance

* - the SQL Server instance specified by connection string in the Settings dialog"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenBrowser"" />

        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />

        <action text=""Open in Browser (Sitecore Client; Login as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""LoginAdmin"" />

        <action text=""Open folder"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenWebsiteFolder"" />

        <action text=""Open Visual Studio"" 
                type=""SIM.Tool.Windows.Pipelines.Reinstall.FinishActions, SIM.Tool.Windows"" method=""OpenVisualStudio"" />

      </finish>
    </reinstall>
    <installmodules title=""Installing modules to the {InstanceName} instance"" startButton=""Install""
                    finishText=""The modules installation was successfully completed"">
      <steps>
        <step name=""STEP 1 of 4 - OFFICIAL SITECORE MODULES"" 
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ModulesDetails, SIM.Tool.Windows"" />
        <step name=""STEP 2 of 4 - CUSTOM PACKAGES AND ZIP FILES""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.FilePackages, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 4 - CONFIGURATION PRESETS""
              type=""SIM.Tool.Windows.UserControls.Install.Modules.ConfigurationPackages, SIM.Tool.Windows"" />
        <step name=""STEP 4 of 4 - REVIEW, REARRANGE INSTALLATION ORDER OR ADD CUSTOM PACKAGE""
          type=""SIM.Tool.Windows.UserControls.Install.Modules.ReorderPackages, SIM.Tool.Windows"" />
      </steps>
      <finish>
        <action text=""Open in Browser"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenBrowser"" />
        <action text=""Open in Browser (Sitecore Client)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""OpenSitecoreClient"" />
        <action text=""Open in Browser (Sitecore Client; Login as Admin)""
                type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""LoginAdmin"" />
        <action text=""Open folder"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenWebsiteFolder"" />
        <action text=""Open Visual Studio"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows""
                method=""OpenVisualStudio"" />
        <action text=""Publish Site"" type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesActions, SIM.Tool.Windows"" method=""PublishSite"" />
        <hive type=""SIM.Tool.Windows.Pipelines.Install.InstallModulesFinishActionHive, SIM.Tool.Windows"" />
      </finish>
    </installmodules>
    <searchAndDeleteResources title=""Search and delete resources"" startButton=""Delete"" finishText=""Done"">
      <steps>
        <step name=""STEP 1 of 3 - INFORMATION"" type=""SIM.Tool.Windows.UserControls.ConfirmStepUserControl, SIM.Tool.Windows""
              param=""This wizard helps you to find the following resources based on the Sitecore site name and delete them if they are not needed:               
    • the certificates from the Local Machine and Current User stores
    • the IIS application pools and sites
    • the lines in the hosts file
    • the Windows services
    • the SQL databases
    • the Solr cores and folders
    • the root folders
    • the environment data defined in the Environments.json file (this is only applicable for Sitecore 9 and later)
    • uninstall params folders (this is only applicable for Sitecore 9 and later)"" />
        <step name=""STEP 2 of 3 - DETAILS""
              type=""SIM.Tool.Windows.UserControls.Resources.Details, SIM.Tool.Windows"" />
        <step name=""STEP 3 of 3 - SEARCH AND DELETE""
              type=""SIM.Tool.Windows.UserControls.Resources.SearchAndDelete, SIM.Tool.Windows"" />
      </steps>
    </searchAndDeleteResources>
  </wizardPipelines>
</configuration>
";
  }
}