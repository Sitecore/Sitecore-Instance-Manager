#region Usings

using System;
using System.Diagnostics;

#endregion

namespace SIM.Base
{
  public static class Assert
  {
    public static void ArgumentNotNull(object argument, string arumentName)
    {
      var message = "The {0} argument is null".FormatWith(arumentName);
      if (argument == null)
      {
        throw new InvalidOperationException(message);
      }

      Debug.Assert(argument != null, message);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="object"></param>
    /// <param name="message"></param>
    /// <param name="isError"></param>
    /// <exception cref="InvalidOperationException"></exception>
    /// <exception cref="UserInformationException"></exception>
    public static void IsNotNull(object @object, string message, bool isError = true)
    {
      if(@object == null)
      {
        throw isError ? new InvalidOperationException(message) : new UserInformationException(message);
      }

      Debug.Assert(@object != null, message);
    }
    
    public static void IsTrue(bool condition, string message, bool isError = true)
    {
      if (!condition)
      {
        throw isError ? new InvalidOperationException(message) : new UserInformationException(message);
      }

      Debug.Assert(condition, message);
    }
    
    public static void ArgumentNotNullOrEmpty(string argument, string arumentName)
    {
      ArgumentNotNull(argument, arumentName);
      if(argument == string.Empty) throw new InvalidOperationException("The {0} argument is an empty string".FormatWith(arumentName));
    }
    
    public static void IsNotNullOrEmpty(string condition, string message, bool isError = true)
    {
      IsTrue(!string.IsNullOrEmpty(condition), message, isError);
    }
  }

  public class UserInformationException : InvalidOperationException
  {
    public UserInformationException(string message) : base(message)
    {      
    }
  }
}