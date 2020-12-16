using System.Collections.Generic;

namespace Acdc.Preprocessor.RMQ.Communicator
{
  public class RmqConfiguration
  {
    public string AppId { get; set; }
    public string ExchangeName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string VirtualHost { get; set; }
    public ushort PrefetchCount { get; set; }
    public ushort Timeout { get; set; }
    public bool PersistentMessages { get; set; }

    public IEnumerable<string> HostNames { get; private set; }

    public RmqConfiguration()
    {
      Timeout = 10;
    }

    public void SetHostNames(string hostName)
    {
      const char PIPE = '|';
      HostNames = hostName.Split(PIPE);
    }
  }
}
