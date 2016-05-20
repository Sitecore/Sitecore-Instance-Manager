namespace SIM.Client
{
  using System;
  using System.Collections;
  using System.Collections.Generic;
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
    [NotNull]
    private static readonly Assembly BaseAssembly = typeof(Program).Assembly;

    [NotNull]
    private static readonly string BaseNamespace = typeof(Program).Namespace;

    static Program()
    {
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => ResolveAssembly(args);
    }

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
      if (!parser.ParseArguments(filteredArgs.ToArray(), options, delegate { }))
      {
        Console.WriteLine("Note, commands provide output when work is done i.e. without any progress indication.");
        Console.WriteLine("\r\n  --query\t      When specified, allows returning only part of any command's output");
        Console.WriteLine("\r\n  --wait\t       When specified, waits for keyboard input before terminating");
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

    [CanBeNull]
    private static Assembly ResolveAssembly([NotNull] ResolveEventArgs args)
    {
      Assert.ArgumentNotNull(args, "args");

      var name = args.Name;
      if (name == null)
      {
        return null;
      }

      var comma = name.IndexOf(',');
      Assert.IsTrue(comma != 0, "Assembly name cannot start with comma: {0}", name);

      var fullyQualified = comma >= 0;
      var fileNameWithoutExtension = fullyQualified ? name.Substring(0, comma) : name;
      var resourceFullName = BaseNamespace + ".Properties.EmbeddedResources." + fileNameWithoutExtension + ".dll";
      var stream = BaseAssembly.GetManifestResourceStream(resourceFullName);
      if (stream == null)
      {
        return null;
      }

      var bytes = new byte[stream.Length];
      if (stream.Read(bytes, 0, bytes.Length) == 0)
      {
        return null;
      }

      return Assembly.Load(bytes);
    }
  }
}
