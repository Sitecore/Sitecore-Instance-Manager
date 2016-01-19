# Sitecore Instance Manager (SIM 1.4)

**SIM 1.4 is an open source tool** for managing the local park of Sitecore instances. You can install, locate, maintain, reinstall or delete Sitecore products. It has API and plugin engine so you can extend it for any your need. 

### [Download SIM 1.4](http://dl.sitecore.net/updater/sim) as a ClickOnce app

### Resources

1. [Release History](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Releases)
2. [Documentation](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Documentation)
3. [Troubleshooting](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Troubleshooting)
4. [Developer Center](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/API)
6. [SIM 2.0 planning](https://bitbucket.org/sitecore/sitecore-instance-manager)
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
* Open Sitecore Client ([bypassing security, logging in as admin](https://bitbucket.org/alienlab/sitecore-instance-manager/wiki/Manual-Features-LoginAdmin))
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

#### Extra features

* Install MongoDB in one click
* Edit `etc\hosts` file
* Batch operations with SQL databases
* Predefined configurations (Enable MVC, Scaling ...)

### [Plug-ins](https://github.com/Sitecore/Sitecore-Instance-Manager/wiki/Plugins) 

* Tray Plugin – simple actions from SIM tray icon: list sites, open in browser, hide window 
* Delete All Instances - wizard for deleting all installed Sitecore instances
* Update Licenses - Updating Sitecore license file in all installed Sitecore instances
* [SSPG](https://marketplace.sitecore.net/en/Modules/Sitecore_Support_Package_Generator.aspx) - Sitecore Support Package Generator for collecting detailed information about an instance
* Linqpad - Incorporated LINQPad plugin written by Adam Conn, used for accessing Sitecore API without touching an instance files
* [Support Toolbox](https://marketplace.sitecore.net/en/Modules/Sitecore_Support_Toolbox.aspx) - an extension of /sitecore/admin pages
* Publish Dialog - executing Sitecore publishing in application

### Supported Products

* Sitecore CMS 6.0 and later
* Sitecore Intranet 3.x 
* Sitecore Foundry 3.0 and 4.x (in a form of Sitecore package)
* Demo Solutions (Nicam, Jetstream) 
* All Sitecore modules 

Some of Sitecore modules have special support for initial configuration:

* Active Directory 
* Web Forms for Marketers 
* E-mail Campaign Manager 
