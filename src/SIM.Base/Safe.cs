namespace SIM
{
  using System;

  public static class Safe
  {                                                                     
    public static T Call<T>(Func<T> func) where T: class 
    {
      try
      {
        return func();
      }
      catch
      {
        return null;
      }
    }
  }
}
