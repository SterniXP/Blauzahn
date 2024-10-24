using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
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
            //devices.CollectionChanged += UpdateDevicesListBox;
        }

        //private void UpdateDevicesListBox(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    // TODO: find a way to bind the list to the listbox without reloading the entire list each time
        //    // TODO: this currently only updates when elements are added, removed or replaced but NOT when elements are updated
        //    BluetoothDevicesListBox.DataSource = null;
        //    BluetoothDevicesListBox.DataSource = devicesView;
        //}

        private void AddNewItemsToViewData(IList newItems)
        {
            Console.WriteLine("Adding new items to view data");
            foreach (DeviceInformation device in newItems)
            {
                if (IsMatchingFilter(device.Name))
                {
                    Console.WriteLine("Adding device to view data: " + device.Name);
                    devicesView.Add(device);
                }
            }
        }

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
        private void ListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            var deviceInfo = (DeviceInformation)e.ListItem;
            e.Value = deviceInfo.Name;
        }

        private void BindDevicesViewToListBox()
        {
            BluetoothDevicesListBox.DataSource = devicesView;
            BluetoothDevicesListBox.Format += new ListControlConvertEventHandler(ListBox_Format);
            devicesView.CollectionChanged += (s, e) =>
            {
                BluetoothDevicesListBox.DataSource = null;
                BluetoothDevicesListBox.DataSource = devicesView;
            };
            //devicesView.CollectionChanged += (s, e) =>
            //{
            //    //Console.Write("Devices collection changed: " + e.Action + ", New Items: ");
            //    //if (e.NewItems != null)
            //    //{
            //    //    foreach (DeviceInformation device in e.NewItems)
            //    //    {
            //    //        Console.Write(device.Name + ", ");
            //    //    }
            //    //}
            //    //Console.Write(", Old Items: ");
            //    //if (e.OldItems != null)
            //    //{
            //    //    foreach (DeviceInformation device in e.OldItems)
            //    //    {
            //    //        Console.Write(device.Name + ", ");
            //    //    }
            //    //}
            //    //Console.WriteLine();
            //    if (BluetoothDevicesListBox.InvokeRequired)
            //    {
            //        BluetoothDevicesListBox.Invoke(new MethodInvoker(delegate
            //        {
            //            UpdateDevicesListBox();
            //        }));
            //    }
            //    else
            //    {
            //        UpdateDevicesListBox();
            //    }
            //};
        }

        private void BluetoothDevicesListBox_Format(object sender, ListControlConvertEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void PopulateOutputComboBox()
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

        private void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation args)
        {
            Console.WriteLine("DeviceWatcher_Added: " + args.Name + " - " + args.Id);
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

        private void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args)
        {
            if (devices.Any(d => d.Id == args.Id))
            {
                devices.First(d => d.Id == args.Id).Update(args);
            }
        }

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


        private int singleAmount = 0;
        private StringBuilder rawDataBuilder = new StringBuilder();
        private StringBuilder singleDataBuilder = new StringBuilder();
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

        private void Characteristic_ValueChanged2(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            Console.WriteLine("parallel<-------------------------------");
        }

        private async void ConnectDevice(DeviceInformation deviceInfo)
        {
            // Note: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
            // find devices that can be connected to
            BluetoothLEDevice device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
            // technically this already connect to the device but also realiably filters paired devices that are not in range
            GattDeviceServicesResult result = await device.GetGattServicesAsync();
            // check if the device has services
            if (result.Status == GattCommunicationStatus.Success)
            {
                await SubscribeToDataServices(result);
            }
        }

        private async Task SubscribeToDataServices(GattDeviceServicesResult result)
        {
            var services = result.Services;
            foreach (var service in services)
            {
                // check if the device has the battery service -- used by LPMS to transfer data
                // TODO: add a switch / interface to handle different services (optional: based on user input)
                if (service.Uuid.ToString().StartsWith(BATTERY_SERVICE_UUID_PREFIX))
                {
                    await SubscribeToNotifyingCharacterristics(service);
                }
            }
        }

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
    }
}
