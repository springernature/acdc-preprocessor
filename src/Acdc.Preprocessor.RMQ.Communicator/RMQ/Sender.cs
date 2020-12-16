using System;
using System.Diagnostics;
using System.Linq;
using EasyNetQ;
using EasyNetQ.Topology;
using Newtonsoft.Json.Linq;

namespace Acdc.Preprocessor.RMQ.Communicator
{
  public class Sender
  {
    private readonly IExchange _exchange;
    private readonly IBus _bus;
    private readonly MessageProperties _properties;

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

    public Sender(RmqConfiguration rmqConfiguration)
    {
      _bus = RabbitHutch.CreateBus(GetConnectionConfiguration(rmqConfiguration), x => x.Register<IEasyNetQLogger>(_ => new EasyNetQ.Loggers.NullLogger()));

      _exchange = new Exchange(rmqConfiguration.ExchangeName);

      string appId = rmqConfiguration.AppId;
      _properties = new MessageProperties { AppId = appId, CorrelationId = appId + "-" + Process.GetCurrentProcess().Id };
    }

    public void Send<T>(string routingKey, T message,byte queuePriority ) where T : class
    {
      _properties.Priority = queuePriority;
      var m = new Message<T>(message, _properties);
      _bus.Advanced.Publish(_exchange, routingKey, true, m);
    }

        public void Send<T>(string routingKey, T message) where T : class
        {
            var m = new Message<T>(message);
            _bus.Advanced.Publish(_exchange, routingKey, true, m);
        }

    }
}
