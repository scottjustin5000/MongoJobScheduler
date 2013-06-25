
using System;
using System.Configuration;
using System.Diagnostics;

namespace MongoJobScheduler.Tasks
{
    public class BackupTask:ScheduleTask
    {
   
        public string MongoDump { get; set; }
        public string Command { get; set; }
        public override void Execute()
        {

                var process = new Process();
                process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
  
            process.StartInfo.FileName = ConfigurationManager.AppSettings["mongodump"];
                process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = ConfigurationManager.AppSettings["command"];
        
                process.Start();




        }
    }
}
