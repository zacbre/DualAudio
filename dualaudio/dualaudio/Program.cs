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
        static int speaker1;
        static int speaker2;
        static int input;
        static WasapiLoopbackCapture waveIn;
        static BufferedWaveProvider m1;
        static BufferedWaveProvider m2;
        
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
                /*if (deviceInfo.ProductName.StartsWith("Line 1"))
                {
                    input = waveInDevice;
                    Console.WriteLine("Detected input as {1}:{0}", deviceInfo.ProductName, input);
                }*/
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
                /*if (deviceInfo.ProductName.StartsWith("ASUS"))
                {
                    speaker2 = waveOutDevice;
                    Console.WriteLine("Detected output2 as {1}:{0}", deviceInfo.ProductName, speaker2);
                }
                else if (deviceInfo.ProductName.StartsWith("Speakers"))
                {
                    speaker1 = waveOutDevice;
                    Console.WriteLine("Detected output1 as {1}:{0}", deviceInfo.ProductName, speaker1);
                }*/
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
            ///
            output2:
            Console.WriteLine("");
            Console.Write("Select Device2: ");
            speaker2 = int.Parse(Console.ReadLine());
            if (speaker2 > waveOutDevices - 1)
            {
                Console.WriteLine("That device doesn't exist!");
                goto output2;
            }
            if (speaker2 != speaker1)
            {
                Console.WriteLine("Successfully set Output2 as device {0}.", speaker2);
            }
            else
            {
                Console.WriteLine("You can't select the same output!");
                goto output2;
            }
            Console.WriteLine("");
            waveIn = new WasapiLoopbackCapture();
            Console.WriteLine("Initialized Loopback Capture...");
            waveIn.DataAvailable += InputBufferToFileCallback;
            waveIn.StartRecording();
            m1 = new BufferedWaveProvider(waveIn.WaveFormat);
            m2 = new BufferedWaveProvider(waveIn.WaveFormat);
            m1.BufferLength = 1024 * 1024 * 5;
            m2.BufferLength = 1024 * 1024 * 5;
            Thread.Sleep(1000);
            //initialize two audio output devices.
            WaveOut device1 = new NAudio.Wave.WaveOut();
            device1.DeviceNumber = speaker1;
            device1.Init(m1);
            Console.WriteLine("Initializing Device1...");
            device1.Play();
            Console.WriteLine("Started Playing on Device1...");

            WaveOut device2 = new NAudio.Wave.WaveOut();
            device2.DeviceNumber = speaker2;
            device2.Init(m2);
            Console.WriteLine("Initializing Device2...");
            device2.Play();
            Console.WriteLine("Started Playing on Device2...");
            //Now 
            while (true)
                Thread.Sleep(10000);
        }

        
        private static void InputBufferToFileCallback(object sender, WaveInEventArgs e)
        {
            //play audio through two inputs.
            m1.AddSamples(e.Buffer, 0, e.BytesRecorded);          
            m2.AddSamples(e.Buffer, 0, e.BytesRecorded);
        }
    }
}
