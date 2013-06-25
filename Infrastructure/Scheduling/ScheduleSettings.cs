using System;
using System.Collections.Generic;


namespace Infrastructure.Scheduling
{
    [Serializable]
    public class ScheduleSettings : SettingBase
    {
        public ScheduleSettings()
        {
        }

        public ScheduleSettings(Dictionary<string, object> properties)
            : base(properties)
        {

        }

        private string _name;

        public new string Name
        {
            get
            {
                if (Contains("name"))
                {
                    _name = GetString("name");
                }
                return _name;
            }
            set { SetValue("name", value); }
        }

        private string _subType;

        public string SubType
        {
            get
            {
                if (Contains("subType"))
                {
                    _subType = GetString("subType");
                }
                return _subType;
            }
            set { SetValue("subType", value); }
        }

        private DateTime _dateUpdated;

        public DateTime DateUpdated
        {
            get
            {
                if (Contains("dateUpdated"))
                {
                    _dateUpdated = GetDateTime("dateUpdated");
                }
                return _dateUpdated;
            }
            set { SetValue("dateUpdated", value); }
        }

        private DateTime _scheduleExpiration;

        public DateTime ScheduleExpiration
        {
            get
            {
                if (Contains("scheduleExpiration"))
                {
                    _scheduleExpiration = GetDateTime("scheduleExpiration");
                }
                return _dateUpdated;
            }
            set { SetValue("maintenanceExpiration", value); }
        }

        private DateTime _initializeDate;

        public DateTime InitializeDate
        {
            get
            {
                if (Contains("initializeDate"))
                {
                    _initializeDate = GetDateTime("initializeDate");
                }
                return _initializeDate;
            }
            set { SetValue("initializeDate", value); }
        }

        private int _id;

        public int Id
        {
            get
            {
                if (Contains("id"))
                {
                    _id = GetInt32("id");
                }
                return _id;
            }

            set { SetValue("id", value); }
        }

        private string _type;

        public string Type
        {
            get
            {
                if (Contains("type"))
                {
                    _type = GetString("type");
                }
                return _type;
            }
            set { SetValue("type", value); }
        }

        public string _scheduleStartTime;

        public string ScheduleStartTime
        {
            get
            {
                if (Contains("scheduleStartTime"))
                {
                    _scheduleStartTime = GetString("scheduleStartTime");
                }
                return _scheduleStartTime;
            }
            set { SetValue("scheduleStartTime", value); }
        }

        private bool _enabled;

        public bool Enabled
        {
            get
            {
                if (Contains("enabled"))
                {
                    _enabled = GetBoolean("enabled");
                }
                return _enabled;
            }
            set { SetValue("enabled", value); }

        }

        public string _identity;

        public string Identity
        {
            get
            {
                if (Contains("identity"))
                {
                    _identity = GetString("identity");
                }
                return _identity;
            }
            set { SetValue("identity", value); }

        }

        private string _password;

        public string Password
        {
            get
            {
                if (Contains("password"))
                {
                    _password = GetString("password");
                }
                return _password;
            }
            set { SetValue("password", value); }
        }

        private DateTime _lastElapsed;

        public DateTime LastElapsed
        {
            get
            {
                if (Contains("lastElapsed"))
                {
                    _lastElapsed = GetDateTime("lastElapsed");
                }
                return _lastElapsed;
            }
            set { SetValue("lastElapsed", value); }

        }

        private DateTime _dateExpired;

        public DateTime DateExpired
        {
            get
            {
                if (Contains("dateExpired"))
                {
                    _dateExpired = GetDateTime("dateExpired");
                }
                return _dateExpired;
            }
            set { SetValue("dateExpired", value); }

        }


    }
}
