using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace SharpMonitor
{
    [RunInstaller(true)]
    public class MonitorInstaller : Installer
    {
        string strServiceName = "Sharp Monitoring Service";

        public MonitorInstaller()
        {
            var processInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();

            // set the privileges
            processInstaller.Account = ServiceAccount.LocalSystem;

            serviceInstaller.DisplayName = strServiceName;
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            // must be the same as what was set in Program's constructor
            serviceInstaller.ServiceName = strServiceName;

            this.Installers.Add(processInstaller);
            this.Installers.Add(serviceInstaller);

            this.Committed += new InstallEventHandler(MonitorInstaller_Committed);
        }

        void MonitorInstaller_Committed(object sender, InstallEventArgs e)
        {
            // Auto Start the Service Once Installation is Finished.
            var controller = new ServiceController(strServiceName);
            controller.Start();
        }
    }
}
