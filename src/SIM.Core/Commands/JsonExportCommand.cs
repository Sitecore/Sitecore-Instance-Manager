namespace SIM.Core.Commands
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Database.Items;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.Diagnostics.SqlDataProvider.Items;
  using SIM.Core.Common;

  public class JsonExportCommand : AbstractCommand
  {
    private readonly string[] SystemFieldIDs =
    {
      Guid.Parse("{25BED78C-4957-4165-998A-CA1B52F67497}").ToString("D"), // created
      Guid.Parse("{5DD74568-4D4B-44C1-B513-0AF5F4CDA34F}").ToString("D"), // created by
      Guid.Parse("{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}").ToString("D"), // updated
      Guid.Parse("{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}").ToString("D"), // updated by
      Guid.Parse("{ba3f86a2-4a1c-4d78-b63d-91c2779c1b5e}").ToString("D"), // sortorder
      Guid.Parse("{8cdc337e-a112-42fb-bbb4-4143751e123f}").ToString("D"), // revision
    };


    public virtual string Database { get; set; }

    public virtual string ItemName { get; set; }

    public virtual string OutputFile { get; set; }

    protected const bool SystemFieldsDefault = false;
    public virtual bool? SystemFields { get; set; } = SystemFieldsDefault;

    public virtual string ConnectionString { get; set; }

    public virtual string Name { get; set; }

    protected const bool SortDefault = false;
    public virtual bool? Sort { get; set; } = SortDefault;

    public virtual string IgnoreFieldIDs { get; set; }

    private bool StripSystemFields => !(SystemFields ?? SystemFieldsDefault);

    protected override void DoExecute(CommandResult result)
    {
      var connectionString = ConnectionString;
      if (string.IsNullOrEmpty(connectionString))
      {
        Assert.ArgumentNotNull(Name, nameof(Name));
        Assert.ArgumentNotNull(Database, nameof(Database));

        var instance = AbstractInstanceActionCommand.GetInstance(Name);

        connectionString = instance.Configuration.ConnectionStrings[Database]?.Value;
        Assert.IsNotNullOrEmpty(connectionString, nameof(connectionString));
      }

      using (var writer = new StreamWriter(File.OpenWrite(OutputFile)))
      {
        var context = ItemManager.Initialize(connectionString);
        var items = context.GetItems();
        var rootItemId = GetRootItemId(items);

        var tree = BuildTree(rootItemId, items, Sort ?? SortDefault);

        var ignoreFieldsArrayIDs = IgnoreFieldIDs?.Split("|;,".ToCharArray(), StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        var ignoreFieldIDs = ignoreFieldsArrayIDs
          .Select(Guid.Parse)
          .Select(x => x.ToString("D"))
          .ToArray();

        if (StripSystemFields || ignoreFieldIDs.Any())
        {
          StripFields(tree, ignoreFieldIDs);
        }

        var serializer = new JsonSerializer
        {
          Formatting = Formatting.Indented
        };

        serializer.Serialize(writer, tree);
      }
    }

    private Guid GetRootItemId(IQueryable<Item> items)
    {
      var rootItemId = Guid.Parse("11111111-1111-1111-1111-111111111111");
      if (!string.IsNullOrEmpty(ItemName))
      {
        if (!Guid.TryParse(ItemName, out rootItemId))
        {
          Assert.IsTrue(ItemName.StartsWith("/sitecore", StringComparison.OrdinalIgnoreCase), "Invalid item path: " + ItemName);
          var itemNames = ItemName.Split("/".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
          var path = "/sitecore";
          Item rootItem = items.FirstOrDefault(x => x.ParentID == Guid.Empty);

          foreach (var itemName in itemNames.Skip(1))
          {
            path += $"/{itemName}";
            rootItem = items.FirstOrDefault(x => x.ParentID == rootItem.ID && x.Name == itemName);
            Assert.IsNotNull(rootItem, $"Item path is not found: {path}");
          }

          rootItemId = rootItem.ID;
        }
      }
      return rootItemId;
    }

    private void StripFields(ContentItem item, string[] ignoreFieldIDs)
    {
      var fields = item.Fields.Shared;
      this.StripFields(fields, ignoreFieldIDs);

      foreach (var language in item.Fields.Unversioned)
      {
        this.StripFields(language.Value, ignoreFieldIDs);
      }

      foreach (var language in item.Fields.Versioned)
      {
        foreach (var version in language.Value)
        {
          this.StripFields(version.Value, ignoreFieldIDs);
        }
      }

      foreach (var child in item.Children)
      {
        this.StripFields((ContentItem)child, ignoreFieldIDs);
      }
    }

    private void StripFields([NotNull] FieldsCollection fields, string[] ignoreFieldIDs)
    {
      Assert.ArgumentNotNull(fields, "fields");

      if (StripSystemFields)
      {
        foreach (var fieldId in this.SystemFieldIDs)
        {
          fields.Remove(fieldId);
        }
      }

      foreach (var fieldId in ignoreFieldIDs)
      {
        fields.Remove(fieldId);
      }
    }

    private ContentItem BuildTree(Guid rootItemId, IQueryable<Item> items, bool sort)
    {
      var rootDataItem = items.SingleOrDefault(x => x.ID == rootItemId);
      Assert.IsNotNull(rootDataItem, "rootDataItem");

      Log.Info("Building item tree");
      var rootItem = new ContentItem(rootDataItem, null);

      Log.Info("Building children items, Level: 1");
      var children = PopulateChildren(items, rootItem, sort).ToArray();

      Log.Info("> Done. Items loaded: {0}", children.Length);

      var level = 2;
      while (children.Length > 0)
      {
        Log.Info("Building children items, Level: {0}", level++);
        children = children // take old children
          .SelectMany(x => PopulateChildren(items, x, sort)) // populate children for all of them
          .ToArray(); // save back to 'children' variable for next iteration

        Log.Info("> Done. Items loaded: {0}", children.Length);
      }

      return rootItem;
    }

    [NotNull]
    private static ContentItem[] PopulateChildren([NotNull] IEnumerable<Item> items, [NotNull] ContentItem parentItem, bool sort)
    {
      Assert.ArgumentNotNull(items, "items");
      Assert.ArgumentNotNull(parentItem, "parentItem");

      var query = items
        .Where(x => x.ParentID == parentItem.ID)
        .Select(x => new ContentItem(x, parentItem));

      if (sort)
      {
        query = query.OrderBy(x => x.ID);
      }

      var children = query.ToArray();

      parentItem.SetChildren(children);

      return children;
    }

    private sealed class ContentItem : Item
    {
      public ContentItem([NotNull] Item item, [CanBeNull] Item parent)
      {
        Assert.ArgumentNotNull(item, "item");

        this.ID = item.ID;
        this.TemplateID = item.TemplateID;
        this.Name = item.Name;
        this.Children = item.Children;
        this.Fields = item.Fields;

        this.ParentID = parent != null ? parent.ID : Guid.Empty;
      }

      [UsedImplicitly]
      public override Guid ID { get; protected set; }

      [UsedImplicitly]
      public override string Name { get; protected set; }

      [UsedImplicitly]
      public override Guid TemplateID { get; protected set; }

      [JsonIgnore]
      public override Guid ParentID { get; protected set; }

      [JsonIgnore]
      [Obsolete("Do not use this property. It was only overridden to mark it JsonIgnore.")]
      public override string ItemPath { get; protected set; }

      [JsonIgnore]
      [Obsolete("Do not use this property. It was only overridden to mark it JsonIgnore.")]
      public override DateTime Created { get; protected set; }

      [JsonIgnore]
      [Obsolete("Do not use this property. It was only overridden to mark it JsonIgnore.")]
      public override DateTime Updated { get; protected set; }

      [UsedImplicitly]
      public override Fields Fields { get; protected set; }

      [JsonIgnore]
      [Obsolete("Do not use this property. It was only overridden to mark it JsonIgnore.")]
      public override Children Children { get; protected set; }

      [JsonProperty("Children")]
      public Dictionary<Guid, ContentItem> NewChildren { [UsedImplicitly] get; protected set; }

      public void RemoveFields()
      {
        this.Fields = null;
      }

      public void SetChildren([NotNull] IEnumerable<ContentItem> children)
      {
        Assert.ArgumentNotNull(children, "children");

        this.NewChildren = children.ToDictionary(x => x.ID, x => x);
      }
    }
  }
}