using System;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using System.Linq;
using WindowsInput;
using System.Threading;

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

        // Set up global hotkey for Ctrl+N
        var inputSimulator = new InputSimulator();
        var hotkey = "^N"; // Ctrl+N

        // Get a list of all playback devices
        var playbackDevices = controller.GetDevices(DeviceType.Playback, DeviceState.Active);
        Console.Write(playbackDevices);
        Console.WriteLine(DeviceType.Playback);

        // Filter devices with names containing "Speakers" and "Headphones"
        var filteredDevices = playbackDevices
            .Where(device => device.FullName.Contains("Speakers") || device.FullName.Contains("Headset"))
            .ToList();

        for (int i = 0; i < filteredDevices.Count; i++)
        {
            var device = filteredDevices[i];
            Console.WriteLine(device.FullName);
        }

        // Check if there are at least two suitable playback devices
        if (filteredDevices.Count() >= 2)
        {
            // Initialize an index to keep track of the current device
            int currentIndex = 0;

            // Use a boolean flag to allow graceful exit
            bool exitFlag = false;

            // Run the loop until the exitFlag is set
            while (!exitFlag)
            {
                // Check for the hotkey combination
                if (inputSimulator.InputDeviceState.IsKeyDown(WindowsInput.Native.VirtualKeyCode.CONTROL) &&
                    inputSimulator.InputDeviceState.IsKeyDown(WindowsInput.Native.VirtualKeyCode.VK_N))
                {
                    // Get the current default playback device
                    var currentPlaybackDevice = controller.DefaultPlaybackDevice;

                    Console.WriteLine("Current Default Playback Device: " + currentPlaybackDevice.FullName);

                    // Calculate the index of the next device, wrapping around if needed
                    int nextIndex = (currentIndex + 1) % filteredDevices.Count;



                    // Get the next playback device
                    var nextPlaybackDevice = filteredDevices[nextIndex];

                    // Set the next playback device as the default
                    controller.DefaultPlaybackDevice = nextPlaybackDevice;

                    Console.WriteLine("New Default Playback Device: " + nextPlaybackDevice.FullName);

                    // Increment the index for the next iteration
                    currentIndex = nextIndex;

                    // Sleep to avoid continuous hotkey triggering
                    Thread.Sleep(2000);
                }

                // You can add additional conditions here for other actions or exit criteria

                // Sleep to avoid high CPU usage in the loop
                Thread.Sleep(100);
            }
        }
        else
        {
            Console.WriteLine("Not enough suitable playback devices available.");
        }
    }
}
