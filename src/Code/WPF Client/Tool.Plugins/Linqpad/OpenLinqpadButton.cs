using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml;
using SIM.Base;
using SIM.Instances;
using SIM.Tool.Base;
using SIM.Tool.Base.Plugins;
using SIM.Tool.Plugins.Linqpad.Repairers;

namespace SIM.Tool.Plugins.Linqpad
{
  public class OpenLinqpadButton : IMainWindowButton
  {
    public static readonly AdvancedProperty<string> Namespaces = AdvancedSettings.Create("Plugins/Linqpad/Namespaces", @"Sitecore|Sitecore.Configuration|Sitecore.Data|Sitecore.Data.Items|Sitecore.Diagnostics|Sitecore.ContentSearch|Sitecore.ContentSearch.Linq");

    public static readonly Type[] RepairerTypes = new[]
      {
        typeof (SIM.Tool.Plugins.Linqpad.Repairers.AspNetRepairer),
        typeof (SIM.Tool.Plugins.Linqpad.Repairers.RoleManagerRepairer),
        typeof (SIM.Tool.Plugins.Linqpad.Repairers.Log4NetRepairer),
        typeof (SIM.Tool.Plugins.Linqpad.Repairers.ConfigStorePathRepairer),
        typeof (SIM.Tool.Plugins.Linqpad.Repairers.NonAsciiRepairer)
      };
    
    private readonly static string LinqpadFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + @"\LINQPad4\";
    private static readonly string LinqpadExecutableFilePath = Path.Combine(LinqpadFolderPath, "LINQPad.exe");
    private IEnumerable<IRepairer> repairers;

    public bool IsEnabled(Window mainWindow, Instance instance)
    {
      return true;
    }

    public void OnClick(Window mainWindow, Instance instance)
    {
      if (!FileSystem.Local.File.Exists(LinqpadExecutableFilePath))
      {
        WindowHelper.ShowMessage(
          @"LINQPad is not installed or installed in custom location. Please download plugin and install in default place.");
        return;
      }

      if (instance != null)
      {
        var linqpadConfiguration = PrepareConfig(instance);
        var configPath = Path.Combine(LinqpadFolderPath, "LINQPad.config");
        FileSystem.Local.File.WriteAllText(configPath, linqpadConfiguration);
        ModifyDefaultQueryXml(instance);
      }

      WindowHelper.RunApp(LinqpadExecutableFilePath);
    }

    private void ModifyDefaultQueryXml(Instance instance)
    {
      var defaultQueryXmlFilePath = Environment.ExpandEnvironmentVariables(@"%appdata%\LINQPad\DefaultQuery.xml");
      var document = XmlDocumentEx.LoadXml("<Query />");
      var binFolderPath = Path.Combine(instance.WebRootPath, "bin");
      foreach (var assmebly in FileSystem.Local.Directory.GetFiles(binFolderPath, "Sitecore.*.dll"))
      {
        Add(document, "Reference", assmebly);
      }
      var namespaces = Namespaces.Value.Split('|');
      foreach (var @namespace in namespaces)
      {
        Add(document, "Namespace", @namespace);
      }
      document.Save(defaultQueryXmlFilePath);
    }

    private void Add(XmlDocument document, string name, string value)
    {
      var referenceElement = document.CreateElement(name);
      referenceElement.InnerText = value;
      document.DocumentElement.AppendChild(referenceElement);
    }

    private string PrepareConfig(Instance instance)
    {
      var doc = instance.GetWebResultConfig();
      RunRepairers(doc, instance);
      using (var stringWriter = new StringWriter())
      {
        using (var xmlWriter = new XmlTextWriter(stringWriter))
        {
          xmlWriter.Formatting = Formatting.Indented;
          doc.WriteTo(xmlWriter);
          xmlWriter.Flush();
          return stringWriter.GetStringBuilder().ToString();
        }
      }
    }

    protected IEnumerable<IRepairer> Repairers
    {
      get { return repairers ?? (repairers = RepairerTypes.Select(Activator.CreateInstance).OfType<IRepairer>()); }
    }

    protected virtual void RunRepairers(XmlDocument doc, Instance instance)
    {
      Assert.ArgumentNotNull(doc, "doc");
      Assert.ArgumentNotNull(instance, "instance");

      foreach (var repairer in this.Repairers)
      {
        repairer.Repair(doc, instance);
      }
    }
  }
}
