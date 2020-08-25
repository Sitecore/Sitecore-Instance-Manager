using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace SIM.Sitecore9Installer
{
  public abstract class BaseParameters:IEnumerable<InstallParam>
  {
    private object _lock = new object();
    private bool _evaluated;
    protected abstract List<InstallParam> Parameters { get; }
    public void Evaluate()
    {
      if (!this._evaluated)
      {
        lock (this._lock)
        {
          if (!this._evaluated)
          {
            this.CalculateParameters();
            this._evaluated = true;
          }
        }
      }
    }

    protected abstract void CalculateParameters();

    protected abstract InstallParam CreateParameter(string name, string value, InstallParamType type);
    
    public virtual void AddOrUpdateParam(string name, string value, InstallParamType type)
    {
      InstallParam param = this[name];
      if (param == null)
      {
        param = CreateParameter(name, value, type);
        this.Parameters.Insert(0, param);
      }

      if (param.Value != value) param.Value = value;
    }

    public InstallParam this[string name]
    {
      get
      {
        return this.Parameters.FirstOrDefault(p => p.Name == name);
      }
    }

    public InstallParam this[int index]
    {
      get
      {
        return this.Parameters[index];
      }
    }

    public int Count
    {
      get
      {
        return this.Parameters.Count;
      }
    }

    protected virtual string GetParamsScript(bool addPrefix = true)
    {
      StringBuilder script = new StringBuilder();
      foreach (InstallParam param in this.Parameters)
      {
        if (param.Value != null)
        {
          string value = param.GetParameterValue();
          string paramName = addPrefix ? "{" + param.Name + "}" : string.Format("'{0}'", param.Name);
          script.AppendLine(string.Format("{0}{1}={2}", addPrefix ? "$" : string.Empty, paramName, value));
        }
      }

      return script.ToString();
    }

    protected Hashtable GetEvaluatedParams(string script)
    {
      using (PowerShell PowerShellInstance = PowerShell.Create())
      {
        PowerShellInstance.AddScript(script);
        var res = PowerShellInstance.Invoke();
        if (res != null && res.Count > 0)
        {
          return (Hashtable)res.First().ImmediateBaseObject;
        }
      }

      return null;
    }

    public IEnumerator<InstallParam> GetEnumerator()
    {
      return ((IEnumerable<InstallParam>)Parameters).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return ((IEnumerable<InstallParam>)Parameters).GetEnumerator();
    }
  }
}
