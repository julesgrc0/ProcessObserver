using System;
using System.Management;
using System.IO;
           
namespace Topshelf
{
    class Observer
    {
        public Observer()
        {
        }
        private void WaitForProcess()
        {
            Write($"[START] at {DateTime.Now}\n");
            ManagementEventWatcher startWatch = new ManagementEventWatcher(
              new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace"));
            startWatch.EventArrived
                                += new EventArrivedEventHandler(onProcess);
            startWatch.Start();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            Write($"[STOP] at {DateTime.Now}\n");
        }

        private void Write(string content)
        {
            string path = Directory.GetCurrentDirectory() + @"\log\log-" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            if (File.Exists(path))
            {
                File.AppendAllText(path, content);
            }
            else
            {
                File.AppendAllText(path, content);
            }
        }
        private void onProcess(object sender, EventArrivedEventArgs e)
        {
            Write($"Process started: {e.NewEvent.Properties["ProcessName"].Value} at {DateTime.Now}\n");
        }
        public void Start()
        {
            WaitForProcess();
        }
        public void Stop()
        {
            Write($"[STOP] at {DateTime.Now}\n");
        }
    }
}
