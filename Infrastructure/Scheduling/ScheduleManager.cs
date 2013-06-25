using System;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace Infrastructure.Scheduling
{
    public class ScheduleManager
    {

        private static object _configurationSyncLock = new object();

        private static ScheduleCollection _schedules;
        private static object _schedulesSyncLock = new object();

        private static ScheduleCollection _addedSchedules;
        private static object _addedSchedulesSyncLock = new object();

        private static readonly TimeSpan _schedulerInterval = TimeSpan.FromMilliseconds(5500);
        private static AutoResetEvent _stopScheduler = new AutoResetEvent(false);

        private static ScheduleCollection _elapseExpires = new ScheduleCollection();
        private static object _elapseExpiresSyncLock = new object();

        // private static ILog _logger = log4net.LogManager.GetLogger(typeof(ScheduleManager));

        public static string ScheduleCollectionType { get; set; }

        /// <summary>
        /// Raised on a dedicated thread when a schedule has elapsed. 
        /// </summary>
        public static event ScheduleEventHandler NotifyElapsed;

        /// <summary>
        /// Raised on a dedicated thread when a schedule has expired.
        /// </summary>
        public static event ScheduleEventHandler NotifyExpired;

        /// <summary>
        /// Gets/sets the scheduling configuration defined for the current application.
        /// </summary>

        private static ScheduleSection _configuration;

        private static DateTime? _maxUpdate;

        public static DateTime MaxDateUpdated
        {
            get
            {
                if (!_maxUpdate.HasValue)
                {
                    _maxUpdate = (from cnfg in _configuration.ScheduleSettings select cnfg.DateUpdated).Max();
                }
                return _maxUpdate.Value;
            }
        }

        public static ScheduleSection Configuration
        {
            get
            {
                if (!string.IsNullOrEmpty(ScheduleCollectionType))
                {
                    lock (_configurationSyncLock)
                    {
                        if (_configuration == null)
                        {
                            _configuration = ConfigurationManager.GetSectionItem(ScheduleCollectionType);
                        }
                    }
                }
                return _configuration;
            }

            set
            {
                lock (_configurationSyncLock)
                {

                    _configuration = value;

                    if (_addedSchedules != null && _addedSchedules.Count != 0)
                    {
                        _schedules = null;
                        GetSchedules();

                        foreach (ScheduleBase schedule in _addedSchedules)
                        {
                            _schedules.Add(schedule);
                        }
                    }
                    _maxUpdate = null;

                }
            }

        }

        /// <summary>
        /// Returns all of the schedules defined for the current 
        /// application. The collection returned is read-only.
        /// </summary>
        public static ScheduleCollection GetSchedules()
        {

            if (_schedules == null)
            {
                lock (_schedulesSyncLock)
                {
                    if (_schedules == null)
                    {
                        ScheduleCollection schedules = new ScheduleCollection();

                        if (Configuration != null)
                        {
                            foreach (ScheduleSettings settings in Configuration.ScheduleSettings)
                            {
                                ScheduleBase schedule = ScheduleFactory.CreateSchedule(settings);
                                schedules.Add(schedule);
                            }
                        }

                        _schedules = schedules;
                    }
                }
            }

            return ScheduleCollection.ReadOnly(ScheduleCollection.FixedSize(_schedules));
        }
        public static void AddSchedule(ScheduleBase schedule)
        {

            lock (_schedulesSyncLock)
            {
                try
                {
                    _schedules.Add(schedule);
                }
                catch (Exception)
                {
                    //_logger.Error(e.Message);
                }
            }

            if (_addedSchedules == null)
            {
                lock (_addedSchedulesSyncLock)
                {
                    if (_addedSchedules == null)
                    {
                        _addedSchedules = new ScheduleCollection();
                    }
                }
            }

            _addedSchedules.Add(schedule);
        }

        public static void RemoveSchedule(ScheduleBase schedule)
        {
            lock (_schedulesSyncLock)
            {
                try
                {
                    if (_schedules.HasKey(schedule.Name))
                    {
                        _schedules.Remove(schedule.Name);
                    }
                }
                catch (Exception)
                {
                    // _logger.Error(e.Message);
                }
            }

            if (_addedSchedules != null)
            {
                lock (_addedSchedulesSyncLock)
                {
                    if (_addedSchedules.HasKey(schedule.Name))
                    {
                        _addedSchedules.Remove(schedule.Name);
                    }
                }
            }

        }
        public static void ScheduleRefreshCheck()
        {
            if (ConfigurationManager.SchedulesStale())
            {
                UpdateSchedules();
            }
        }
        public static void UpdateSchedules()
        {
            _schedules = null;
            Configuration = ConfigurationManager.RefreshSchedule();
        }
        /// <summary>
        /// Starts the scheduler for publishing scheduled alerts.
        /// </summary>
        public static void RunScheduler()
        {
            Thread schedulerThread = new Thread(new ThreadStart(StartScheduler));
            schedulerThread.Name = "SchedulerThread";
            schedulerThread.Start();
        }

        /// <summary>
        /// Safely stops the underlying scheduler thread.
        /// </summary>
        public static void StopScheduler()
        {
            _stopScheduler.Set();
        }

        private static void StartScheduler()
        {
            while (!_stopScheduler.WaitOne(_schedulerInterval, false))
            {
                try
                {
                    foreach (ScheduleBase schedule in GetSchedules())
                    {
                        if (schedule.Enabled)
                        {
                            bool isNotRunning = false;

                            if (!_elapseExpires.HasKey(schedule.Name))
                            {
                                lock (_elapseExpiresSyncLock)
                                {
                                    if (!_elapseExpires.HasKey(schedule.Name))
                                    {
                                        _elapseExpires.Add(schedule);
                                        isNotRunning = true;
                                    }
                                }
                            }

                            if (isNotRunning)
                            {
                                bool hasElapsed = schedule.HasElapsed();
                                bool hasExpired = schedule.HasExpired();

                                if (hasElapsed)
                                {
                                    schedule.Settings.LastElapsed = DateTime.Now;

                                }

                                if (hasExpired)
                                {
                                    schedule.Settings.DateExpired = DateTime.Now;

                                }

                                if (hasElapsed || hasExpired)
                                {
                                    ScheduleEventArgs eventArgs = new ScheduleEventArgs(schedule, schedule.Settings);
                                    Thread newThread = new Thread(new ParameterizedThreadStart(ElapseExpire));
                                    newThread.Name = schedule.Name;
                                    newThread.Start(new object[] { eventArgs, hasElapsed, hasExpired });
                                }
                                else
                                {
                                    RemoveRunning(schedule);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //_logger.Error(e);
                }
            }
        }

        private static void ElapseExpire(object state)
        {
            object[] stateArray = (object[])state;
            ScheduleEventArgs eventArgs = (ScheduleEventArgs)stateArray[0];

            try
            {
                bool hasElapsed = (bool)stateArray[1];
                bool hasExpired = (bool)stateArray[2];
                WindowsImpersonationContext impersonation = null;
                IntPtr userToken = IntPtr.Zero;
                string userName = eventArgs.Settings.Identity;
                string password = eventArgs.Settings.Password;

                try
                {
                    if (!string.IsNullOrEmpty(userName) &&
                        !string.IsNullOrEmpty(password))
                    {
                        userToken = SecurityHelper.CreateToken(userName, password);

                        if (userToken != IntPtr.Zero)
                        {
                            WindowsIdentity identity = new WindowsIdentity(userToken, "NTLM");
                            impersonation = identity.Impersonate();
                        }
                    }

                    if (hasElapsed)
                    {
                        try
                        {
                            if (NotifyElapsed != null)
                            {
                                NotifyElapsed(null, eventArgs);
                            }
                        }
                        catch
                        {
                            // ignore all errors.
                        }

                        try
                        {
                            eventArgs.Schedule.OnElapsed(eventArgs);
                        }
                        catch
                        {
                            // ignore all errors.
                        }
                    }

                    if (hasExpired)
                    {
                        try
                        {
                            if (NotifyExpired != null)
                            {
                                NotifyExpired(null, eventArgs);
                            }
                        }
                        catch
                        {
                            // ignore all errors.
                        }

                        try
                        {
                            eventArgs.Schedule.OnExpired(eventArgs);
                        }
                        catch
                        {
                            // ignore all errors.
                        }
                    }
                }
                finally
                {
                    if (impersonation != null)
                    {
                        impersonation.Undo();
                        SecurityHelper.DestroyToken(userToken);
                    }
                }
            }
            catch (Exception)
            {
                //_logger.Error(exception);
            }
            finally
            {
                RemoveRunning(eventArgs.Schedule);
            }
        }

        private static void RemoveRunning(ScheduleBase schedule)
        {
            try
            {
                string scheduleName = schedule.Name;

                if (_elapseExpires.HasKey(scheduleName))
                {
                    lock (_elapseExpiresSyncLock)
                    {
                        if (_elapseExpires.HasKey(scheduleName))
                        {
                            _elapseExpires.Remove(scheduleName);
                        }
                    }
                }
            }
            catch
            {
                // do nothing.
            }
        }

    }
}
