namespace SIM.Client
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using System.Security.Principal;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using log4net.Core;
  using log4net.Layout;
  using log4net.Util;
  using SIM.Client.Serialization;
  using SIM.Core;
  using SIM.Core.Common;
  using SIM.Core.Logging;

  public static class Program
  {
    public static void Main([NotNull] string[] args)
    {
      Assert.ArgumentNotNull(args, nameof(args));

      RunDiagnostics();

      var filteredArgs = args.ToList();
      var query = GetQueryAndFilterArgs(filteredArgs);
      var wait = GetWaitAndFilterArgs(filteredArgs);

      if (!EnsureAccessRights(wait))
      {
        Environment.Exit(403);
        return;
      }

      var parseArguments = new ParseArgumentsCommand
      {
        Args = filteredArgs,
        Autocomplete = true,
        HelpWriter = Console.Error
      };

      var selectedCommand = parseArguments.Execute();
      if (selectedCommand == null)
      {
        Console.WriteLine("Note, commands provide output when work is done i.e. without any progress indication.");
        Console.WriteLine("\r\n  --query\t   When specified, allows returning only part of any command's output");
        Console.WriteLine("\r\n  --data\t   When specified, allows returning only 'data' part of any command's output");
        Console.WriteLine("\r\n  --wait\t   When specified, waits for keyboard input before terminating");

        if (wait)
        {
          Console.ReadKey();
        }

        Environment.Exit(666);
        return;
      }

      var commandResult = selectedCommand.Execute();
      Assert.IsNotNull(commandResult, nameof(commandResult));

      if (!ProcessResult(commandResult, query))
      {
        return;
      }

      if (wait)
      {
        Console.ReadKey();
      }
    }

    private static void RunDiagnostics()
    {
      InitializeLogging();

      CoreApp.LogMainInfo();

      Analytics.Start();
    }

    private static bool ProcessResult([NotNull] CommandResult commandResult, [CanBeNull] string query)
    {
      Assert.IsNotNull(commandResult, nameof(commandResult));

      var result = QueryResult(commandResult, query);
      if (result == null)
      {
        return false;
      }

      var serializer = new JsonSerializer
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented
      };

      serializer.Converters.Add(new DirectoryInfoConverter());

      var writer = Console.Out;
      serializer.Serialize(writer, result);

      return true;
    }


    private static bool EnsureAccessRights(bool wait)
    {
      if (new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
      {
        return true;
      }

      Console.WriteLine("Current user account is not Administrator. Please re-run using Administrator user account.");

      if (wait)
      {
        Console.ReadKey();
      }

      return false;
    }

    private static void InitializeLogging()
    {
      var info = new LogFileAppender
      {
        AppendToFile = true,
        File = "$(currentFolder)\\sim.log",
        Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
        SecurityContext = new WindowsSecurityContext(),
        Threshold = Level.Info
      };

      var debug = new LogFileAppender
      {
        AppendToFile = true,
        File = "$(currentFolder)\\sim.debug",
        Layout = new PatternLayout("%4t %d{ABSOLUTE} %-5p %m%n"),
        SecurityContext = new WindowsSecurityContext(),
        Threshold = Level.Debug
      };

      CoreApp.InitializeLogging(info, debug);
    }

    [CanBeNull]
    private static object QueryResult([NotNull] CommandResult result, [CanBeNull] string query)
    {
      Assert.ArgumentNotNull(result, nameof(result));

      if (string.IsNullOrEmpty(query) || !result.Success)
      {
        return result;
      }

      object obj = result;
      foreach (var chunk in query.Split("./".ToCharArray()))
      {
        if (string.IsNullOrEmpty(chunk))
        {
          continue;
        }

        var newObj = null as object;
        var dictionary = obj as IDictionary;
        if (dictionary != null)
        {
          if (dictionary.Contains(chunk))
          {
            newObj = dictionary[chunk];
          }
        }
        else
        {
          var type = obj.GetType();
          var prop = type.GetProperties().FirstOrDefault(x => x.Name.Equals(chunk, StringComparison.OrdinalIgnoreCase));
          if (prop != null)
          {
            newObj = prop.GetValue(obj, null);
          }
        }

        if (newObj == null)
        {
          Console.WriteLine("Cannot find '" + chunk + "' chunk of '" + query + "' query in the object: ");
          Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

          return null;
        }

        obj = newObj;
      }

      return obj;
    }

    [CanBeNull]
    private static string GetQueryAndFilterArgs([NotNull] List<string> filteredArgs)
    {
      Assert.ArgumentNotNull(filteredArgs, nameof(filteredArgs));

      var query = string.Empty;
      for (var i = 0; i < filteredArgs.Count; i++)
      {
        if (filteredArgs[i] == "--data")
        {
          filteredArgs[i] = "--query";
          filteredArgs.Insert(i + 1, "data");
        }

        if (filteredArgs[i] != "--query")
        {
          continue;
        }

        filteredArgs.RemoveAt(i);

        if (filteredArgs.Count > i)
        {
          query = filteredArgs[i];
          filteredArgs.RemoveAt(i);
        }

        break;
      }

      return query;
    }

    private static bool GetWaitAndFilterArgs([NotNull] List<string> filteredArgs)
    {
      Assert.ArgumentNotNull(filteredArgs, nameof(filteredArgs));

      for (var i = 0; i < filteredArgs.Count; i++)
      {
        if (filteredArgs[i] != "--wait")
        {
          continue;
        }

        filteredArgs.RemoveAt(i);

        return true;
      }

      return false;
    }
  }
}
