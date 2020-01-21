using HostedService.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Prometheus;
using Serilog;
using System;
using System.Linq;
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
        private IOptionsMonitor<WorkerConfig> _workerConfig;

        public WorkerHostedService(
            ILogger<WorkerHostedService> logger,
            IOptionsMonitor<WorkerConfig> workerConfig,
            IConfiguration configuration)
        {
            _logger = logger;
            _workerConfig = workerConfig;
            _configuration = configuration;
            _random = new Random();
            _workDuration = Metrics.CreateHistogram(
                "worker_service", 
                "Histogram of worker processing durations.",
                new HistogramConfiguration()
                {
                    Buckets = Histogram.LinearBuckets(0, 0.1, 50)
                                .Concat(Histogram.LinearBuckets(5.0, 2.5, 10)).ToArray()
                });
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            _logger.LogInformation("ExecuteAsync-Begin");

            while (!stopToken.IsCancellationRequested)
            {
                var minDelay = _workerConfig.CurrentValue.MinServiceDelay;
                var maxDelay = _workerConfig.CurrentValue.MaxServiceDelay;
                var stressTest = _workerConfig.CurrentValue.StressTest;
                var stressTestDelay = _workerConfig.CurrentValue.StressTestDelay;
                var stressTestCores = _workerConfig.CurrentValue.StressTestCores;

                using (_workDuration.NewTimer())
                {
                    if (stressTest)
                    {
                        var stressTestTaskDelay = Task.Delay(stressTestDelay);
                        Parallel.For(0, stressTestCores, (value, state) =>
                        {
                            while (!stressTestTaskDelay.IsCompleted && !stopToken.IsCancellationRequested);
                        });
                    }

                    Log.Information($"MinServiceDelay:{minDelay} MaxServiceDelay:{maxDelay} StressTest:{stressTest} StressTestDelay: {stressTestDelay}");
                    await Task.Delay(_random.Next(minDelay, maxDelay));
                }
            }
            _logger.LogInformation("ExecuteAsync-End");
        }

    }
}
