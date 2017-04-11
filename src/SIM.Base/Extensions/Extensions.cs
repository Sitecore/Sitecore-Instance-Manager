namespace SIM.Extensions
{
  #region

  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Logging;

  #endregion

  public static class Extensions
  {
    #region Public Methods

    public static string ToShortGuid(this Guid guid)
    {
      return guid.ToString("N");
    }

    public static TOut PipeTo<TIn, TOut>(this TIn obj, Func<TIn, TOut> func)
    {
      return func(obj);
    }

    public static string GetNonEmptyAttribute(this XmlElement element, string name)
    {
      return element.GetAttribute(name).EmptyToNull().IsNotNull("{0} doesn't have the {1} attribute filled in".FormatWith(element.OuterXml, name));
    }

    [NotNull]
    public static IEnumerable<T1> NotNull<T1>([CanBeNull] this IEnumerable<T1> source) where T1 : class
    {
      return source.Where(item => item != null);
    }

    [CanBeNull]
    public static T SafeCall<T>(this object obj, Func<T> func) where T : class
    {
      try
      {
        return func();
      }
      catch (Exception ex)
      {
        Log.Warn(ex, $"SafeCall failed");
        return null;
      }
    }

    [NotNull]
    public static IEnumerable<XmlElement> SelectElements(this XmlDocument document, string xpath)
    {
      Assert.ArgumentNotNull(document, nameof(document));
      Assert.ArgumentNotNull(xpath, nameof(xpath));
      
      return document.SelectNodes(xpath)?.OfType<XmlElement>() ?? new XmlElement[0];
    }

    [NotNull]
    public static IEnumerable<XmlElement> SelectElements(this XmlElement element, string xpath)
    {    
        Assert.ArgumentNotNull(element, nameof(element));
        Assert.ArgumentNotNull(xpath, nameof(xpath));

        return element.SelectNodes(xpath)?.OfType<XmlElement>() ?? new XmlElement[0];
    }

    [CanBeNull]
    public static XmlElement SelectSingleElement(this XmlDocument document, string xpath)
    {
      return document.SelectSingleNode(xpath) as XmlElement;
    }

    [NotNull]
    public static XmlElement SelectSingleElementOrCreate([NotNull] this XmlDocument document, [NotNull] string xpath)
    {
      Assert.ArgumentNotNull(document, nameof(document));
      Assert.ArgumentNotNullOrEmpty(xpath, nameof(xpath));

      var element = document.SelectSingleNode(xpath) as XmlElement;
      if (element != null)
      {
        return element;
      }

      var pos = xpath.LastIndexOf('/');
      Assert.IsTrue(pos >= 0, $"wrong xpath ({xpath})");

      var parentXPath = xpath.Substring(0, pos);
      var name = xpath.Substring(pos + 1);
      Assert.IsNotNullOrEmpty(name, $"wrong name ({xpath})");

      // recursion
      var parent = SelectSingleElementOrCreate(document, parentXPath);

      // create element
      element = parent.AddElement(name);
      Assert.IsNotNull(element, nameof(element));

      return element;
    }

    [CanBeNull]
    public static XmlElement SelectSingleElement(this XmlElement element, string xpath)
    {
      return element.SelectSingleNode(xpath) as XmlElement;
    }

    #endregion

    // Monad maybe
    #region Public methods

    public static IEnumerable<T> Add<T>(this IEnumerable<T> @this, T item)
    {
      foreach (var obj in @this)
      {
        yield return obj;
      }

      yield return item;
    }

    public static XmlElement AddElement(this XmlNode node, string name)
    {
      var element = node.OwnerDocument.CreateElement(name);
      node.AppendChild(element);
      return element;
    }                       

    public static string Join<T>(this IEnumerable<T> @this, string separator, string wrapperLeft, string wrapperRight)
    {
      return @this.Join(separator, wrapperLeft, wrapperRight, string.Empty, string.Empty);
    }

    public static string Join<T>(this IEnumerable<T> @this, string separator, string wrapperLeft, string wrapperRight, string before, string after)
    {
      return before + @this.Aggregate(string.Empty, (sentense, item) => sentense + separator + wrapperLeft + item + wrapperRight).TrimStart(separator) + after;
    }

    [CanBeNull]
    public static TResult With<TInput, TResult>(this TInput @this, Func<TInput, TResult> evaluator)
      where TResult : class
      where TInput : class
    {
      if (@this == null)
      {
        return null;
      }

      return evaluator(@this);
    }

    #endregion
  }
}