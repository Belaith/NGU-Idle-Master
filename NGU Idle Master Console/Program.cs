using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Threading;

namespace NGU_Idle_Master
{
    class Program
    {
        static string filePath;
        static NguIdleMaster nguIdleMaster;
        static FileSystemWatcher watcher = new FileSystemWatcher();


        static void Main(string[] args)
        {
            filePath = @"config.xml";

            if (args.Length > 0)
            {
                filePath = args[0];
            }

            watcher = new FileSystemWatcher();
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Path = Directory.GetCurrentDirectory();
            watcher.Filter = filePath;
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            OnChanged(new object(), new FileSystemEventArgs(WatcherChangeTypes.All, Directory.GetCurrentDirectory(), filePath));

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            if (nguIdleMaster != null)
            {
                nguIdleMaster.window.Log("Config geändert, warte auf Abschluss!");
                nguIdleMaster.Stop = true;
                nguIdleMaster.mre.WaitOne();
                nguIdleMaster.window.Log("Abgeschlossen, Config wird neu geladen!");
                nguIdleMaster.Dispose();
                nguIdleMaster = null;
                Thread.Sleep(1000);
            }

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Config));
            Config config;
            using (Stream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                config = (Config)xmlSerializer.Deserialize(reader);
            }
            xmlSerializer = null;

            nguIdleMaster = new NguIdleMaster(config);

            //nguIdleMaster.window.SaveBitmap("window");

            nguIdleMaster.window.Log("Config neu geladen!");

            while (!nguIdleMaster.Stop)
            {
                nguIdleMaster.Runs();
            }
            nguIdleMaster.mre.Set();
        }
    }
}
