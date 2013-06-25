using System;
using System.ServiceProcess;
using System.Threading;
using Infrastructure;
using Infrastructure.Scheduling;
using MongoJobScheduler.Tasks;

namespace MongoJobScheduler
{
    public partial class MongoJobService : ServiceBase
    {
        private ScheduleEventHandler _notifyElapsedEvent;
        private ScheduleEventHandler _notifyExpiredEvent;
        static Factory<ScheduleTask> _taskFactory;
        public MongoJobService()
        {
            _taskFactory = new Factory<ScheduleTask>();
            //InitializeComponent();
        }
        private void RunProcess(object state)
        {
            ScheduleManager.ScheduleCollectionType ="TaskConfig"; 
                //System.Configuration.ConfigurationManager.AppSettings["configProvider"];
            _notifyElapsedEvent = new ScheduleEventHandler(_ScheduleElapsed);
            _notifyExpiredEvent = new ScheduleEventHandler(_ScheduleExpired);
            ScheduleManager.NotifyElapsed += _notifyElapsedEvent;
            ScheduleManager.NotifyExpired += _notifyExpiredEvent;
            ScheduleManager.RunScheduler();

            if (state != null)
            {
                ((ManualResetEvent)state).Set();
            }


        }
        private void _ScheduleExpired(object sender, ScheduleEventArgs e)
        {
            ScheduleManager.RemoveSchedule(e.Schedule);
        }
        private void _ScheduleElapsed(object sender, ScheduleEventArgs e)
        {
            try
            {
                OnScheduleElapsed(e);
            }
            catch (ThreadAbortException)
            {
                //do nothing
            }
        }
        protected void OnScheduleElapsed(ScheduleEventArgs args)
        {
            string tsk = args.Settings.GetString("task");
            var t = _taskFactory.CreateObject(tsk);
       

            Try.Do(() => t.Execute(), error =>
            {
                //errorHandler(error);
            });


        }

        public void OnStart()
        {
            OnStart(null);
        }
        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Thread serviceThread = new Thread(new ParameterizedThreadStart(RunProcess));

            ManualResetEvent serviceStarting = new ManualResetEvent(false);
            serviceThread.Start(serviceStarting);
            serviceStarting.WaitOne();
            serviceStarting.Close();
        }

        protected override void OnStop()
        {
            ScheduleManager.StopScheduler();
            base.OnStop();
        }
    }
}
