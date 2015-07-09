namespace SIM.Tool.Plugins.TrayPlugin.Resourcing
{
  using System.Drawing;

  public class MultisourceResourcesManager
  {
    #region Fields

    private static ResourcesProvider actualProvider;

    #endregion

    #region Public properties

    public static ResourcesProvider ActualProvider
    {
      // Lazy to allow other principals to override it.
      get
      {
        if (actualProvider != null)
        {
          return actualProvider;
        }

        actualProvider = new ResourcesProvider();
        return actualProvider;
      }

      set
      {
        actualProvider = value;
      }
    }

    #endregion

    #region Public methods

    public static Icon GetIconResource(string iconResourceName, Icon defaultValue)
    {
      return ActualProvider.GetIconResource(iconResourceName, defaultValue);
    }

    public static string GetStringResource(string stringResourceName, string defaultValue)
    {
      return ActualProvider.GetStringResource(stringResourceName, defaultValue);
    }

    #endregion
  }
}