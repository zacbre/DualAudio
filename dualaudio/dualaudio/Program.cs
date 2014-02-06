using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.IO;
using System.Threading;
using System.IO.Compression;
using System.Reflection;
namespace dualaudio
{
    class Program
    {
        static WasapiLoopbackCapture waveIn;
        static BufferedWaveProvider[] m1;
        static int totaloutputs = 0;
        public static void Main()
        {
            //Decrypt resources and load.
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("Dual Audio - (C) 2013 Thr. Using NAudio (http://naudio.codeplex.com/)");
            Console.WriteLine("---------------------------------------------------------------------");

            int waveInDevices = WaveIn.DeviceCount;
            int waveOutDevices = WaveOut.DeviceCount;
            int inputdevice = 0;
            int output = 0;

            List<int> Outputs = new List<int>();
            Console.WriteLine("Located {0} Input Devices.\n", waveInDevices);
            Console.Write("How many outputs to bind to? (max {0}): ", waveOutDevices);
            //grab inputs.
            
            while (!int.TryParse(Console.ReadLine(), out totaloutputs) || totaloutputs > waveOutDevices)
                Console.Write("How many outputs to bind to? (max {0}): ", waveOutDevices);

            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("{0}: {1}, {2} channels.", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }

            Console.Write("Select Input Line: ");           
            while (!int.TryParse(Console.ReadLine(), out inputdevice))
                Console.Write("Select Input Line: ");

            Console.WriteLine("Successfully set input as device {0}.", inputdevice);
            Console.WriteLine("");
            output = totaloutputs;
            while (output > 0)
            {
                for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
                {
                    if (!Outputs.Contains(waveOutDevice))
                    {
                        WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                        Console.WriteLine("{0}: {1}, {2}", waveOutDevice, deviceInfo.ProductName, deviceInfo.Channels);
                    }
                }
                Console.Write("Select the output device for playback{0}: ", (totaloutputs - output).ToString());
                int device = 0;
                while(!int.TryParse(Console.ReadLine(), out device) || device > waveOutDevices - 1)
                {
                    Console.WriteLine("Invalid Device!");
                    Console.Write("Select the output device for playback{0}: ", (totaloutputs - output).ToString());
                }
                Outputs.Add(device);
                Console.WriteLine("Successfully set the output device for playback{0}.", (totaloutputs - output).ToString());
                output--;
            }
            Console.WriteLine("");

            waveIn = new WasapiLoopbackCapture();

            Console.WriteLine("Initialized Loopback Capture...");

            waveIn.DataAvailable += InputBufferToFileCallback;
            waveIn.StartRecording(); //Start our loopback capture.
            WaveOut[] devices = new WaveOut[totaloutputs];

            m1 = new BufferedWaveProvider[totaloutputs];
            for (int i = 0; i < totaloutputs; i++)
            {
                m1[i] = new BufferedWaveProvider(waveIn.WaveFormat);
                m1[i].BufferLength = 1024 * 1024 * 5;
                devices[i] = new WaveOut();
                devices[i].DeviceNumber = Outputs[i];
                devices[i].Init(m1[i]);
                Console.WriteLine("Initializing Device{0}...", i);
                devices[i].Play();
                Console.WriteLine("Started Playing on Device{0}...", i);
            }

            while (true)
                Thread.Sleep(10000);
        }

        
        private static void InputBufferToFileCallback(object sender, WaveInEventArgs e)
        {
            //write to our audio sample buffers.
            for (int i = 0; i < totaloutputs; i++)
                m1[i].AddSamples(e.Buffer, 0, e.BytesRecorded);          
        }
    }
}
