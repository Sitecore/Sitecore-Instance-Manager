using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SIM.Sitecore9Installer.Validation
{
  public class ValidationFactory
  {
    private static Dictionary<string, IValidator> validators=new Dictionary<string, IValidator>();
    private static Dictionary<string, List<string>> validatorLists;
    static ValidationFactory()
    {
      JObject doc =
        JObject.Parse(File.ReadAllText(Path.Combine(Directory.GetCurrentDirectory(),
          "GlobalParamsConfig/Validators.json")));
      List<ValidatorDefinition> validatorDefinitions =
        doc["ValidatorDefinitions"].ToObject<List<ValidatorDefinition>>();
      foreach (var definition in validatorDefinitions)
      {
        IValidator validator = (IValidator)Activator.CreateInstance(Type.GetType(definition.Type));
        validators.Add(definition.Name, validator);
      }

      validatorLists = doc["ValidatorLists"].ToObject<Dictionary<string, List<string>>>();
    }

    public static IEnumerable<IValidator> GetValidators(IEnumerable<string> names)
    {
      List<IValidator> result = new List<IValidator>();
      foreach (string name in names)
      {
        if (name.StartsWith("list:"))
        {
          string listName = name.Split(':')[1];
          if (validatorLists.ContainsKey(listName))
          {
            foreach (string validatorName in validatorLists[listName])
            {
              if (validators.ContainsKey(validatorName))
              {
                result.Add(validators[validatorName]);
              }
            }
          }
        }
        else
        {
          if (validators.ContainsKey(name))
          {
            result.Add(validators[name]);
          }
        }
      }

      return result;
    }
  }
}
