using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;
using static System.Net.Mime.MediaTypeNames;
using Windows.UI.Xaml.Controls;


namespace Blauzahn
{
    public partial class Form1 : Form
    {
        private const string BATTERY_SERVICE_UUID_PREFIX = "0000180f";
        private DeviceWatcher deviceWatcher;
        private readonly ObservableCollection<BluetoothDevice> devices = new ObservableCollection<BluetoothDevice>();
        private Dictionary<string, DeviceInformation> deviceDictionary = new Dictionary<string, DeviceInformation>();

        public Form1()
        {
            InitializeComponent();
            BluetoothDevicesListBox.DisplayMember = "Name";
            deviceWatcher = CreateBLEWatcher();
            deviceWatcher.Start();
            populateOutputComboBox();
        }

        private void populateOutputComboBox()
        {
            saveFormatComboBox.Items.Add("CSV");
            saveFormatComboBox.Items.Add("HEX");
            saveFormatComboBox.SelectedIndex = 0;
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

        /// <summary>
        /// Gets Called, when a new Device is Detected
        /// </summary>
        /// <param name="sender">The Device-Watcher, which found the Device</param>
        /// <param name="args">More Device Information from the found Device</param>
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {

            if (args.Name == "") { return; }
            StringBuilder consoleOutput = new StringBuilder();
            consoleOutput.Append($"Name: {args.Name}, ID: {args.Id}\n");
            deviceDictionary[args.Name] = args;


            if (BluetoothDevicesListBox.InvokeRequired)
            {
                BluetoothDevicesListBox.Invoke(new Action(() =>
                {
                    // Füge das DeviceInformation-Objekt der ListBox hinzu
                    BluetoothDevicesListBox.Items.Add(args.Name);
                }));
            }
            else
            {
                BluetoothDevicesListBox.Items.Add(args.Name);
            }

            //TODO: Change Filtering to Advertisment watcher
            // https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/ble-beacon
            if (args != null && args.Name.StartsWith("LPMSB2-"))
            {
                bool isNamedSensor = false;
                foreach (var str in sensorNamesTextBox.Lines)
                {
                    if (args.Name.StartsWith(str))
                    {
                        isNamedSensor = true;
                    }
                }
                if (!isNamedSensor)
                {
                    return;
                }
                consoleOutput.Append(2 + "\n");

                //Retrieve Services from BLE Device
                BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(args.Id);
                GattDeviceServicesResult result = await device.GetGattServicesAsync();
                consoleOutput.Append(3 + "\n");
                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;
                    foreach (var service in services)
                    {
                        consoleOutput.Append("UUID: " + service.Uuid);
                        if (service.Uuid.ToString().StartsWith(BATTERY_SERVICE_UUID_PREFIX))
                        {
                            consoleOutput.Append(4 + "\n");
                            //Get BLE Device Characteristics
                            var characteristicsResult = await service.GetCharacteristicsAsync();
                            if (characteristicsResult.Status == GattCommunicationStatus.Success)
                            {
                                consoleOutput.Append(5 + "\n");
                                var characteristics = characteristicsResult.Characteristics;
                                foreach (var characteristic in characteristics)
                                {
                                    consoleOutput.Append("Characteristic: " + characteristic + "\n");
                                    GattCharacteristicProperties properties = characteristic.CharacteristicProperties;

                                    //if (properties.HasFlag(GattCharacteristicProperties.Read))
                                    //{
                                    //    // This characteristic supports reading from it.
                                    //    consoleOutput.Append("Has Read property \n");
                                    //}
                                    //if (properties.HasFlag(GattCharacteristicProperties.Write))
                                    //{
                                    //    // This characteristic supports writing to it.
                                    //    consoleOutput.Append("Has Write property \n");
                                    //}
                                    if (properties.HasFlag(GattCharacteristicProperties.Notify))
                                    {
                                        // This characteristic supports subscribing to notifications.
                                        consoleOutput.Append("Has Notify property \n");
                                        GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                        if (status == GattCommunicationStatus.Success)
                                        {
                                            consoleOutput.Append("Added listener \n");
                                            // Server has been informed of clients interest.
                                            characteristic.ValueChanged += Characteristic_ValueChanged;
                                        }
                                    } 
                                    else
                                    {
                                        // ignore this device (either broken or of unknown design)
                                    }
                                }
                            }
                        }
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

        /// <summary>
        /// Gets Called, when a previously found Device is not available anymore
        /// </summary>
        /// <param name="sender">The Devicewatcher</param>
        /// <param name="args">More Information about the removed Device</param>
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


        private int singleAmount = 0;
        private StringBuilder rawDataBuilder = new StringBuilder();
        private StringBuilder singleDataBuilder = new StringBuilder();
        //Called, when new Notifications / Pakets arrive
        void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                if (singleAmount >= 20)
                {
                    Console.WriteLine("Time: " + DateTime.Now.ToString());
                    Console.WriteLine("Raw data: "+rawDataBuilder.ToString());
                    rawDataBuilder.Clear();
                    Console.WriteLine("Single data:" + singleDataBuilder.ToString());
                    singleAmount = 0;
                }
                // An Indicate or Notify reported that the value has changed.
                var reader = DataReader.FromBuffer(args.CharacteristicValue);
                byte[] inputBytes = new byte[reader.UnconsumedBufferLength];
                if (inputBytes.Length != 20)
                {
                    return;
                }
                reader.ReadBytes(inputBytes);

                // Output the raw data as a hexadecimal string
                rawDataBuilder.Append(BitConverter.ToString(inputBytes));

                // Process each byte as an individual value
                // TODO: add a switch / interface to handle different data types (optional: based on user input)
                for (int j = 1; j <= inputBytes.Length; j += sizeof(Single))
                {
                    singleAmount++;
                    int i = j - 1;
                    if (i + sizeof(Single) <= inputBytes.Length)
                    {
                        singleDataBuilder.Append(" " + BitConverter.ToSingle(inputBytes, i));
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Error reading data: " + ex.Message);
                // Handle or log the exception as needed
            }
        }

        async void ConnectDevice(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
        }

        async void DisconnectDevice(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            BluetoothLEDevice bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            bluetoothLeDevice.Dispose();
        }

        private void BluetoothDevicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void connectButton_Click(object sender, EventArgs e)
        {
            string selectedDeviceName = BluetoothDevicesListBox.GetItemText(BluetoothDevicesListBox.SelectedItem);
            if (selectedDeviceName == null)
            {
                throw new ArgumentNullException("Der Gerätename ist NULL");
            }
            else if (!deviceDictionary.ContainsKey(selectedDeviceName))
            {
                throw new KeyNotFoundException("Der Gerätename ist nicht im DeviceDictionary enthalten");
            }
            else {
                DeviceInformation selectedDevice = deviceDictionary[selectedDeviceName];
                ConnectDevice(selectedDevice);
            }
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            string selectedDeviceName = BluetoothDevicesListBox.GetItemText(BluetoothDevicesListBox.SelectedItem);
            if (selectedDeviceName == null)
            {
                throw new ArgumentNullException("Der Gerätename ist NULL");
            }
            else if (!deviceDictionary.ContainsKey(selectedDeviceName))
            {
                throw new KeyNotFoundException("Der Gerätename ist nicht im DeviceDictionary enthalten");
            }
            else
            {
                DeviceInformation selectedDevice = deviceDictionary[selectedDeviceName];
                DisconnectDevice(selectedDevice);
                deviceDictionary.Remove(selectedDeviceName);
            }

        }
    }
}
