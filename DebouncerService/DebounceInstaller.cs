using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DeBouncer
{
    [RunInstaller(true)]
    public class DebounceInstaller : Installer
    {
        private ServiceProcessInstaller _ServiceProcessInstaller;
        private ServiceInstaller _ServiceInstaller;

        public DebounceInstaller()
        {
            _ServiceProcessInstaller = new ServiceProcessInstaller();
            _ServiceInstaller = new ServiceInstaller();

            _ServiceProcessInstaller.Account = ServiceAccount.LocalService;

            _ServiceInstaller.StartType = ServiceStartMode.Manual;
            _ServiceInstaller.ServiceName = "Debouncer Service";

            Installers.Add(_ServiceInstaller);
            Installers.Add(_ServiceProcessInstaller);
        }
    }
}
