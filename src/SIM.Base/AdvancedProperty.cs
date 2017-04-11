namespace SIM
{
  using SIM.Extensions;

  public delegate bool RawPropertyValueParser<TValue>(string valueToParse, out TValue result);

  public class AdvancedProperty<T> : AdvancedPropertyBase
  {
    #region Fields

    protected string _UserValue;

    #endregion

    #region Constructors

    public AdvancedProperty(string xpathKey, T defaultValue, IAdvancedSettingsStorage settingsStorage, RawPropertyValueParser<T> rawValueParser)
      : base(xpathKey, settingsStorage)
    {
      DefaultValue = defaultValue;
      RawValueParser = rawValueParser;
      _UserValue = ReadSetting();
    }

    #endregion

    #region Public Properties

    public T DefaultValue { get; set; }

    public override bool IsRawUserValueValid
    {
      get
      {
        if (RawUserValue.IsNullOrEmpty())
        {
          return true;
        }

        T parsedValue;
        return RawValueParser(RawUserValue, out parsedValue);
      }
    }

    public override string RawDefaultValue
    {
      get
      {
        return DefaultValue.ToString();
      }
    }


    public override string RawUserValue
    {
      get
      {
        return _UserValue;
      }

      set
      {
        WriteSetting(value);
        _UserValue = value.IsNullOrEmpty() ? null : value;
      }
    }

    public T Value
    {
      get
      {
        T parsedValue;
        if (!RawUserValue.IsNullOrEmpty() && RawValueParser(RawUserValue, out parsedValue))
        {
          return parsedValue;
        }

        return DefaultValue;
      }

      set
      {
        RawUserValue = value.ToString();
      }
    }

    #endregion

    #region Properties

    protected RawPropertyValueParser<T> RawValueParser { get; set; }

    #endregion
  }
}