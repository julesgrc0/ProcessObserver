using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System.Management;
using System.IO;
using System.Net.Http;
using Topshelf;

namespace ProcessObserver
{
    class Program
    {
        static void Main(string[] args)
        {
            TopshelfExitCode TSExitCode = HostFactory.Run(x =>
            {
                x.Service<Observer>(s =>
                {
                    s.ConstructUsing(Observer => new Observer());
                    s.WhenStarted(Observer =>  Observer.Start());
                    s.WhenStopped(Observer =>  Observer.Stop());
                });
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.SetDisplayName("ProccessObserverLocal");
                x.SetDescription("Observes all starting services and writes their names in a log file");
            });
            int exitCode = (int)Convert.ChangeType(TSExitCode, TSExitCode.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        
    }
}

