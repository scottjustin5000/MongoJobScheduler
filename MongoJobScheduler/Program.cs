﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MongoJobScheduler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {

            var service = new MongoJobService();
            if (Environment.UserInteractive)
            {
                service.OnStart();
            }
            else
            {
                ServiceBase.Run(service);
            }
        }
    }
}
