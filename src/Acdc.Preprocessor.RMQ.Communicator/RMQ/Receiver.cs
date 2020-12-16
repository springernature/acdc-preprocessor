using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Acdc.Preprocessor.Settings;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Acdc.Preprocessor.RMQ.Communicator
{
  public class Receiver
  {
    private readonly IBus _bus;

    private ConnectionConfiguration GetConnectionConfiguration(RmqConfiguration rmqConfiguration)
    {
      return new ConnectionConfiguration
      {
        VirtualHost = rmqConfiguration.VirtualHost,
        UserName = rmqConfiguration.UserName,
        Password = rmqConfiguration.Password,
        PrefetchCount = rmqConfiguration.PrefetchCount,
        Timeout = rmqConfiguration.Timeout,
        PersistentMessages = rmqConfiguration.PersistentMessages,
        Hosts = rmqConfiguration.HostNames.Select(hostName => new HostConfiguration { Host = hostName }).ToList()
      };
    }

    public Receiver(RmqConfiguration rmqConfiguration)
    {
      _bus = RabbitHutch.CreateBus(GetConnectionConfiguration(rmqConfiguration), x => x.Register<IEasyNetQLogger>(_ => new EasyNetQ.Loggers.NullLogger()));
    }

    public void StartMonitoring(string queueName,string rmqExchangeName ,Action<string,byte> messageHandler)
    {
      if (messageHandler == null) throw new ArgumentNullException(nameof(messageHandler));

      IExchange exchange = _bus.Advanced.ExchangeDeclare(rmqExchangeName, ExchangeType.Topic);
      IQueue queue = _bus.Advanced.QueueDeclare(queueName, maxPriority: 10);
      _bus.Advanced.Bind(exchange, queue, $"#.{queueName}");


      _bus.Advanced.Consume(queue, (body, properties, info) => Task.Factory.StartNew(() =>
      {
        var message = Encoding.UTF8.GetString(body);
        messageHandler(message,properties.Priority);
      }));
    }

  
  }
}
