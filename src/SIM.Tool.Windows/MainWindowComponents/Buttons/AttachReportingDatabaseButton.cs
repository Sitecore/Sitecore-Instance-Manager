using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using JetBrains.Annotations;
using SIM.Adapters.SqlServer;
using SIM.Core.Common;
using SIM.Extensions;
using SIM.Instances;
using SIM.IO.Real;
using SIM.Pipelines;
using SIM.Tool.Base;
using Sitecore.Diagnostics.Base;

namespace SIM.Tool.Windows.MainWindowComponents.Buttons
{
  public class AttachReportingDatabaseButton : InstanceOnlyButton
  {
    #region Public methods

    [UsedImplicitly]
    public AttachReportingDatabaseButton()
    {
    }

    public override void OnClick(Window mainWindow, Instance instance)
    {
      Assert.ArgumentNotNull(instance, nameof(instance));
                    
      WindowHelper.LongRunningTask(() => Process(instance), "Attaching database", mainWindow);
    }

    #endregion

    #region Private methods

    private static void Process(Instance instance)
    {
      var firstDatabase = instance.AttachedDatabases?.FirstOrDefault();
      Assert.IsNotNull(firstDatabase, "The databases information is not available");

      var databaseFilePath = firstDatabase.FileName.EmptyToNull();
      Assert.IsNotNull(databaseFilePath, nameof(databaseFilePath));

      var databasesFolderPath = Path.GetDirectoryName(databaseFilePath);
      Assert.IsNotNull(databasesFolderPath, nameof(databasesFolderPath));

      var reportingCstr = instance.Configuration.ConnectionStrings.FirstOrDefault(x => x.Name == "reporting");
      if (reportingCstr != null)
      {
        var sqlName = new SqlConnectionStringBuilder(reportingCstr.Value).InitialCatalog;
        var database = instance.AttachedDatabases.FirstOrDefault(x => x.RealName.Equals(sqlName));
        var fileName = database?.FileName;
        if (fileName != null)
        {
          var file = new FileInfo(fileName);
          Assert.IsTrue(!file.Exists, $"The {file.FullName} reporting database file already exist");
        }

        reportingCstr.Delete();
      }

      var reportingFile = new FileInfo(Path.Combine(databasesFolderPath, "Sitecore.Reporting.mdf"));
      if (reportingFile.Exists)
      {
        reportingFile.Delete();
      }

      AttachDatabasesHelper.ExtractReportingDatabase(instance, reportingFile);

      var sqlPrefix = AttachDatabasesHelper.GetSqlPrefix(instance);
      var reportingSqlName = sqlPrefix + "_reporting";

      var mgmtText = Profile.Read(new RealFileSystem()).ConnectionString;
      var mgmtValue = new SqlConnectionStringBuilder(mgmtText);
      SqlServerManager.Instance.AttachDatabase(reportingSqlName, reportingFile.FullName, mgmtValue);

      var reportingValue = new SqlConnectionStringBuilder(mgmtText)
      {
        InitialCatalog = reportingSqlName
      };

      instance.Configuration.ConnectionStrings.Add("reporting", reportingValue);
    }

    #endregion
  }
}