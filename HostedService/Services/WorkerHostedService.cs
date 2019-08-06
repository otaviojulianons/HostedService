using Microsoft.Extensions.Logging;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace HostedService
{
    public class WorkerHostedService : BackgroundService
    {
        private ILogger<WorkerHostedService> _logger;

        public WorkerHostedService(ILogger<WorkerHostedService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stopToken)
        {
            Log.Information("Execute");
            //Do your preparation (e.g. Start code) here
            while (!stopToken.IsCancellationRequested)
            {
                _logger.LogInformation("I'm alive");
                await Task.Delay(5000);
            }
            Log.Information("Stop");
            //Do your cleanup (e.g. Stop code) here
        }
    }
}
