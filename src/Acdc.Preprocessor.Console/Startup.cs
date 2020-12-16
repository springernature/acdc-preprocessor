
using Acdc.Preprocessor.Logging;
using Acdc.Preprocessor.RMQ.Communicator;
using Acdc.Preprocessor.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Acdc.Preprocessor.Console
{
    public class Startup
    {
        private readonly IConfigurationRoot _configurationRoot;

        public Startup()
        {
            _configurationRoot = new ConfigurationBuilder()
              .AddEnvironmentVariables()
              .Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            LoggerCF.SetElkConfiguration(_configurationRoot);
            services.AddOptions();
            services.Configure<AppSettings>(_configurationRoot);
            services.AddSingleton(_configurationRoot);
            services.AddTransient<Connector>();
        }
    }
}
