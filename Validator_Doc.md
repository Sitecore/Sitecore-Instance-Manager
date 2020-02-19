## Simple validator:

Simple validator allows checks in a context of a single task. For example check that dns name is valid.
1. Create a new class derived from the `SIM.Sitecore9Installer.Validation.Validators.BaseValidator`
2. Implement `IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)` method. Implementation details are explained later.
3. Add validator definition to the `GlobalParamsConfig\Validators.json` file:
```
    "ValidatorDefinitions": [   
    {
      "Name": "HostNameValidator",
      "Type": "SIM.Sitecore9Installer.Validation.Validators.HostNameValidator",
      "Data": { "ParamNames": "DnsName" }
    }
  ]
  ```
  4. In the `ParamNames` provide comma separated list of parameter names that should be validated.
  5. [optional] Add validator to a list in the `GlobalParamsConfig\Validators.json` file:
  ```
  "ValidatorLists": {    
    "Basic": [
      "HostNameValidator",
      "SolrServiceValidator"
    ] 
  }
  ```
  6. Reference validator or validator list in the Global params file. For example for sitecore 9.2.0 XP0 - `GlobalParamsConfig\XP\GlobalParams_9.2_XP0.json`:
  ```
  "Validators": [
    "HostNameValidator"
  ]
  ```
  for a valitador list:
  ```
   "Validators": [
    "list|Basic"
  ]
  ```
  
  
  
  **GetErrorsForTask method.**
  
  The method is invoked for each task that will be executed for the installtion. It accespts a `Task` and list of `InstallParams` objects. 
  InstallParams list contains `InstallParam` obejcts of the current task which names were provided in the `ParamNames`. 
  Here you can do the necessary check and return validation result. You can do multiple checks and return a validation result collection:
  ```
  protected override IEnumerable<ValidationResult> GetErrorsForTask(Task task, IEnumerable<InstallParam> paramsToValidate)
    {
      foreach (InstallParam param in paramsToValidate)
      {
        if (Uri.CheckHostName(param.Value) != UriHostNameType.Dns)
        {
          ValidationResult r = new ValidationResult(ValidatorState.Error,
            string.Format("Invalid host in '{0}' of '{1}'", param.Name, task.Name), null);
          yield return r;
        }
      }
    }
```
    
## Complex validator

This approach allows to perfrom cross-task checks. For example onr can check that host of the reporting instance corresponds 
to the reporting services of other instances.
1. Create a class that implements `SIM.Sitecore9Installer.Validation.IValidator`.
2. Implement `IEnumerable<ValidationResult> Evaluate(IEnumerable<Task> tasks)` and `Dictionary<string,string> Data { get; set; }`. 
Data must not return null.
3. Perfrom strps 3-6 from the simple validator part.

**Evaluate method**

The method accepts a collection of `Task` that will be executed for an installtions and returns a collection of `ValidationResult`
    
  
