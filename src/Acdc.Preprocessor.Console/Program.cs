using Microsoft.Extensions.DependencyInjection;
using System;

namespace Acdc.Preprocessor.Console
{
    class Program
    {
        protected Program()
        {
        }

        static void Main(string[] args)
        {
            ConfigureConsole();

            var serviceCollection = new ServiceCollection();
            var startup = new Startup();
            startup.ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            serviceProvider.GetService<RMQ.Communicator.Connector>().Connect();
        }

        private static void ConfigureConsole()
        {
            System.Console.Clear();
            System.Console.Title = "acdc-preprocessor-service- [" + typeof(Program).Assembly.GetName().Version + "]";
            System.Console.WriteLine("preprocessor service started.");
        }
    }
}
