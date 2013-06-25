using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace MongoJobScheduler
{
    [RunInstaller(true)]
    public partial class SchedulerInstaller : System.Configuration.Install.Installer
    {
        public SchedulerInstaller()
        {
            InitializeComponent();
        }
    }
}
