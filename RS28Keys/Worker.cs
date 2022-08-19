using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;
using System.Diagnostics;
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

            if (newStatus.IsLeftDown != _status.IsLeftDown)
            {
                SendKey(VirtualKeyboard.KeyCode.VK_LSHIFT, newStatus.IsLeftDown);
            }
            if (newStatus.IsRightDown != _status.IsRightDown)
            {
                SendKey(VirtualKeyboard.KeyCode.VK_LCONTROL, newStatus.IsRightDown);
            }
            if (newStatus.IsMiddleDown != _status.IsMiddleDown)
            {
                SendKey(VirtualKeyboard.KeyCode.VK_LMENU, newStatus.IsMiddleDown);
            }

            _status = newStatus;
        }

        private void SendKey(VirtualKeyboard.KeyCode keyCode, bool isDown)
        {
            if(isDown)
            {
                VirtualKeyboard.SendKeyDown(keyCode);
            } else
            {
                VirtualKeyboard.SendKeyUp(keyCode);
            }
        }
    }
}
