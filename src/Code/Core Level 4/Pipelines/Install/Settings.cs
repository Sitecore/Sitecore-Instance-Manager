using SIM.Base;

namespace SIM.Pipelines.Install
{
  public static class Settings
  {
    [NotNull]
    public static readonly AdvancedProperty<string> CoreInstallWebServerIdentity = AdvancedSettings.Create("Core/Install/WebServer/Identity", "NetworkService");

    [NotNull]
    public static readonly AdvancedProperty<string> CoreInstallWebServerIdentityPassword = AdvancedSettings.Create("Core/Install/WebServer/Identity/Password", "");

    [NotNull]
    public static readonly AdvancedProperty<string> AppMongoConnectionString = AdvancedSettings.Create("App/Mongo/ConnectionString", @"mongodb://localhost:27017/");

    [NotNull]
    public static readonly AdvancedProperty<bool> CoreInstallRadControls = AdvancedSettings.Create("Core/Install/RadControls", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> CoreInstallDictionaries = AdvancedSettings.Create("Core/Install/Dictionaries", false);

    [NotNull]
    public static readonly AdvancedProperty<bool> CoreInstallNotFoundTransfer = AdvancedSettings.Create("Core/Install/NotFoundError/UseTransfer", true);

    [NotNull]
    public static readonly AdvancedProperty<string> CoreInstallMailServerAddress = AdvancedSettings.Create("Core/Install/MailServer/Address", "");

    [NotNull]
    public static readonly AdvancedProperty<string> CoreInstallMailServerCredentials = AdvancedSettings.Create("Core/Install/MailServer/Credentials", "");

    [NotNull]
    public static readonly AdvancedProperty<string> CoreExportTempFolderLocation = AdvancedSettings.Create("Core/Export/TempFolder/Location", "");

    [NotNull]
    public static readonly AdvancedProperty<string> CoreInstallTempFolderLocation = AdvancedSettings.Create("Core/Install/TempFolder/Location", "");

    [NotNull]
    public static readonly AdvancedProperty<bool> CoreDeleteMongoDatabases = AdvancedSettings.Create("Core/Delete/MongoDatabases", true);
  }
}
