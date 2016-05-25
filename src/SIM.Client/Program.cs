namespace SIM.Client
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using CommandLine;
  using Newtonsoft.Json;
  using SIM.Client.Commands;
  using SIM.Client.Serialization;
  using SIM.Core;
  using SIM.Core.Common;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public static class Program
  {
    public static void Main([NotNull] string[] args)
    {
      Assert.ArgumentNotNull(args, "args");

      CoreApp.InitializeLogging();

      CoreApp.LogMainInfo();

      Analytics.Start();

      var filteredArgs = args.ToList();
      var query = GetQueryAndFilterArgs(filteredArgs);
      var wait = GetWaitAndFilterArgs(filteredArgs);

      var parser = new Parser(with => with.HelpWriter = Console.Error);
      Assert.IsNotNull(parser, "parser");

      var options = new MainCommandGroup();
      EnsureAutoCompeteForCommands(options);
      if (!parser.ParseArguments(filteredArgs.ToArray(), options, delegate { }))
      {
        Console.WriteLine("Note, commands provide output when work is done i.e. without any progress indication.");
        Console.WriteLine("\r\n  --query\t   When specified, allows returning only part of any command's output");
        Console.WriteLine("\r\n  --data\t   When specified, allows returning only 'data' part of any command's output");
        Console.WriteLine("\r\n  --wait\t   When specified, waits for keyboard input before terminating");

        Environment.Exit(Parser.DefaultExitCodeFail);
      }

      var result = options.SelectedCommand.Execute();

      result = QueryResult(result, query);
      if (result == null)
      {
        return;
      }

      var serializer = new JsonSerializer
      {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.Indented,        
      };

      serializer.Converters.Add(new DirectoryInfoConverter());

      var writer = Console.Out;
      serializer.Serialize(writer, result);

      if (wait)
      {
        Console.ReadKey();
      }
    }

    private static void EnsureAutoCompeteForCommands(MainCommandGroup options)
    {
      foreach (var propertyInfo in options.GetType().GetProperties())
      {
        if (typeof(ICommand).IsAssignableFrom(propertyInfo.PropertyType))
        {
          var verb = propertyInfo.GetCustomAttributes().OfType<VerbOptionAttribute>().FirstOrDefault();
          if (verb == null)
          {
            continue;
          }

          var command = verb.LongName;
          if (File.Exists(command))
          {
            continue;
          }

          File.Create(command).Close();
        }
      }
    }

    [CanBeNull]
    private static CommandResult QueryResult([NotNull] CommandResult result, [CanBeNull] string query)
    {
      Assert.ArgumentNotNull(result, "result");

      if (string.IsNullOrEmpty(query) || !result.Success)
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

        var newObj = null as CommandResult;
        var dictionary = obj as IDictionary;
        if (dictionary != null)
        {
          if (dictionary.Contains(chunk))
          {
            newObj = dictionary[chunk] as CommandResult;
          }
        }
        else
        {
          var type = obj.GetType();
          var prop = type.GetProperties().FirstOrDefault(x => x.Name.Equals(chunk, StringComparison.OrdinalIgnoreCase));
          if (prop != null)
          {
            newObj = prop.GetValue(obj, null) as CommandResult;
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
