using System;
using System.Runtime.Serialization;

namespace Infrastructure.Scheduling
{
    [Serializable]
    public abstract class ScheduleBase
    {

        private ScheduleSettings _settings;
        public event ScheduleEventHandler Elapsed;
        public event ScheduleEventHandler Expired;

        private bool _enabled;
        private string _name;
        protected ScheduleBase() { }
        protected ScheduleBase(SerializationInfo info, StreamingContext context) { }

        public virtual void Configure(ScheduleSettings settings, bool preconfigured)
        {


            //get rid of this make it work like all other classes,
            //then can get rid of chain of configuration.
            _settings = settings;
            _enabled = settings.Enabled;
            _settings = settings;
            _name = settings.Name;
            SubType = settings.SubType;
            Id = settings.Id;


        }
        public string SubType
        {

            get;
            set;


        }
        public int Id { get; set; }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }

        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                _enabled = value;
            }
        }
        public ScheduleSettings Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                _settings = value;
            }
        }

        /// <summary>
        /// Determines if the event has elapsed.
        /// </summary>
        public abstract bool HasElapsed();

        /// <summary>
        /// Determines if the event has expired.
        /// </summary>
        public abstract bool HasExpired();

        /// <summary>
        /// Called when the schedule has elapsed.
        /// </summary>
        protected internal virtual void OnElapsed(ScheduleEventArgs args)
        {
            if (Elapsed != null)
            {
                Elapsed(this, args);
            }
        }

        /// <summary>
        /// Called when the schedule has expired.
        /// </summary>
        protected internal virtual void OnExpired(ScheduleEventArgs args)
        {
            if (Expired != null)
            {
                Expired(this, args);
            }
        }
    }
}
