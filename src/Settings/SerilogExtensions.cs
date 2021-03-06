﻿using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using System;

namespace HostedService.Settings
{
    public static class SerilogExtensions
    {
        
        public static void AddSerilog(this IServiceCollection services, SerilogConfig serilogConfig)
        {
            
            var loggerConfig = new LoggerConfiguration()
                           .MinimumLevel.Information()
                           .Enrich.FromLogContext()
                           .WriteTo.Console();

            if (serilogConfig.Elasticsearch.Enabled)
            {
                if(!string.IsNullOrEmpty(serilogConfig.Elasticsearch.Path))
                {
                    loggerConfig.WriteTo.File(new ElasticsearchJsonFormatter(), serilogConfig.Elasticsearch.Path, rollingInterval: RollingInterval.Day);
                }

                if (!string.IsNullOrEmpty(serilogConfig.Elasticsearch.Url))
                {
                    loggerConfig.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(serilogConfig.Elasticsearch.Url))
                    {
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog",
                        IndexFormat = "serilog-{0:yyyy.MM}"
                    });
                }

            }
            if (serilogConfig.File.Enabled)
            {
                loggerConfig.WriteTo.File(serilogConfig.File.Path, rollingInterval: RollingInterval.Day);
            }

            Serilog.Log.Logger = loggerConfig.CreateLogger();
        }

    }
}
