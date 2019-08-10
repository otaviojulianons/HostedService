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
        private readonly Random _random;
        private readonly Histogram _workDuration;

        public WorkerHostedService(ILogger<WorkerHostedService> logger)
        {
            _logger = logger;
            _random = new Random();
            _workDuration = Metrics.CreateHistogram("worker_service", "Histogram of worker processing durations.");
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("ExecuteAsync-Begin");
            
            while (!stopToken.IsCancellationRequested)
            {
                using (_workDuration.NewTimer())
                {
                    Log.Information("I'm alive");
                    await Task.Delay(_random.Next(100,1500));
                }
            }
            _logger.LogInformation("ExecuteAsync-End");
        }
    }
}
