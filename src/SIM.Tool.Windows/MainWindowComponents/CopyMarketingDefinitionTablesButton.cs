namespace SIM.Tool.Windows.MainWindowComponents
{
  using System.Data.SqlClient;
  using System.Linq;
  using System.Windows;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using SIM.Instances;
  using SIM.Tool.Base;
  using SIM.Tool.Base.Plugins;

  public class CopyMarketingDefinitionTablesButton : IMainWindowButton
  {
    [UsedImplicitly]
    public CopyMarketingDefinitionTablesButton()
    {
    }

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return instance != null && instance.Configuration.ConnectionStrings.Any(x => x.Name == "reporting.secondary");
    }

    public void OnClick(Window mainWindow, Instance instance)
    {                                                                                         
      WindowHelper.LongRunningTask(() => Process(instance), "Copying definitions", mainWindow);
    }

    private static void Process(Instance instance)
    {
      var connectionStrings = instance.Configuration.ConnectionStrings;

      var primary = connectionStrings.FirstOrDefault(x => x.Name == "reporting");
      Assert.IsNotNull(primary, nameof(primary));

      var secondary = connectionStrings.FirstOrDefault(x => x.Name == "reporting.secondary");
      Assert.IsNotNull(secondary, nameof(secondary));

      var tablesNames = new[] {"CampaignActivityDefinitions", "GoalDefinitions", "OutcomeDefinitions", "MarketingAssetDefinitions", "Taxonomy_TaxonEntity", "Taxonomy_TaxonEntityFieldDefinition", "Taxonomy_TaxonEntityFieldValue"};

      using (var connection = new SqlConnection(primary.Value))
      {
        connection.Open();

        using (var connection2 = new SqlConnection(secondary.Value))
        {
          connection2.Open();
          foreach (var tableName in tablesNames)
          {
            using (var command = new SqlCommand("DELETE FROM [" + tableName + "]", connection2))
            {
              command.ExecuteNonQuery();
            }
          }
        }

        using (var command = connection.CreateCommand())
        {
          using (var copy = new SqlBulkCopy(secondary.Value))
          {
            foreach (var tableName in tablesNames)
            {
              copy.DestinationTableName = tableName;
              command.CommandText = "SELECT * FROM [" + tableName + "]";

              using (SqlDataReader reader = command.ExecuteReader())
              {
                copy.WriteToServer(reader);
              }
            }
          }
        }
      }
    }
  }
}