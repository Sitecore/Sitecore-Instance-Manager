using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SitecoreInstaller.Validation.Abstractions;
using SitecoreInstaller.Validation.SQL;

namespace SitecoreInstaller.Validation.Factory
{
  public class ValidatorFactory
  {
    private ValidatorFactory()
    {
      this.Initialize();
    }

    public IEnumerable<Type> validatorTypes;

    static ValidatorFactory instance = new ValidatorFactory();

    public static ValidatorFactory Instance
    {
      get { return instance; }
    }

    private void Initialize()
    {
      Assembly current = typeof(ValidatorFactory).Assembly;
      this.validatorTypes =
        current.GetTypes().Where(x => typeof(IInstallationValidator).IsAssignableFrom(x));

    }


    public List<IInstallationValidator> GetValidators(Dictionary<string,string> installParams)
    {
      List<IInstallationValidator> result = new List<IInstallationValidator>(); ///TODO: check if validator already created
      foreach (var validatorType in validatorTypes)
      {
        if (installParams.Any(x => validatorType.Name.StartsWith(x.Key)))
        {
          result.Add((IInstallationValidator)Activator.CreateInstance(validatorType)); //TODO: fix later?
        }
      }
      if(validatorTypes.Contains(typeof(SqlDbChecker)))
        result.Add(((IInstallationValidator)Activator.CreateInstance(typeof(SqlDbChecker))));
      return result;
    }

  }
}
