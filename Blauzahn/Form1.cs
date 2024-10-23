using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;


namespace Blauzahn
{
    public partial class Form1 : Form
    {
        private DeviceWatcher deviceWatcher;
        private readonly ObservableCollection<BluetoothDevice> devices = new ObservableCollection<BluetoothDevice>();

        public Form1()
        {
            InitializeComponent();
            deviceWatcher = CreateBLEWatcher();
            deviceWatcher.Start();
        }

        private DeviceWatcher CreateBLEWatcher()
        {
            // Query for extra properties you want returned
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            DeviceWatcher watcher =
                        DeviceInformation.CreateWatcher(
                                BluetoothLEDevice.GetDeviceSelectorFromPairingState(false),
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            // Added, Updated and Removed are required to get all nearby devices
            watcher.Added += DeviceWatcher_Added;
            watcher.Updated += DeviceWatcher_Updated;
            watcher.Removed += DeviceWatcher_Removed;

            // EnumerationCompleted and Stopped are optional to implement.
            //deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            //deviceWatcher.Stopped += DeviceWatcher_Stopped;

            return watcher;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (args.Name == "") { return; }
            StringBuilder consoleOutput = new StringBuilder();
            consoleOutput.Append($"Name: {args.Name}, ID: {args.Id}\n");
            BluetoothDevicesListBox.Items.Add(args.Name);
            if (args != null && args.Name.StartsWith("LPMSB2-"))
            {
                consoleOutput.Append(2 + "\n");

                BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(args.Id);
                GattDeviceServicesResult result = await device.GetGattServicesAsync();
                consoleOutput.Append(3 + "\n");
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;
                    foreach (var service in services)
                    {
                        consoleOutput.Append("UUID: " + service.Uuid + "\n");
                    }
                }
            }
            Console.WriteLine(consoleOutput.ToString());
        }

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TODO: update existing one
            //throw new NotImplementedException();
        }

        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            // TODO: dispose and remove from list
            //bluetoothLeDevice.Dispose();
            //throw new NotImplementedException();
        }

        //optional
        //private void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object args)
        //{
        //    throw new NotImplementedException();
        //}

        //optional
        //private void DeviceWatcher_Stopped(DeviceWatcher sender, object args)
        //{
        //    throw new NotImplementedException();
        //}

        async void ConnectDevice(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            // ...
        }
    }
}
