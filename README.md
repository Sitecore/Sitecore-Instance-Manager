# Sitecore Instance Manager (SIM 1.5)

**SIM 1.5 is an open source tool** for managing the local park of Sitecore instances. You can install, locate, maintain, reinstall or delete Sitecore products. It has API and plugin engine so you can extend it for any your need. 

### [Download SIM 1.5](http://dl.sitecore.net/updater/sim) as a ClickOnce app

### Resources

1. [Release History](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Releases)
2. [Documentation](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Documentation)
3. [Troubleshooting](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Troubleshooting)
4. [Developer Center](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/API)
5. [Contact Us](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Support)

Note, we are in progress of migrating from bitbucket to github so don't be confused by some of the links pointing back to bitbucket.

### Features

#### Global Features

1. **List** Sitecore websites installed locally and their modules
2. **Install** new Sitecore standalone product
3. **Install** Sitecore modules and packages
4. **Download** Sitecore products from SDN and DEV
5. **Back up** Sitecore websites, and **Restore** them
6. **Export** Sitecore website and **Import** it on remote computer
7. **Delete** Sitecore websites
8. **Reinstall** Sitecore website

#### Open in Browser

* Open website 
* Open Sitecore Client 
* Open Sitecore Client ([bypassing security, logging in as admin](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Log-in-admin))
* Open [Support Toolbox](https://bitbucket.org/sitecoresupport/sitecore-support-toolbox)

#### Open applications

* Open Website folder
* Open Visual Studio 2012 project (create project if missing)
* Open web.config and other *.config files
* Open `showconfig.xml` configuration
* Open current log file in Dynamic Log Viewer
* Open entire log files in [Sitecore Log Analyzer](http://marketplace.sitecore.net/Modules/Sitecore_Log_Analyzer.aspx)

#### Change website

* Start/Stop App Pool
* Recycle App Pool
* Kill `w3wp.exe` process
* Change Framework version
* Change Framework bitness

#### Solr index creation

* SIM now includes Solr support.
  * For Sitecore 8.2, this is available as a Configuration Preset during the installation wizard. 
  * For Sitecore 8.1, it is necessary to download the appropriate "Solr support package" from http://dev.sitecore.net/downloads, and to add it to SIM using the "Add Module" button on the "Modules list" screen during installation.
* This supports Solr 4 and higher. 
  * Note: For Solr 4.x, the default "collection1" is used as a template, and must be present. For Solr 5.x and higher, the configuration located
   at `server\solr\configsets\data_driven_schema_configs` is used, as it has the language support required to index non-English text.
* This module automates the following tasks:
    * Activates all Solr config files, and deactivates matching Lucene config files (same name with "Lucene" replacing "Solr"), with the following exceptions:
      * Sitecore 8.2 Solr + IOC files are not enabled.
      * Lucene default configuration files `Sitecore.ContentSearch.Lucene.DefaultIndexConfiguration.config` and `Sitecore.ContentSearch.Lucene.DefaultIndexConfiguration.Xdb.config` are not disabled.
      * The unmatched configuration file `Sitecore.Social.Lucene.Index.Analytics.Facebook.config` is disabled as required.
    * Sets core name to instance name + index name (e.g. "sc82u0_sitecore_web_index")
    * Copies configuration from "collection1" for each new core.
    * Calls Sitecore schema update wizard ("Generate the Solr Schema.xml file") for each new core.
    * Calls Solr API to create each core/collection.
    * Indexes are left empty, but can be built from Control Panel/Indexing Manager.
    * Enables Solr term support, as described [here.](https://doc.sitecore.net/sitecore_experience_platform/80/setting_up__maintaining/search_and_indexing/walkthrough_setting_up_solr#_Toc399318998)


#### Extra features

* Install MongoDB in one click
* Edit `etc\hosts` file
* Batch operations with SQL databases
* Predefined configurations (Enable MVC, Scaling ...)
* Delete All Instances - wizard for deleting all installed Sitecore instances
* Update Licenses - Updating Sitecore license file in all installed Sitecore instances
* [SSPG](https://marketplace.sitecore.net/en/Modules/Sitecore_Support_Package_Generator.aspx) - Sitecore Support Package Generator for collecting detailed information about an instance
* Publish Dialog - executing Sitecore publishing in application

### Prerequisites

SIM will only work: 

* on **Windows 7+**, or **Windows Server 2008 R2+** 
* with `Administrator` permissions
* with direct internet access (no proxy)
* when **IIS is pre-configured** with all necessary features (check Sitecore installation guide for details)
* with **local SQL Server 2012+** instance with `sa` user account with `sysadmin` permissions

For `Sitecore 7.5, 8.0, 8.1 and 8.2`:  
* with **local MongoDB** instance pre-configured and running as a service

### Known Issues

* [SQL Server integrated security is not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/KnownIssue-IntegratedSecurity)
* [IIS ip bindings are not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/KnownIssue-IpBindings)

### Supported Products

* Sitecore CMS 6.3 and later
* All Sitecore modules 

Some of Sitecore modules have special support for initial configuration:

* Active Directory 
* Web Forms for Marketers 
* E-mail Campaign Manager 
