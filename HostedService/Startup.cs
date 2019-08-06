﻿using HostedService.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Mime;

namespace HostedService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var elasticConfig = new ConfigurationElasticSerilog();
            Configuration.GetSection("Serilog:Elasticsearch").Bind(elasticConfig);

            var healthChecksBuilder = services.AddHealthChecks();
            if(elasticConfig.Enabled)
                healthChecksBuilder.AddElasticsearch(elasticConfig.Url);

            services.AddSerilog(elasticConfig);

            services.AddHostedService<WorkerHostedService>();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHealthChecks("/health", new HealthCheckOptions()
            {
                ResponseWriter = async (context, report) =>
                {
                    var result = JsonConvert.SerializeObject(
                        new
                        {
                            statusApplication = report.Status.ToString(),
                            healthChecks = report.Entries.Select(e => new
                            {
                                check = e.Key,
                                ErrorMessage = e.Value.Exception?.Message,
                                status = Enum.GetName(typeof(HealthStatus), e.Value.Status)
                            })
                        });
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
