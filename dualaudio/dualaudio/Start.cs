using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Threading;

namespace dualaudio
{
    class Start
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(pHandleAssemblyResolver);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            Program.Main();
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("There was an error! {0}", "Error!\n" + e.ExceptionObject.ToString());
        }

        private static System.Reflection.Assembly pHandleAssemblyResolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.Contains("NAudio"))
            {
                return Assembly.Load(DecompressAssembly(Properties.Resources.NAudio));
            }
            else
                return null;
        }

        public static byte[] DecompressAssembly(byte[] assembly)
        {
            Stream stream = new MemoryStream(assembly);
            using (var decompress = new GZipStream(stream, CompressionMode.Decompress, false))
            {
                try
                {
                    const int size = 1024;
                    byte[] buffer = new byte[size];
                    using (MemoryStream memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = decompress.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.ToString());
                    return null;
                }
            }
        }
    }
}
