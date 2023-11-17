# Sitecore Instance Manager (SIM)

**SIM is an open source tool** for managing the local park of Sitecore instances. You can install, locate, maintain, reinstall or delete Sitecore products. It has API and plugin engine so you can extend it for any your need. 

### [Download SIM](https://dl.sitecore.net/updater/sim/v2) as a ClickOnce app

### Resources

1. [Release History](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Releases)
2. [Documentation](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Documentation)
3. [Troubleshooting](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Troubleshooting)
4. [Developer Center](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/API)
5. [Contact Us](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Support)

### Features

#### Global Features

1. **List** Sitecore websites installed locally and their modules
2. **Install** new Sitecore standalone product
3. [**Install** Sitecore modules and packages](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Install-packages)
4. **Download** Sitecore products from https://dev.sitecore.net
5. **Back up** Sitecore websites, and **Restore** them *(available only for Sitecore 8.2 and earlier)*
6. **Export** Sitecore website and **Import** it on remote computer *(available only for Sitecore 8.2 and earlier)*
7. **Delete** Sitecore websites
8. **Reinstall** Sitecore websites
9. **Deploy** Sitecore to Docker

#### Open in Browser

* Open website 
* Open Sitecore Client 
* Open Sitecore Client ([bypassing security, logging in as admin](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Log-in-admin))
* Open [administrative tools](https://doc.sitecore.com/xp/en/developers/103/platform-administration-and-architecture/enable-and-disable-an-administrative-tool.html)

#### Open applications

* Open Website folder
* Open Visual Studio project (create project if missing)
* Open web.config and other *.config files
* Open `showconfig.xml` configuration
* Open current log file in Dynamic Log Viewer
* Open entire log files in [Sitecore Log Analyzer](https://dl.sitecore.net/updater/scla)

#### Change website

* Start/Stop App Pool
* Recycle App Pool
* Kill `w3wp.exe` process
* Change Framework version
* Change Framework bitness

#### Solr index creation

* SIM now includes Solr support.
  * For Sitecore 8.2, this is available as a Configuration Preset during the installation wizard. 
  * For Sitecore 8.1, it is necessary to download the appropriate "Solr support package" from https://dev.sitecore.net/downloads, and to add it to SIM using the "Add Module" button on the "Modules list" screen during installation.
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
    * Enables Solr term support, as described [here.](https://doc.sitecore.com/xp/en/developers/81/sitecore-experience-platform/walkthrough--setting-up-solr.html)


#### Extra features

* Install MongoDB in one click
* Edit `etc\hosts` file
* Batch operations with SQL databases
* Predefined configurations (Enable MVC, Scaling ...)
* Update Licenses - updating Sitecore license file in all installed Sitecore instances
* Multi deletion - wizard for deleting multiple installed Sitecore instances at once
* Search and delete resources - wizard for searching different types of resources and deleting specific or all of them
* [SCLA](https://dl.sitecore.net/updater/scla) - Sitecore Log Analyzer for analyzing, grouping and navigating through logs entries
* [SSPG](https://dl.sitecore.net/updater/sspg) - Sitecore Support Package Generator for collecting detailed information about an instance
* [SCB](https://dl.sitecore.net/updater/scb/) - Sitecore Config Builder for outputing single summarized configuration file based on all configuration files
* Install Solr - wizard for installing Solr instances compatible with different Sitecore versions
* Publish Dialog - executing Sitecore publishing in application *(available only for Sitecore 8.2 and earlier)*

### Prerequisites

SIM will only work: 

* on **Windows 7+**, or **Windows Server 2008 R2+** 
* with `Administrator` permissions
* with direct Internet access (no proxy)
* when **IIS is pre-configured** with all necessary features (check Sitecore installation guide for details)

For `Sitecore 9.0` and later:
* with [compatible **SQL Server**](https://support.sitecore.com/kb?id=kb_article_view&sysparm_article=KB0087164) instance with `sysadmin` permissions
* with [compatible **Azure SQL Server**](https://support.sitecore.com/kb?id=kb_article_view&sysparm_article=KB0087164) instance (connection string example: Data Source=tcp:[name].database.windows.net;User ID=[user];Password=[password])
* with [ssdt15 chocolatey package](https://chocolatey.org/packages/ssdt15) installed 

For `Sitecore 7.5, 8.0, 8.1 and 8.2`:  
* with **local MongoDB** instance pre-configured and running as a service
* with **local SQL Server 2012+** instance with `sa` user account with `sysadmin` permissions

### Known Issues

* [Outdated download links in Prerequisites.json](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Outdated-download-links-in-Prerequisites.json)
* [Prerequisites require Internet access to be installed](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Prerequisites-require-Internet-access-to-be-installed)
* [SQL Server integrated security is not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Integrated-Security)
* [HTTPS or IP-based Bindings are not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-IP-Bindings)
* [Sitecore environment can use only the default HTTPS port 443](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Sitecore-Uses-Only-Default-HTTPS-port)
* [SQL Server 2012 or 2014 default user account is not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Sql-Server-Default-Account)
* [Automatic upgrade to SIM 1.6 is not supported](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Known-Issue-Upgrade-to-SIM-1.6)

### Supported Products

* Sitecore CMS 6.3 and later
* All Sitecore modules 

Some of Sitecore modules have special support for initial configuration:

* Active Directory 
* Web Forms for Marketers 
* E-mail Campaign Manager 
