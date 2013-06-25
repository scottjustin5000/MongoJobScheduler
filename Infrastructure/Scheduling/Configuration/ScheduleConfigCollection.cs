using System;
using System.Configuration;


namespace Infrastructure.Scheduling.Configuration
{
    public class ScheduleConfigCollection : ConfigurationElementCollection
    {
        public ScheduleConfigCollection()
        {
            ScheduleConfigElement drvr = (ScheduleConfigElement)CreateNewElement();
            Add(drvr);
        }

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.AddRemoveClearMap;
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduleConfigElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ScheduleConfigElement)element).Name;
        }

        public ScheduleConfigElement this[int index]
        {
            get
            {
                return (ScheduleConfigElement)BaseGet(index);
            }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        new public ScheduleConfigElement this[string Name]
        {
            get
            {
                return (ScheduleConfigElement)BaseGet(Name);
            }
        }

        public int IndexOf(ScheduleConfigElement drivr)
        {
            return BaseIndexOf(drivr);
        }

        public void Add(ScheduleConfigElement drvr)
        {
            BaseAdd(drvr);
        }
        protected override void BaseAdd(ConfigurationElement element)
        {
            BaseAdd(element, false);
        }

        public void Remove(ScheduleConfigElement drvr)
        {
            if (BaseIndexOf(drvr) >= 0)
            {
                BaseRemove(drvr.Name);
            }
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }

        public void Clear()
        {
            BaseClear();
        }
    }
}
