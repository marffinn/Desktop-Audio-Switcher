using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AudioDeviceSwitcher
{
    public partial class MainWindow : Window
    {
        private IMMDeviceEnumerator enumerator;
        private MMDevice[] devices;
        private const double ItemHeight = 20.0;

        public MainWindow()
        {
            InitializeComponent();enumerator = new MMDeviceEnumeratorCom();
            PopulateDeviceList();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustWindowHeight();
        }
        private void PopulateDeviceList()
        {
            try
            {
                devices = enumerator.EnumerateAudioEndPoints();
                DeviceListBox.Items.Clear();

                foreach (var device in devices)
                {
                    DeviceListBox.Items.Add(device.FriendlyName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading audio devices: {ex.Message}");
            }
        }
        private void DeviceListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DeviceListBox.SelectedIndex >= 0)
            {
                var selectedDevice = devices[DeviceListBox.SelectedIndex];
                SetDefaultAudioDevice(selectedDevice);
            }
        }
        private void SetDefaultAudioDevice(MMDevice device)
        {
            try
            {
                var policyConfig = (IPolicyConfig)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("568b9108-44bf-40b4-9006-86afe5b5a620")));
                policyConfig.SetDefaultEndpoint(device.Id, Role.Console);
                MessageBox.Show($"Switched to: {device.FriendlyName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error switching device: {ex.Message}");
            }
        }
        private void AdjustListBoxHeight()
        {
            int itemCount = DeviceListBox.Items.Count;
            DeviceListBox.Height = itemCount * ItemHeight - 7;
        }
        private void AdjustWindowHeight()
        {
            double listBoxHeight = DeviceListBox.ActualHeight;
            double windowHeight = listBoxHeight; // Padding
            this.Height = windowHeight + 48;
        }
        private MMDevice GetDefaultAudioDevice()
        {
            enumerator.GetDefaultAudioEndpoint(EDataFlow.Render, Role.Console, out IMMDevice defaultDevice);
            defaultDevice.GetId(out string defaultId);

            return devices.FirstOrDefault(d => d.Id == defaultId);
        }
        private void HighlightCurrentDefaultDevice()
        {
            var defaultDevice = GetDefaultAudioDevice();
            if (defaultDevice != null)
            {
                int index = Array.FindIndex(devices, d => d.Id == defaultDevice.Id);
                if (index >= 0)
                {
                    DeviceListBox.SelectedIndex = index;
                }
            }
        }
        private string GetShortenedName(string fullName)
        {
            const int maxLength = 60;
            if (fullName.Length > maxLength)
            {
                return fullName.Substring(0, maxLength) + "...";
            }
            return fullName;
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void Close_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize_btn_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
