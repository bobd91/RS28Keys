using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RS28Keys.ExtensionMethods;

namespace RS28Keys
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        private FootpedalStatus _status = new FootpedalStatus();
        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);

            using (Footpedal footpedal = new Footpedal())
            {
                footpedal.StatusChanged += StatusChangeMonitor;

                footpedal.Start();

                while (!stoppingToken.IsCancellationRequested)
                {
                    await stoppingToken.WaitHandle.WaitOneAsync();
                }
            }

        }

        private void StatusChangeMonitor(object source, FootpedalEventArgs statuschange)
        {
            FootpedalStatus newStatus = statuschange.Status;

            if(newStatus.IsLeftDown != _status.IsLeftDown)
            {


            }
        }
    }
}
