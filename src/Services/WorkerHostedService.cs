using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Prometheus;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HostedService
{
    public class WorkerHostedService : HostedService
    {
        private readonly ILogger<WorkerHostedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly Random _random;
        private readonly Histogram _workDuration;

        public WorkerHostedService(ILogger<WorkerHostedService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _random = new Random();
            _workDuration = Metrics.CreateHistogram("worker_service", "Histogram of worker processing durations.");
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("ExecuteAsync-Begin");
            var minDelay = _configuration.GetValue<int>("MinServiceDelay");
            var maxDelay = _configuration.GetValue<int>("MaxServiceDelay");

            while (!stopToken.IsCancellationRequested)
            {
                using (_workDuration.NewTimer())
                {
                    Log.Information("I'm alive");
                    await Task.Delay(_random.Next(minDelay, maxDelay));
                }
            }
            _logger.LogInformation("ExecuteAsync-End");
        }
    }
}
