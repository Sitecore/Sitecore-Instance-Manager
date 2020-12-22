using System.Collections.Generic;

namespace SIM.ContainerInstaller.DockerCompose
{
  public interface IRepository<T>
    where T : class
  {
    IDictionary<string, T> GetServices(); // Get all services
  }
}