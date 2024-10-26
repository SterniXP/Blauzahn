using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;


namespace Blauzahn
{
    public partial class Form1 : Form
    {
        private const string BATTERY_SERVICE_UUID_PREFIX = "0000180f";

        private readonly DeviceWatcher deviceWatcher;
        private readonly ObservableCollection<DeviceInformation> devices = new ObservableCollection<DeviceInformation>();
        private readonly ObservableCollection<DeviceInformation> devicesView = new ObservableCollection<DeviceInformation>();

        public Form1()
        {
            InitializeComponent();
            PopulateOutputComboBox();
            BindDevicesViewToData();
            BindDevicesViewToListBox();
            // will run the entire time in the backround
            //  should we stop it when not neeeded? -- how would that and the restart be implemented?
            deviceWatcher = CreateBLEWatcher();
            deviceWatcher.Start();
        }

        private void BindDevicesViewToData()
        {
            devices.CollectionChanged += (s, e) =>
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        AddNewItemsToViewData(e.NewItems);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        RemoveOldItemsFromViewData(e.OldItems);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        RemoveOldItemsFromViewData(e.OldItems);
                        AddNewItemsToViewData(e.NewItems);
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Reset:
                        devicesView.Clear();
                        break;
                }
            };
        }

        /// <summary>
        /// update method for adding new items to the viewData
        /// </summary>
        /// <param name="newItems"></param>
        private void AddNewItemsToViewData(IList newItems)
        {
            foreach (DeviceInformation device in newItems)
            {
                if (IsMatchingFilter(device.Name))
                {
                    devicesView.Add(device);
                }
            }
        }

        /// <summary>
        /// update method for removing old items from the viewData
        /// </summary>
        /// <param name="oldItems"></param>
        private void RemoveOldItemsFromViewData(IList oldItems)
        {
            foreach (DeviceInformation device in oldItems)
            {
                if (devicesView.Contains(device))
                {
                    devicesView.Remove(device);
                }
            }
        }

        /// <summary>
        /// currently this only makes sure the name is not null or empty
        /// </summary>
        /// <param name="name">name of the bluetooth LE device</param>
        /// <returns></returns>
        private bool IsMatchingFilter(string name)
        {
            if (sensorNamesTextBox.Lines.Length == 0)
            {
                return true;
            }
            foreach (var filter in sensorNamesTextBox.Lines)
            {
                if (name.Contains(filter.Trim()))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// determines what is shown in the listbox as the items representation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">args of the event</param>
        private void ListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            var deviceInfo = (DeviceInformation)e.ListItem;
            e.Value = deviceInfo.Name;
        }

        /// <summary>
        /// binds the devicesView updates to the listbox
        /// </summary>
        private void BindDevicesViewToListBox()
        {
            if (BluetoothDevicesListBox.InvokeRequired)
            {
                BluetoothDevicesListBox.Invoke(new MethodInvoker(delegate
                {
                    BluetoothDevicesListBox.DataSource = devicesView;
                }));
            }
            else
            {
                BluetoothDevicesListBox.DataSource = devicesView;
            }
            BluetoothDevicesListBox.Format += new ListControlConvertEventHandler(ListBox_Format);
            devicesView.CollectionChanged += (s, e) =>
            {
                BluetoothDevicesListBox.DataSource = null;
                BluetoothDevicesListBox.DataSource = devicesView;
            };
        }

        /// <summary>
        /// populates the output combobox with the available options and sets the default value (CSV)
        /// </summary>
        private void PopulateOutputComboBox()
        {
            saveFormatComboBox.Items.Add("CSV");
            saveFormatComboBox.Items.Add("HEX");
            saveFormatComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// creates a new DeviceWatcher for Bluetooth LE devices and adds all necessary event handlers
        /// </summary>
        /// <returns></returns>
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
        /// adds any device found by the deviceWatcher to the devices list
        /// happens when the watcher starts or the device comes into range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            if (IsNamed(args.Name))
            {
                devices.Add(args);
            }
        }

        /// <summary>
        /// filter unnamed devices and false positives
        /// </summary>
        /// <param name="name">name of the Bluetooth LE device</param>
        /// <returns>true if the name is not null or empty else false</returns>
        private static bool IsNamed(string name)
        {
            return name != null && name != "";
        }

        /// <summary>
        /// updates the device information for the first device in the list that has the same id as the args
        /// happens when the device is still in range but has changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (devices.Any(d => d.Id == args.Id))
            {
                devices.First(d => d.Id == args.Id).Update(args);
            }
        }

        /// <summary>
        /// removes the device with the same id as the args from the devices list
        /// happens when the device is no longer in range
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (devices.Any(d => d.Id == args.Id))
            {
                devices.Remove(devices.First(d => d.Id == args.Id));
            }
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


        // this is just a ruderimental test for the data processing
        int _packetIndex = 0;
        long _startTime;
        int _packetCounter = 0;
        static int _packetNumber = 1111;
        static int _packetSize = 1;
        double[,] _data = new double[_packetSize, _packetNumber];
        int _maxPacketSize = 60;
        int _errorCounter = 0;
        void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            if (args.CharacteristicValue.Length != 20)
            {
                _errorCounter++;
                return;
            }
            // this how to read the values from the args
            DataReader reader = DataReader.FromBuffer(args.CharacteristicValue);
            byte[] inputBytes = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(inputBytes);
            // proceed with the data as needed...
            for (int i = 0; i < inputBytes.Length; i += sizeof(Single))
            {
                Single floatyBoy = BitConverter.ToSingle(inputBytes, i);
                _data[_packetIndex, _packetCounter] = floatyBoy;
                if (++_packetIndex >= _packetSize)
                {
                    _packetCounter++;
                    _packetIndex = 0;
                }
                if (_packetCounter == _packetNumber)
                {
                    // print results
                    long elapsedTime = (DateTime.Now.Ticks - _startTime) / 10000;
                    double packetsPerSecond = _packetNumber / (elapsedTime / 1000.0);
                    double averageDeviationPercent = 0;
                    StringBuilder sb = new StringBuilder();
                    for (int j = 0; j < _packetSize; j++)
                    {
                        double sum = 0;
                        double backup = 0;
                        for (int k = 0; k < _packetNumber; k++)
                        {
                            backup = sum;
                            sum += _data[j, k];
                            if (Double.IsNaN(sum))
                            {
                                sum = backup;
                            }
                        }
                        double average = sum / _packetNumber;
                        // avoid division by zero (smallest possible double value)
                        if (average == 0) average = 2.225E-307;
                        double deviationPercent = 0.0;
                        for (int k = 0; k < _packetNumber; k++)
                        {
                            // catch / skip errorinous values
                            backup = deviationPercent;
                            deviationPercent += (Math.Abs(_data[j, k] - average));
                            if (Double.IsNaN(deviationPercent))
                            {
                                deviationPercent = backup;
                            }
                        }
                        deviationPercent = ((deviationPercent / average) / _packetNumber) * 100;
                        averageDeviationPercent += (deviationPercent / _packetSize);
                        sb.Append("Value " + (j+1) + " average deviation: " + deviationPercent.ToString("#.000") + "%\n");
                    }
                    Console.WriteLine($"PacketSize (number of floats): {_packetSize}" +
                        $", Average deviation: {averageDeviationPercent}" +
                        $", PackerNumber: {_packetNumber}" +
                        $", Time: {elapsedTime}ms" +
                        $", Packets/s: {packetsPerSecond}\n" +
                        $"{sb}");
                    // clear data and prepare next run with larger packet size
                    _packetCounter = 0;
                    _packetSize++;
                    if (_packetSize > _maxPacketSize)
                    {
                        Console.WriteLine("ErrorCounter: "+_errorCounter);
                    }
                    Console.WriteLine("New run with Packet size: " + _packetSize);
                    _startTime = DateTime.Now.Ticks;
                    _data = new double[_packetSize, _packetNumber];
                }
            }
            //printBuffer(args.CharacteristicValue);
        }

        /// <summary>
        /// prints a given buffer to the console
        /// </summary>
        /// <param name="buffer"></param>
        void printBuffer(IBuffer buffer)
        {
            DataReader reader = DataReader.FromBuffer(buffer);
            byte[] inputBytes = new byte[reader.UnconsumedBufferLength];
            reader.ReadBytes(inputBytes);
            Console.WriteLine("Raw Data: " + BitConverter.ToString(inputBytes));
        }

        private int _singleAmount = 0;
        private readonly StringBuilder _rawDataBuilder = new StringBuilder();
        private readonly StringBuilder _singleDataBuilder = new StringBuilder();
        void Characteristic_ValueChanged2(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            try
            {
                if (_singleAmount >= 20)
                {
                    Console.WriteLine("Time: " + DateTime.Now.ToString());
                    Console.WriteLine("Raw data: "+_rawDataBuilder.ToString());
                    _rawDataBuilder.Clear();
                    Console.WriteLine("Single data:" + _singleDataBuilder.ToString());
                    _singleAmount = 0;
                    _rawDataBuilder.Clear();
                    _singleDataBuilder.Clear();
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
                _rawDataBuilder.Append(BitConverter.ToString(inputBytes));

                // Process each byte as an individual value
                // TODO: add a switch / interface to handle different data types (optional: based on user input)
                for (int j = 1; j <= inputBytes.Length; j += sizeof(Single))
                {
                    _singleAmount++;
                    int i = j - 1;
                    if (i + sizeof(Single) <= inputBytes.Length)
                    {
                        _singleDataBuilder.Append(" " + BitConverter.ToSingle(inputBytes, i));
                    }
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("Error reading data: " + ex.Message);
                // Handle or log the exception as needed
            }
        }

        private async void ConnectDevice(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            // find devices that can be connected to
            BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            // pairs with the device
            GattDeviceServicesResult result = await device.GetGattServicesAsync();
            // check if the device has services
            if (result.Status == GattCommunicationStatus.Success)
            {
                Console.WriteLine("Device: " + device.Name + " - " + device.DeviceId);
                await SubscribeToDataServices(result);
            }
            else
            {
                Console.WriteLine("Error connecting to device: " + result.Status);
            }
        }

        /// <summary>
        /// this attempts to subscribe to the Battery service of the device (current implementation is for LPMS only)
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private async Task SubscribeToDataServices(GattDeviceServicesResult result)
        {
            var services = result.Services;
            foreach (var service in services)
            {
                // check if the device has the battery service -- used by LPMS to transfer data
                // TODO: add a switch / interface to handle different services (optional: based on user input)
                //if (service.Uuid.ToString().StartsWith(BATTERY_SERVICE_UUID_PREFIX))
                if (service.Uuid.ToString().StartsWith(BATTERY_SERVICE_UUID_PREFIX))
                {
                    await SubscribeToNotifyingCharacterristics(service);
                }
            }
        }

        /// <summary>
        /// attempts to subscribe to the notifying characteristics of the service (current implementation is for LPMS only)
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        private async Task SubscribeToNotifyingCharacterristics(GattDeviceService service)
        {
            var characteristicsResult = await service.GetCharacteristicsAsync();
            if (characteristicsResult.Status == GattCommunicationStatus.Success)
            {
                foreach (var characteristic in characteristicsResult.Characteristics)
                {
                    if (characteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                    {
                        GattCommunicationStatus status = await characteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                        if (status == GattCommunicationStatus.Success)
                        {
                            SubscribeToAcceptedCharacteristic(characteristic);
                        }
                    }
                    // write / read characteristics here if necessary
                }
            }
        }

        private void SubscribeToAcceptedCharacteristic(GattCharacteristic characteristic)
        {
            // TODO: create a new separate data stream handler
            characteristic.ValueChanged += Characteristic_ValueChanged;
        }

        private void sensorNamesTextBox_TextChanged(object sender, EventArgs e)
        {
            // TODO: update filter and filter devicesView with updated filter
        }

        /// <summary>
        /// connect button event handler
        ///  is currently hardcoded to connect to the first device in the list that starts with "LPMS" for simplicity reasons (Listbox wont update correctly)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void connectButton_Click(object sender, EventArgs e)
        {
            DeviceInformation deviceInfo = null;
            try
            {
                int index = BluetoothDevicesListBox.SelectedIndex;
                if (index > -1 && index < BluetoothDevicesListBox.Items.Count && BluetoothDevicesListBox.SelectedItem != null)
                {
                    deviceInfo = (DeviceInformation)BluetoothDevicesListBox.SelectedItem;
                }
                foreach (DeviceInformation information in devices)
                {
                    if (information.Name.StartsWith("LPMS"))
                    {
                        deviceInfo = information;
                        break;
                    }
                }
            }
            catch (IndexOutOfRangeException) { }
            if (deviceInfo != null)
            {
                ConnectDevice(deviceInfo);
            }
        }
    }
}
