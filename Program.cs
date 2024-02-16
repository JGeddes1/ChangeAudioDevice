using System;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Linq;

class Program
{
    static void Main()
    {
        ChangeDefaultPlaybackDevice();
    }

    static void ChangeDefaultPlaybackDevice()
    {
        // Get the CoreAudioController
        var controller = new CoreAudioController();

        // Get a list of all playback devices
        var playbackDevices = controller.GetDevices(DeviceType.Playback, DeviceState.Active);

        // Filter devices with names containing "Speakers" and "Headphones"
        var filteredDevices = playbackDevices
            .Where(device => device.FullName.Contains("Speakers") || device.FullName.Contains("Headset"))
            .ToList();

        // Check if there are at least two suitable playback devices
        if (filteredDevices.Count() >= 2)
        {
            // Get the current default playback device
            var currentPlaybackDevice = controller.DefaultPlaybackDevice;

            Console.WriteLine("Current Default Playback Device: " + currentPlaybackDevice.FullName);

            // Find the index of the current device in the filtered list
            var currentIndex = filteredDevices.IndexOf(currentPlaybackDevice);

            // Calculate the index of the next device, wrapping around if needed
            var nextIndex = (currentIndex + 1) % filteredDevices.Count();

            // Get the next playback device
            var nextPlaybackDevice = filteredDevices.ElementAt(nextIndex);

            // Set the next playback device as the default
            controller.DefaultPlaybackDevice = nextPlaybackDevice;

            Console.WriteLine("New Default Playback Device: " + nextPlaybackDevice.FullName);
        }
        else
        {
            Console.WriteLine("Not enough suitable playback devices available.");
        }
    }
}