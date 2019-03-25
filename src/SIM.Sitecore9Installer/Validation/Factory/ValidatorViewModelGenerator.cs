using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SitecoreInstaller.Validation.Abstractions;

namespace SIM.Sitecore9Installer.Validation.Factory
{
  /// <summary>
  /// temporary solution, until validators are reworked to be async
  /// </summary>
  public class ValidatorViewModelGenerator
  {
    private ValidatorViewModelGenerator()
    { }

    private static ValidatorViewModelGenerator _instance;

    public static ValidatorViewModelGenerator Instance
    {
      get { return _instance ?? new ValidatorViewModelGenerator(); }
    }

    public List<ValidatorViewModel> GetViewModels(IEnumerable<IInstallationValidator> validators, Dictionary<string, string> installParams)
    {
      List<ValidatorViewModel> result = new List<ValidatorViewModel>();
      foreach (var validatror in validators)
      {
        result.Add(new ValidatorViewModel(validatror, installParams));
      }

      return result;
    }

  }
}
