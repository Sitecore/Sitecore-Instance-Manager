using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using Sitecore.Diagnostics.Base;
using JetBrains.Annotations;

namespace SIM.FileSystem
{
  using Sitecore.Diagnostics.Logging;
  using SIM.Extensions;

  public static class SecurityExtensions
  {
    #region Fields

    private static Type SecurityIdentifier { get; } = typeof(SecurityIdentifier);     

    #endregion

    #region Public methods

    public static bool CompareTo(this IdentityReference left, IdentityReference right)
    {
      return left != null && right != null
        ? left.Translate(SecurityIdentifier).ToString().EqualsIgnoreCase(right.Translate(SecurityIdentifier).ToString())
        : left == right;
    }

    #endregion
  }

  public class SecurityProvider
  {
    #region Fields
                                          
    protected IdentityReference Everyone { get; } = new SecurityIdentifier("S-1-1-0").Translate(typeof(NTAccount));
    protected IdentityReference LocalService { get; } = new SecurityIdentifier("S-1-5-19").Translate(typeof(NTAccount));
    protected IdentityReference LocalSystem { get; } = new SecurityIdentifier("S-1-5-18").Translate(typeof(NTAccount));
    protected IdentityReference NetworkService { get; } = new SecurityIdentifier("S-1-5-20").Translate(typeof(NTAccount));

    protected FileSystem FileSystem { get; }

    #endregion

    #region Constructors

    public SecurityProvider(FileSystem fileSystem)
    {
      FileSystem = fileSystem;
    }

    #endregion

    #region Public methods

    public virtual void EnsurePermissions([NotNull] string path, [NotNull] string identity)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(identity, nameof(identity));

      var identityReference = GetIdentityReference(identity);
      Assert.IsNotNull(identityReference, "Cannot find {0} identity reference".FormatWith(identity));

      if (FileSystem.Directory.Exists(path))
      {
        EnsureDirectoryPermissions(path, identityReference);
        return;
      }

      if (FileSystem.File.Exists(path))
      {
        EnsureFilePermissions(path, identityReference);
        return;
      }

      throw new InvalidOperationException("File or directory not found: " + path);
    }

    [CanBeNull]
    public virtual IdentityReference GetIdentityReference([NotNull] string name)
    {
      Assert.ArgumentNotNullOrEmpty(name, nameof(name));

      IdentityReference reference = null;
      if (name.EndsWith("NetworkService", StringComparison.OrdinalIgnoreCase) || name.EndsWith("Network Service", StringComparison.OrdinalIgnoreCase))
      {
        reference = NetworkService;
      }
      else if (name.EndsWith("LocalSystem", StringComparison.OrdinalIgnoreCase) || name.EndsWith("Local System", StringComparison.OrdinalIgnoreCase))
      {
        reference = LocalSystem;
      }
      else if (name.EndsWith("LocalService", StringComparison.OrdinalIgnoreCase) || name.EndsWith("Local Service", StringComparison.OrdinalIgnoreCase))
      {
        reference = LocalService;
      }
      else
      {
        try
        {
          if (!name.Contains("\\"))
          {
            name = Environment.MachineName + "\\" + name.TrimStart("\\");
          }
          else if (name.StartsWith(".\\"))
          {
            name = Environment.MachineName + "\\" + name.TrimStart(".\\");
          }

          reference = new SecurityIdentifier(name).Translate(typeof(NTAccount));
        }
        catch (Exception ex)
        {
          Log.Warn(ex, $"An error occurred during paring {name} security identifier");
          try
          {
            reference = new NTAccount(name);
          }
          catch (Exception ex1)
          {
            Log.Warn(ex, $"An error occurred during parsing {ex1} user account");
          }
        }
      }

      Assert.IsNotNull(reference, "The '" + name + "' isn't valid NTAccount");

      return reference;
    }

    public virtual bool HasPermissions(string path, string identity, FileSystemRights permissions)
    {
      Assert.ArgumentNotNullOrEmpty(path, nameof(path));
      Assert.ArgumentNotNullOrEmpty(identity, nameof(identity));
      Assert.ArgumentNotNull(permissions, nameof(permissions));

      if (FileSystem.Directory.Exists(path))
      {
        return HasDirectoryPermissions(path, GetIdentityReference(identity), permissions);
      }

      if (FileSystem.File.Exists(path))
      {
        return HasFilePermissions(path, GetIdentityReference(identity), permissions);
      }

      throw new InvalidOperationException("File or directory not found: " + path);
    }

    #endregion

    #region Protected methods

    protected virtual void EnsureDirectoryPermissions([NotNull] string path, [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(path, nameof(path));
      Assert.ArgumentNotNull(identity, nameof(identity));

      DirectoryInfo dirInfo = new DirectoryInfo(path);
      DirectorySecurity dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      if (!HasPermissions(rules, identity, FileSystemRights.FullControl))
      {
        Log.Info(string.Format("Granting full access for '{0}' identity to the '{1}' folder", identity, path, 
          typeof(FileSystem)));
        FileSystemAccessRule rule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, 
          InheritanceFlags.ContainerInherit |
          InheritanceFlags.ObjectInherit, PropagationFlags.None, 
          AccessControlType.Allow);
        dirSecurity.AddAccessRule(rule);
        dirInfo.SetAccessControl(dirSecurity);

        dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
        rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
        Assert.IsTrue(HasPermissions(rules, identity, FileSystemRights.FullControl), 
          "The Full Control access to the '" + path + "' folder isn't permitted for " + identity.Value +
          ". Please fix it and then restart the process");
      }
    }

    protected virtual void EnsureFilePermissions([NotNull] string path, [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(path, nameof(path));
      Assert.ArgumentNotNull(identity, nameof(identity));

      var fileInfo = new FileInfo(path);
      var dirSecurity = fileInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      if (!HasPermissions(rules, identity, FileSystemRights.FullControl))
      {
        Log.Info(string.Format("Granting full access for '{0}' identity to the '{1}' file", identity, path, 
          typeof(FileSystem)));

        var rule = new FileSystemAccessRule(identity, FileSystemRights.FullControl, AccessControlType.Allow);
        dirSecurity.AddAccessRule(rule);
        fileInfo.SetAccessControl(dirSecurity);

        dirSecurity = fileInfo.GetAccessControl(AccessControlSections.All);
        rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
        Assert.IsTrue(HasPermissions(rules, identity, FileSystemRights.FullControl), 
          "The Full Control access to the '" + path + "' file isn't permitted for " + identity.Value +
          ". Please fix it and then restart the process");
      }
    }

    [NotNull]
    protected virtual IEnumerable<AuthorizationRule> GetRules([NotNull] AuthorizationRuleCollection rules, 
      [NotNull] IdentityReference identity)
    {
      Assert.ArgumentNotNull(rules, nameof(rules));
      Assert.ArgumentNotNull(identity, nameof(identity));

      try
      {
        return rules.Cast<AuthorizationRule>().Where(rule => rule.IdentityReference.CompareTo(identity) || rule.IdentityReference.CompareTo(Everyone));
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"Cannot get rules. {ex.Message}");
        return new AuthorizationRule[0];
      }
    }

    protected virtual bool HasDirectoryPermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      DirectoryInfo dirInfo = new DirectoryInfo(path);
      DirectorySecurity dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      return HasPermissions(rules, identity, permissions);
    }

    protected virtual bool HasFilePermissions(string path, IdentityReference identity, FileSystemRights permissions)
    {
      var dirInfo = new FileInfo(path);
      var dirSecurity = dirInfo.GetAccessControl(AccessControlSections.All);
      AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));

      return HasPermissions(rules, identity, permissions);
    }

    protected virtual bool HasPermissions([NotNull] AuthorizationRuleCollection rules, [NotNull] IdentityReference identity, FileSystemRights permissions)
    {
      Assert.ArgumentNotNull(rules, nameof(rules));
      Assert.ArgumentNotNull(identity, nameof(identity));
      try
      {
        return
          GetRules(rules, identity).Any(
            rule => (((FileSystemAccessRule)rule).FileSystemRights & permissions) > 0);
      }
      catch (Exception ex)
      {
        Log.Warn(ex, "Cannot get permissions for rules collection");
        return false;
      }
    }

    #endregion
  }
}