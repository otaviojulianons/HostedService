using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;

namespace HostedService.Utils
{
    public static class SerilogExtensions
    {
        
        public static void AddSerilog(this IServiceCollection services, ConfigurationElasticSerilog elasticConfig)
        {
            var loggerConfig = new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .Enrich.FromLogContext();

            if (elasticConfig.Enabled)
            {
                loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticConfig.Url))
                {
                    AutoRegisterTemplate = true,
                    TemplateName = "serilog",
                    IndexFormat = "serilog-{0:yyyy.MM}"
                });
            }
            Serilog.Log.Logger = loggerConfig.CreateLogger();
        }

    }
}
