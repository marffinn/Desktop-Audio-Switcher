using System;
using System.Linq;
using System.Windows;
using NAudio.CoreAudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Windows.Input;

namespace AudioDeviceSwitcher
{
    public partial class MainWindow : Window
    {
        private MMDeviceEnumerator enumerator;
        private MMDeviceCollection devices;
        private CoreAudioController audioController;

        private const double ItemHeight = 20.0;

        public MainWindow()
        {
            InitializeComponent();
            enumerator = new MMDeviceEnumerator();
            devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            audioController = new CoreAudioController();
            PopulateDeviceList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AdjustWindowHeight(); // Adjust the window height after layout
        }

        private void PopulateDeviceList()
        {
            DeviceListBox.Items.Clear();
            foreach (var device in devices)
            {
                DeviceListBox.Items.Add(GetShortenedName(device.FriendlyName));
            }
            AdjustListBoxHeight();
            AdjustWindowHeight();
            HighlightCurrentDefaultDevice();
        }

        private void AdjustListBoxHeight()
        {
            int itemCount = DeviceListBox.Items.Count;
            DeviceListBox.Height = itemCount * ItemHeight;
        }

        private void AdjustWindowHeight()
        {
            double listBoxHeight = DeviceListBox.ActualHeight;
            double windowHeight = listBoxHeight; // Padding
            this.Height = windowHeight+60;
        }

        private void HighlightCurrentDefaultDevice()
        {
            var defaultDevice = audioController.GetDefaultDevice(AudioSwitcher.AudioApi.DeviceType.Playback, AudioSwitcher.AudioApi.Role.Console);
            if (defaultDevice != null)
            {
                int defaultIndex = devices
                    .Select((device, index) => new { device, index })
                    .FirstOrDefault(x => GetShortenedName(x.device.FriendlyName) == GetShortenedName(defaultDevice.FullName))?.index ?? -1;

                if (defaultIndex >= 0)
                {
                    DeviceListBox.SelectedIndex = defaultIndex;
                }
            }
        }

        private void DeviceListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (DeviceListBox.SelectedIndex >= 0)
            {
                var selectedDevice = devices[DeviceListBox.SelectedIndex];
                try
                {
                    SetDefaultAudioDevice(selectedDevice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error switching device: {ex.Message}");
                }
            }
        }

        private void SetDefaultAudioDevice(MMDevice naudioDevice)
        {
            var audioSwitcherDevice = audioController.GetPlaybackDevices()
                .FirstOrDefault(d => GetShortenedName(d.FullName) == GetShortenedName(naudioDevice.FriendlyName));

            if (audioSwitcherDevice == null)
            {
                throw new Exception($"Device not found: {naudioDevice.FriendlyName}");
            }
            audioSwitcherDevice.SetAsDefault();
            HighlightCurrentDefaultDevice();
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
        private void CloseApp(object sender) 
        {
            Close();
        }
    }
}
