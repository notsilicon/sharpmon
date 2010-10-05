using System.ServiceProcess;
using System;
using System.Reflection;
using System.Configuration.Install;

using System.IO.Ports;
using Modbus.Device;

namespace SharpMonitor
{
    static class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0] == "/i")
                {
                    ManagedInstallerClass.InstallHelper(new string[] { Assembly.GetExecutingAssembly().Location });
                }
                else if (args[0] == "/u")
                {
                    ManagedInstallerClass.InstallHelper(new string[] { "/u", Assembly.GetExecutingAssembly().Location });
                }
            }
            else
            {
                ServiceBase[] servicesToRun = new ServiceBase[] { new MonitorService() };

                if (Environment.UserInteractive)
                {
                    Type type = typeof(ServiceBase);
                    BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
                    MethodInfo method = type.GetMethod("OnStart", flags);

                    foreach (ServiceBase service in servicesToRun)
                    {
                        method.Invoke(service, new object[] { args });
                    }

                    Console.WriteLine("Press any key to exit...");
                    Console.Read();

                    foreach (ServiceBase service in servicesToRun)
                    {
                        service.Stop();
                    }
                }
                else
                {
                    ServiceBase.Run(servicesToRun);
                }
            }
        }
    }
}