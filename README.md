# Audio Device Switcher

A simple WPF application to quickly switch between audio output devices on Windows. This tool allows you to list, select, and set your default audio device directly from the app.

## Features

- List all available audio output devices.
- Highlight the current default audio device.
- Switch between devices by selecting them in the list.
- Simple, intuitive interface with device names shown in the list.
- Adjust the window size automatically based on the number of audio devices.

## Requirements

- **Windows OS** (Windows 7 or later recommended)
- .NET Framework 4.7.2 or later (for .NET Framework versions) or .NET Core / .NET 5/6 (for cross-platform capabilities)
- [NAudio](https://github.com/naudio/NAudio) for audio device management.
- [AudioSwitcher.AudioApi.CoreAudio](https://github.com/xenolightning/AudioSwitcher) for switching the audio devices.

## Installation

1. Download or clone the repository.
2. Open the project in Visual Studio (or your favorite IDE).
3. Restore the NuGet packages using the **Package Manager Console**:

