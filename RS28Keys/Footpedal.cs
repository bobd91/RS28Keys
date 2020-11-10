using Device.Net;
using Hid.Net;
using Hid.Net.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Usb.Net.Windows;

namespace RS28Keys {
    public class Footpedal : IDisposable
    {
        
        /* Vendor and Product ID of Olympus RS28 Footpedal */
        private const int VENDOR_ID = 0x07B4;
        private const int PRODUCT_ID = 0x0218;

        /* Index in USB Report data of pedal down bits */
        private const int STATUS_INDEX = 3;

        /* Bit masks for left, right  and middle buttons */
        private const byte LEFT_MASK = 1 << 0;
        private const byte RIGHT_MASK = 1 << 1;
        private const byte MIDDLE_MASK = 1 << 2;

        /* Taken from Device.Net examples */
        private const int PollMilliseconds = 3000;

        private readonly List<FilterDeviceDefinition> _DeviceDefinitions = new List<FilterDeviceDefinition>
        {
            new FilterDeviceDefinition{ DeviceType = DeviceType.Hid, VendorId= VENDOR_ID, ProductId = PRODUCT_ID }
        };

        public event EventHandler<FootpedalEventArgs> StatusChanged;

        private IHidDevice _rs28Device;

        private DeviceListener _deviceListener;

        private static readonly DebugLogger _debugLogger = new DebugLogger();
        private static readonly DebugTracer _debugTracer = new DebugTracer();

        public Footpedal()
        {
            WindowsUsbDeviceFactory.Register(_debugLogger, _debugTracer);
            WindowsHidDeviceFactory.Register(_debugLogger, _debugTracer);
            _deviceListener = new DeviceListener(_DeviceDefinitions, PollMilliseconds) { Logger = _debugLogger };
        }
        public void Start()
        {
            _rs28Device?.Close();
            _deviceListener.DeviceDisconnected += DevicePoller_DeviceDisconnected;
            _deviceListener.DeviceInitialized += DevicePoller_DeviceInitialized;
            _deviceListener.Start();
        }

        public void Stop()
        {
            _rs28Device?.Close();
            _deviceListener.Stop();
            _deviceListener.DeviceDisconnected -= DevicePoller_DeviceDisconnected;
            _deviceListener.DeviceInitialized -= DevicePoller_DeviceInitialized;
        }

        private async void DevicePoller_DeviceInitialized(object sender, DeviceEventArgs e)
        {
            _rs28Device = (IHidDevice)e.Device;
            try
            {
                while (true)
                {
                    FootpedalStatus status = await ReadStatusAsync();
                    StatusChanged?.Invoke(this, new FootpedalEventArgs(status));
                }
            }
            catch (Exception)
            {
                /* Probably due to device being uplugged */
            }
        }

        private void DevicePoller_DeviceDisconnected(object sender, DeviceEventArgs e)
        {
            _rs28Device = null;
        }



        public async Task<FootpedalStatus> ReadStatusAsync()
        {
            var report = await _rs28Device.ReadReportAsync();
            byte statusBits = report.Data[STATUS_INDEX];

            return new FootpedalStatus(
                0 != (statusBits & LEFT_MASK),
                0 != (statusBits & RIGHT_MASK),
                0 != (statusBits & MIDDLE_MASK)
                );
        }

        public void Dispose()
        {
            _deviceListener.DeviceDisconnected -= DevicePoller_DeviceDisconnected;
            _deviceListener.DeviceInitialized -= DevicePoller_DeviceInitialized;
            _deviceListener.Dispose();
            _rs28Device?.Dispose();
        }
    }
}
