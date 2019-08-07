using Microsoft.Extensions.Logging;
using Prometheus;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace HostedService
{
    public class WorkerHostedService : HostedService
    {
        private readonly ILogger<WorkerHostedService> _logger;
        private readonly Histogram _workDuration;

        public WorkerHostedService(ILogger<WorkerHostedService> logger)
        {
            _logger = logger;
            _workDuration = Metrics.CreateHistogram("worker_service_duration_seconds", "Histogram of worker processing durations.");
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("ExecuteAsync-Begin");
            while (!stopToken.IsCancellationRequested)
            {
                using (_workDuration.NewTimer())
                {
                    Log.Information("I'm alive");
                    await Task.Delay(5000);
                }
            }
            _logger.LogInformation("ExecuteAsync-End");
        }
    }
}
