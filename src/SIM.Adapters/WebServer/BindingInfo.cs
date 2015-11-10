namespace SIM.Adapters.WebServer
{
  using System.Net;
  using Microsoft.Web.Administration;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Annotations;

  public sealed class BindingInfo
  {
    #region Fields

    public readonly string Host;
    public readonly string IP;
    public readonly int Port;
    public readonly string Protocol;

    #endregion

    #region Constructors

    public BindingInfo([NotNull] string protocol, [NotNull] string host, int port, [NotNull] string ip)
    {
      Assert.ArgumentNotNull(protocol, "protocol");
      Assert.ArgumentNotNull(host, "host");
      Assert.ArgumentNotNull(ip, "ip");

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
      Assert.ArgumentNotNull(binding, "binding");
    }

    private BindingInfo([NotNull] string protocol, [NotNull] string host, [NotNull] IPEndPoint endPoint) : this(protocol, host, endPoint.Port, endPoint.Address.ToString())
    {
      Assert.ArgumentNotNull(protocol, "protocol");
      Assert.ArgumentNotNull(host, "host");
      Assert.ArgumentNotNull(endPoint, "endPoint");
    }

    #endregion
  }
}