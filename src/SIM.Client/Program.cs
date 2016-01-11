namespace SIM.Client
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.Linq;
  using CommandLine;
  using Newtonsoft.Json;
  using SIM.Client.Commands;
  using SIM.Core;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;
  using Sitecore.Diagnostics.Logging;

  public static class Program
  {
    public static void Main([NotNull] string[] args)
    {
      Assert.ArgumentNotNull(args, "args");

      CoreApp.InitializeLogging();

      CoreApp.LogMainInfo();

      var filteredArgs = args.ToList();
      var query = GetQueryAndFilterArgs(filteredArgs);
      var wait = GetWaitAndFilterArgs(filteredArgs);

      var parser = new Parser(with => with.NotNull().HelpWriter = Console.Error);
      Assert.IsNotNull(parser, "parser");

      var options = new MainCommandGroup();
      if (!parser.ParseArguments(filteredArgs.ToArray(), options, delegate { }))
      {
        Console.WriteLine("\r\n  --query\t      When specified, allows returning only part of any command's output");
        Console.WriteLine("\r\n  --wait\t       When specified, waits for keyboard input before terminating");
        Environment.Exit(Parser.DefaultExitCodeFail);
      }

      var result = options.SelectedCommand.Execute() as object;

      result = QueryResult(result, query);
      if (result == null)
      {
        return;
      }

      Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

      if (wait)
      {
        Console.ReadKey();
      }
    }

    [CanBeNull]
    private static object QueryResult([NotNull] object result, [CanBeNull] string query)
    {
      Assert.ArgumentNotNull(result, "result");

      if (string.IsNullOrEmpty(query))
      {
        return result;
      }

      var obj = result;
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
      Assert.ArgumentNotNull(filteredArgs, "filteredArgs");

      var query = string.Empty;
      for (int i = 0; i < filteredArgs.Count; i++)
      {
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
      Assert.ArgumentNotNull(filteredArgs, "filteredArgs");

      for (int i = 0; i < filteredArgs.Count; i++)
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
