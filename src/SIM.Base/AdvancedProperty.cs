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
      this.DefaultValue = defaultValue;
      this.RawValueParser = rawValueParser;
      this._UserValue = this.ReadSetting();
    }

    #endregion

    #region Public Properties

    public T DefaultValue { get; set; }

    public override bool IsRawUserValueValid
    {
      get
      {
        if (this.RawUserValue.IsNullOrEmpty())
        {
          return true;
        }

        T parsedValue;
        return this.RawValueParser(this.RawUserValue, out parsedValue);
      }
    }

    public override string RawDefaultValue
    {
      get
      {
        return this.DefaultValue.ToString();
      }
    }


    public override string RawUserValue
    {
      get
      {
        return this._UserValue;
      }

      set
      {
        this.WriteSetting(value);
        this._UserValue = value.IsNullOrEmpty() ? null : value;
      }
    }

    public T Value
    {
      get
      {
        T parsedValue;
        if (!this.RawUserValue.IsNullOrEmpty() && this.RawValueParser(this.RawUserValue, out parsedValue))
        {
          return parsedValue;
        }

        return this.DefaultValue;
      }

      set
      {
        this.RawUserValue = value.ToString();
      }
    }

    #endregion

    #region Properties

    protected RawPropertyValueParser<T> RawValueParser { get; set; }

    #endregion
  }
}