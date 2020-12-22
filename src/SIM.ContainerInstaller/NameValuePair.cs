using System;

namespace SIM.ContainerInstaller
{
  public class NameValuePair
  {
    public NameValuePair(string name, string value)
    {
      if (string.IsNullOrWhiteSpace(name))
      {
        throw new ArgumentException("Name connot be null or whitespace");
      }

      this.Name = name;
      this.Value = value;
    }

    public string Name { get; }
    public string Value { get; set; }
  }
}
