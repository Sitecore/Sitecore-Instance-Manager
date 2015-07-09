using System.Collections.Generic;
using SIM.CustomDataStoring.SavePolicies;

namespace SIM.CustomDataStoring
{
  public interface IDataBox
  {
    #region Public Properties

    string Name { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Delete the inner Box. Pay attention that if you try to work with already deleted box the exception will be thrown
    /// </summary>
    /// <param name="name">Name of subBox to delete</param>
    /// <param name="throwExceptionIfNotExists">If there is no subBox with such name, the exception will be thrown</param>
    void DeleteSubBox(string name, bool throwExceptionIfNotExists = false);

    bool GetBool(string key, bool defaultValue);
    double GetDouble(string key, double defaultValue);
    int GetInt(string key, int defaultValue);
    long GetLong(string key, long defaultValue);

    /// <summary>
    ///   Get SubBox with specified name and save strategy. SPecified strategy will be used to handle box and SubBox changes
    ///   (if SubBox doesn't have its own strategy).
    /// </summary>
    /// <param name="name"></param>
    /// <param name="saveStrategy"></param>
    /// <returns></returns>
    IDataBox GetOrCreateSubBox(string name, ISaveStrategy saveStrategy);

    /// <summary>
    ///   Get SubBox with specified name. The default save strategy will be used.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IDataBox GetOrCreateSubBox(string name);

    IEnumerable<string> GetPresentSubBoxNames();
    string GetString(string key, string defaultValue);
    void SetBool(string key, bool value);
    void SetDouble(string key, double value);
    void SetInt(string key, int value);
    void SetLong(string key, long value);
    void SetString(string key, string value);
    bool SubBoxExists(string name);
    bool TryGetBool(string key, out bool value);
    bool TryGetDouble(string key, out double value);
    bool TryGetInt(string key, out int value);
    bool TryGetLong(string key, out long value);

    #endregion
  }
}