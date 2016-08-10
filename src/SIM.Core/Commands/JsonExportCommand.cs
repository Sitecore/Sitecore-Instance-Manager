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
  using SIM.Extensions;
  using SIM.Instances;

  public class JsonExportCommand : AbstractInstanceActionCommand
  {
    private readonly string[] FieldIDs =
    {
      Guid.Parse("{25BED78C-4957-4165-998A-CA1B52F67497}").ToString("D"),
      Guid.Parse("{5DD74568-4D4B-44C1-B513-0AF5F4CDA34F}").ToString("D"),
      Guid.Parse("{D9CF14B1-FA16-4BA6-9288-E8A174D4D522}").ToString("D"),
      Guid.Parse("{BADD9CF9-53E0-4D0C-BCC0-2D784C282F6A}").ToString("D")
    };

    public virtual string Database { get; set; }   

    public virtual string ItemName { get; set; }    

    public virtual string OutputFile { get; set; }

    public virtual bool? SystemFields { get; set; }

    protected override void DoExecute(Instance instance, CommandResult result)
    {
      Assert.ArgumentNotNull(Database, nameof(Database));
                                                                                                                     
      var context = ItemManager.Initialize(instance.Configuration.ConnectionStrings[Database].Value);
      var items = context.GetItems();
      var rootItemId = Guid.Parse("11111111-1111-1111-1111-111111111111");
      bool filter = false;
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

      var tree = BuildTree(rootItemId, items);

      if (SystemFields == null || !SystemFields.Value)
      {
        StripSystemFields(tree);
      }

      File.WriteAllText(OutputFile, JsonConvert.SerializeObject(tree, Formatting.Indented));
    }

    private void StripSystemFields(ContentItem item)
    {
      var fields = item.Fields.Shared;
      this.StripSystemFields(fields);

      foreach (var language in item.Fields.Unversioned)
      {
        this.StripSystemFields(language.Value);
      }

      foreach (var language in item.Fields.Versioned)
      {
        foreach (var version in language.Value)
        {
          this.StripSystemFields(version.Value);
        }
      }

      foreach (var child in item.Children)
      {
        this.StripSystemFields((ContentItem)child);
      }
    }

    private void StripSystemFields([NotNull] FieldsCollection fields)
    {
      Assert.ArgumentNotNull(fields, "fields");

      foreach (var fieldId in this.FieldIDs)
      {
        fields.Remove(fieldId);
      }
    }

    private ContentItem BuildTree(Guid rootItemId, IQueryable<Item> items)
    {                                     
      var rootDataItem = items.SingleOrDefault(x => x.ID == rootItemId);
      Assert.IsNotNull(rootDataItem, "rootDataItem");

      Log.Info("Building item tree");
      var rootItem = new ContentItem(rootDataItem, null);

      Log.Info("Building children items, Level: 1");
      var children = PopulateChildren(items, rootItem).ToArray();

      Log.Info("> Done. Items loaded: {0}", children.Length);

      var level = 2;
      while (children.Length > 0)
      {
        Log.Info("Building children items, Level: {0}", level++);
        children = children // take old children
          .SelectMany(x => PopulateChildren(items, x)) // populate children for all of them
          .ToArray(); // save back to 'children' variable for next iteration

        Log.Info("> Done. Items loaded: {0}", children.Length);
      }
                                           
      return rootItem;
    }

    [NotNull]
    private static ContentItem[] PopulateChildren([NotNull] IEnumerable<Item> items, [NotNull] ContentItem parentItem)
    {
      Assert.ArgumentNotNull(items, "items");
      Assert.ArgumentNotNull(parentItem, "parentItem");

      var children = items
        .Where(x => x.ParentID == parentItem.ID)
        .Select(x => new ContentItem(x, parentItem))
        .ToArray();

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

      [UsedImplicitly]
      public override Children Children { get; protected set; }

      public void RemoveFields()
      {
        this.Fields = null;
      }

      public void SetChildren([NotNull] ContentItem[] children)
      {
        Assert.ArgumentNotNull(children, "children");

        this.Children = new ContentChildren(children);
      }
    }

    private class ContentChildren : Children
    {
      public ContentChildren([NotNull] IEnumerable<ContentItem> children) : base(children)
      {
        Assert.ArgumentNotNull(children, "children");
      }
    }
  }
}