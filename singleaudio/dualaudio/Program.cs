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
        static int speaker1, input;
        static WasapiLoopbackCapture waveIn;
        static BufferedWaveProvider m1;
        public static void Main()
        {
            //Decrypt resources and load.
            Console.WriteLine("---------------------------------------------------------------------");
            Console.WriteLine("Dual Audio - (C) 2013 Thr. Using NAudio (http://naudio.codeplex.com/)");
            Console.WriteLine("---------------------------------------------------------------------");
            int waveInDevices = WaveIn.DeviceCount;
            for (int waveInDevice = 0; waveInDevice < waveInDevices; waveInDevice++)
            {
                WaveInCapabilities deviceInfo = WaveIn.GetCapabilities(waveInDevice);
                Console.WriteLine("{0}: {1}, {2} channels.", waveInDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }
            input:
            Console.Write("Select Input: ");
            input = int.Parse(Console.ReadLine());
            if (input > waveInDevices - 1)
            {
                Console.WriteLine("That device doesn't exist!");
                goto input;
            }
            Console.WriteLine("Successfully set input as device {0}.", input);
            Console.WriteLine("");
            int waveOutDevices = WaveOut.DeviceCount;
            for (int waveOutDevice = 0; waveOutDevice < waveOutDevices; waveOutDevice++)
            {
                WaveOutCapabilities deviceInfo = WaveOut.GetCapabilities(waveOutDevice);
                Console.WriteLine("{0}: {1}, {2}", waveOutDevice, deviceInfo.ProductName, deviceInfo.Channels);
            }
            output1:
            Console.Write("Select Device1: ");
            speaker1 = int.Parse(Console.ReadLine());
            if (speaker1 > waveOutDevices - 1)
            {
                Console.WriteLine("That device doesn't exist!");
                goto output1;
            }
            Console.WriteLine("Successfully set Output1 as device {0}.", speaker1);

            Console.WriteLine("");

            waveIn = new WasapiLoopbackCapture();

            Console.WriteLine("Initialized Loopback Capture...");

            waveIn.DataAvailable += InputBufferToFileCallback;
            waveIn.StartRecording(); //Start our loopback capture.

            m1 = new BufferedWaveProvider(waveIn.WaveFormat);

            m1.BufferLength = 1024 * 1024 * 5;

            //initialize two chosen audio output devices.
            WaveOut device1 = new NAudio.Wave.WaveOut();
            device1.DeviceNumber = speaker1;
            device1.Init(m1);
            Console.WriteLine("Initializing Device...");
            device1.Play();
            Console.WriteLine("Started Playing on Device...");

            //Now 
            while (true)
                Thread.Sleep(10000);
        }

        
        private static void InputBufferToFileCallback(object sender, WaveInEventArgs e)
        {
            //write to our audio sample buffers.
            m1.AddSamples(e.Buffer, 0, e.BytesRecorded);          
        }
    }
}
