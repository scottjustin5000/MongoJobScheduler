using System;
using System.Collections.Generic;

namespace Infrastructure.Scheduling
{
    public class ScheduleSection
    {
        private List<ScheduleSettings> _settings;
        public virtual List<ScheduleSettings> ScheduleSettings
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

        public bool Contains(string name)
        {
            //loop thorugh
            throw new NotImplementedException();
        }
        public ScheduleSettings GetByName(string name)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Returns the type of the specified alias, or null if the 
        /// alias does not exist in the settings collection. This 
        /// method can basically be used to determine if a settings
        /// type has been aliased.
        /// </summary>
        public string TypeOf(string alias)
        {
            throw new NotImplementedException();
        }


    }
}
