namespace SIM.Adapters.WebServer
{
  using System.Net;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using JetBrains.Annotations;
  using SIM.Extensions;

  public sealed class BindingInfo
  {
    #region Fields

    public string Host { get; }
    public string IP { get; }
    public int Port { get; }
    public string Protocol { get; }

    #endregion

    #region Constructors

    public BindingInfo([NotNull] string protocol, [NotNull] string host, int port, [NotNull] string ip)
    {
      Assert.ArgumentNotNull(protocol, nameof(protocol));
      Assert.ArgumentNotNull(host, nameof(host));
      Assert.ArgumentNotNull(ip, nameof(ip));

      if (host == "*")
      {
        host = string.Empty;
      }

      if (ip == "0.0.0.0")
      {
        ip = "*";
      }

      this.Protocol = protocol;
      this.Host = host;
      this.Port = port;
      this.IP = ip;
    }

    public BindingInfo([NotNull] Binding binding)
      : this(
        binding.Protocol.IsNotNull("binding.Protocol"), 
        binding.Host.IsNotNull("binding.Host"), 
        binding.EndPoint.IsNotNull("binding.EndPoint"))
    {
      Assert.ArgumentNotNull(binding, nameof(binding));
    }

    private BindingInfo([NotNull] string protocol, [NotNull] string host, [NotNull] IPEndPoint endPoint) : this(protocol, host, endPoint.Port, endPoint.Address.ToString())
    {
      Assert.ArgumentNotNull(protocol, nameof(protocol));
      Assert.ArgumentNotNull(host, nameof(host));
      Assert.ArgumentNotNull(endPoint, nameof(endPoint));
    }

    #endregion
  }
}