namespace SIM.Pipelines.Agent
{
  public static class AgentFiles
  {
    #region InstallPackage

    public const string InstallPackageContents = @"
<%@ Page Language=""C#"" AutoEventWireup=""true"" %>
<%@ Import Namespace=""System.IO"" %>
<%@ Import Namespace=""Sitecore.Configuration"" %>
<%@ Import Namespace=""Sitecore.Data.Engines"" %>
<%@ Import Namespace=""Sitecore.Data.Proxies"" %>
<%@ Import Namespace=""Sitecore.Diagnostics"" %>
<%@ Import Namespace=""Sitecore.Install"" %>
<%@ Import Namespace=""Sitecore.Install.Files"" %>
<%@ Import Namespace=""Sitecore.Install.Framework"" %>
<%@ Import Namespace=""Sitecore.Install.Items"" %>
<%@ Import Namespace=""Sitecore.Install.Utils"" %>
<%@ Import Namespace=""Sitecore.SecurityModel"" %>
<%@ Import Namespace=""Sitecore.Security.Accounts"" %>

<script runat=""server"">

#region Methods

  protected override void OnInitComplete(EventArgs e)
  {
	  this.Server.ScriptTimeout = 10*60*60; // 10 hours should be enough
    var fileName = this.Request.QueryString[""fileName""];
    if (string.IsNullOrEmpty(fileName))
    {
      this.Finish(""Error: fileName is empty"");
      return;
    }

    var filePath = GetFilePath(fileName);
    if (!File.Exists(filePath))
    {
      this.Finish(""Error: the '"" + filePath + ""' file doesn't exists"");
      return;
    }

    try
    {
      Sitecore.Context.SetActiveSite(""shell"");
      using (new UserSwitcher(""sitecore\\admin"", true))
      {
        using (new ProxyDisabler())
        {
          using (new SyncOperationContext())
          {
            SimpleProcessingContext context = new SimpleProcessingContext();
            DefaultItemInstallerEvents events = new DefaultItemInstallerEvents(new BehaviourOptions(InstallMode.Overwrite, MergeMode.Undefined));
            context.AddAspect(events);
            DefaultFileInstallerEvents events1 = new DefaultFileInstallerEvents(true);
            context.AddAspect(events1);
            this.UpdateStatus(@""Started: package is being installed"");
            new Installer().InstallPackage(filePath, context);
            new Installer().InstallSecurity(filePath, context);
            this.Finish(@""Done: package installed"");
          }
        }
      }
    }
    catch (Exception ex)
    {
      var inn = string.Empty;
      if (ex.InnerException != null)
      {
        inn = ""\n\nInner Exception:\n"" + ex.InnerException;
      }

      this.Finish(@""Error: "" + ex + inn);
    }
  }

  private void Finish(string message)
  {
    Log.Info(@""[SIM] " + AgentFiles.InstallPackageFileName + @": "" + message, this);
    this.UpdateStatus(message);
    this.Response.Write(message);
  }

  private void UpdateStatus(string message)
  {
    var installedTemp = this.Server.MapPath(Path.Combine(Settings.TempFolderPath, ""sim.status""));
    File.WriteAllText(installedTemp, message);
  }

  private string GetFilePath(string name)
  {
    Assert.ArgumentNotNullOrEmpty(name, ""name"");

    var packageFolderPath = Sitecore.Configuration.Settings.PackagePath;
    Assert.IsNotNullOrEmpty(packageFolderPath, ""packageFolderPath"");

    // if path is virtual i.e. not C:\something then do a map path
    if (packageFolderPath.Length < 2 || packageFolderPath[1] != ':')
    {
      packageFolderPath = packageFolderPath.TrimStart('/');
      var prefix = ""~/"";
      if (packageFolderPath.StartsWith(prefix))
      {
        packageFolderPath = packageFolderPath.Substring(prefix.Length);
      }

      packageFolderPath = Server.MapPath(prefix + packageFolderPath);
    }
      
    return Path.Combine(packageFolderPath, name);      
  }

#endregion

</script>";
    public const string InstallPackageFileName = @"InstallPackage.aspx";

    #endregion

    #region PostInstallActions

    public const string PostInstallActionsContents = @"<%@ Page Language=""C#"" AutoEventWireup=""true"" %>
<%@ Import Namespace=""System.IO"" %>
<%@ Import Namespace=""System.Reflection"" %>
<%@ Import Namespace=""System.Threading"" %>
<%@ Import Namespace=""Sitecore"" %>
<%@ Import Namespace=""Sitecore.Configuration"" %>
<%@ Import Namespace=""Sitecore.Install"" %>
<%@ Import Namespace=""Sitecore.Install.Framework"" %>
<%@ Import Namespace=""Sitecore.Install.Metadata"" %>
<%@ Import Namespace=""Sitecore.Install.Zip"" %>
<%@ Import Namespace=""Sitecore.Jobs"" %>
<%@ Import Namespace=""Sitecore.Diagnostics"" %>

<script runat=""server"">

#region Fields

private string path;

#endregion

#region Methods

  protected override void OnInitComplete(EventArgs e)
  {
	  this.Server.ScriptTimeout = 10*60*60; // 10 hours should be enough
    var filename = this.Request.QueryString[""fileName""];
    if (string.IsNullOrEmpty(filename))
    {
      var value = this.Request.QueryString[""custom""];
      if (string.IsNullOrEmpty(value))
      {
        this.Finish(""Error: 'fileName' or 'custom' is empty"");
        return;
      }

      try
      {
        string[] pairValues = value.Split(';');
        if (pairValues.Length > 0)
        {
          foreach (string pairValue in pairValues)
          {
            string[] pair = pairValue.Split('-');
            var className = pair[0];
            Type type = Type.GetType(className);
            Assert.IsNotNull(type, ""type: "" + className);
            ConstructorInfo ctor = type.GetConstructor(new Type[0]);
            Assert.IsNotNull(ctor, ""ctor: "" + className);
            object instance = ctor.Invoke(new object[0]);
            var methodName = pair[1];
            MethodInfo method = type.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            var infoMessage = ""method: "" + methodName + "" of class: "" + className;
            Assert.IsNotNull(method, infoMessage);
            Log.Info(""Custom PostStepAction executing: "" + infoMessage, this);
            this.UpdateStatus(""Started: actions performing"");
            method.Invoke(instance, new object[0]);
            this.Finish(@""Done: actions performed"");
          }

          return;
        }
      }
      catch (Exception ex)
      {
        this.Finish(""Error: "" + ex.Message);        
        return;
      }
    } 
      
    this.path = GetFilePath(filename);

    if (!File.Exists(this.path))
    {
      this.Finish(""Error: the '"" + this.path + ""' file doesn't exists"");
      return;
    }

    try
    {
      this.UpdateStatus(""Started: actions performing"");
      Job job = new Job(new JobOptions(""PostInstallActions"", ""SIM"", ""shell"", this, ""Run""));
      job.Start();
      while (!job.IsDone)
      {
        Thread.Sleep(1000);
      }

      this.Finish(@""Done: actions performed"");
    }
    catch (Exception ex)
    {
      var inn = string.Empty;
      if (ex.InnerException != null)
      {
        inn = ""\n\nInner Exception:\n"" + ex.InnerException;
      }

      this.Finish(@""Error: "" + ex + inn);
    }
  }

  [UsedImplicitly]
  private void Run()
  {
    IProcessingContext context2 = Installer.CreatePreviewContext();
    ISource<PackageEntry> source = new PackageReader(this.path);
    MetadataView view = new MetadataView(context2);
    MetadataSink sink = new MetadataSink(view);
    sink.Initialize(context2);
    source.Populate(sink);
    new Installer().ExecutePostStep(view.PostStep, context2);
  }

  private void UpdateStatus(string message)
  {
    var tempFolderPath = Settings.TempFolderPath;
    Assert.IsNotNullOrEmpty(tempFolderPath, ""Settings.TempFolderPath"");  
    var installedTemp = this.Server.MapPath(Path.Combine(tempFolderPath, ""sim.status""));
    File.WriteAllText(installedTemp, message);
    Log.Info(@""[SIM] " + AgentFiles.InstallPackageFileName + @": "" + message, this);
  }

  private void Finish(string message)
  {
    Log.Info(@""[SIM] " + AgentFiles.InstallPackageFileName + @": "" + message, this);
    var tempFolderPath = Settings.TempFolderPath;
    Assert.IsNotNullOrEmpty(tempFolderPath, ""Settings.TempFolderPath"");  
    var installedTemp = this.Server.MapPath(Path.Combine(tempFolderPath, ""sim.status""));
    File.WriteAllText(installedTemp, message);
    this.Response.Write(message);
  }

  private string GetFilePath(string name)
  {
    Assert.ArgumentNotNullOrEmpty(name, ""name"");

    var packageFolderPath = Sitecore.Configuration.Settings.PackagePath;
    Assert.IsNotNullOrEmpty(packageFolderPath, ""packageFolderPath"");

    // if path is virtual i.e. not C:\something then do a map path
    if (packageFolderPath.Length < 2 || packageFolderPath[1] != ':')
    {
      packageFolderPath = packageFolderPath.TrimStart('/');
      var prefix = ""~/"";
      if (packageFolderPath.StartsWith(prefix))
      {
        packageFolderPath = packageFolderPath.Substring(prefix.Length);
      }

      packageFolderPath = Server.MapPath(prefix + packageFolderPath);
    }
      
    return Path.Combine(packageFolderPath, name);      
  }

#endregion

</script>
";
    public const string PostInstallActionsFileName = @"PostInstallActions.aspx";

    #endregion

    #region Status

    public const string StatusContents = @"<%@ Page Language=""C#"" AutoEventWireup=""true"" %>
<%@ Import Namespace=""System.IO"" %>
<%@ Import Namespace=""Sitecore.Configuration"" %>
<%@ Import Namespace=""Sitecore.Diagnostics"" %>
<script runat=""server"">

#region Methods

  protected override void OnInitComplete(EventArgs e)
  {
	  this.Server.ScriptTimeout = 10*60*60; // 10 hours should be enough
    var installedTemp = this.Server.MapPath(Path.Combine(Settings.TempFolderPath, ""sim.status""));
    var message;
    if (File.Exists(installedTemp))
    {
      message = File.ReadAllText(installedTemp);
      File.Delete(installedTemp);
    }
    else
    {
      message = @""Pending: no information"";
    }

    Log.Info(@""[SIM] " + AgentFiles.StatusFileName + @": "" + message, this);
    this.Response.Write(message);
  }

#endregion

</script>";
    public const string StatusFileName = @"Status.aspx";

    #endregion
  }
}