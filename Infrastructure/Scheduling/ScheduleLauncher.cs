using System;
using System.Threading;

namespace Infrastructure.Scheduling
{
    class ScheduleLauncher
    {
        private const string EXCEPTION_COUNT_SETTING_NAME = "exceptions";
        private const int EXCEPTION_COUNT_MAXIMUM_VALUE = 5;
        /// <summary>
        /// Raised when a schedule has elapsed.
        /// </summary>
        public event ScheduleEventHandler ScheduleElapsed;

        private object _serviceSyncLock = new object();
        private ScheduleEventHandler _notifyElapsedEvent;
        private bool _isFatalException;
        /// <summary>
        /// Called when the service is started.
        /// </summary>
        protected void OnStart(string[] args)
        {
            try
            {
                Thread serviceThread = new Thread(new ParameterizedThreadStart(StartAsync));

                ManualResetEvent serviceStarting = new ManualResetEvent(false);
                serviceThread.Start(serviceStarting);
                serviceStarting.WaitOne();
                serviceStarting.Close();

                foreach (ScheduleBase schedule in ScheduleManager.GetSchedules())
                {
                    schedule.Settings.LastElapsed = new DateTime(0);

                    ObjectHelper wrapper = new ObjectHelper(schedule);

                    if (wrapper.HasProperty("TimeOfDay"))
                    {
                        wrapper.SetValue("TimeOfDay", DateTime.Now.TimeOfDay);
                    }

                    if (wrapper.HasProperty("DayOfMonth"))
                    {
                        wrapper.SetValue("DayOfMonth", DayOfMonth.AllDays);
                    }
                }

            }
            catch (ThreadAbortException)
            {
                // do nothing.            
            }

        }
        protected virtual void OnScheduleElapsed(ScheduleEventArgs args)
        {
            if (ScheduleElapsed != null)
            {
                ScheduleElapsed(this, args);
            }
        }

        private void StartAsync(object state)
        {

            if (ScheduleManager.GetSchedules().Count != 0)
            {
                _notifyElapsedEvent = new ScheduleEventHandler(_ScheduleElapsed);
                ScheduleManager.NotifyElapsed += _notifyElapsedEvent;
                ScheduleManager.RunScheduler();
            }

            if (state != null)
            {
                ((ManualResetEvent)state).Set();
            }
        }

        private void _ScheduleElapsed(object sender, ScheduleEventArgs e)
        {
            if (!_isFatalException)
            {
                try
                {
                    OnScheduleElapsed(e);
                }
                catch (ThreadAbortException)
                {
                    // do nothing.            
                }
                catch (Exception)
                {
                    if (!e.Settings.Contains(EXCEPTION_COUNT_SETTING_NAME))
                    {
                        e.Settings.Add(EXCEPTION_COUNT_SETTING_NAME, 0);
                    }
        
                     int exceptions = e.Settings.GetInt32(EXCEPTION_COUNT_SETTING_NAME);
                    e.Settings.LastElapsed = new DateTime(0);

                    if (++exceptions <= EXCEPTION_COUNT_MAXIMUM_VALUE)
                    {
                        e.Settings.SetValue(EXCEPTION_COUNT_SETTING_NAME, exceptions);
                    }
                    else
                    {
                        e.Settings.Remove(EXCEPTION_COUNT_SETTING_NAME);

                        _isFatalException = true;
                        throw;
                    }
                }
            }
        }

    }
}
