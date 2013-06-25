using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Infrastructure.Scheduling
{
    [Serializable]
    public class SettingBase
    {

        internal Dictionary<string, object> Properties { get; set; }
        internal string SettingsId { get; private set; }
        public string Name { get; set; }

        public SettingBase()
        {
            Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
        public SettingBase(XElement elem)
        {
            Properties = ExtractDictionary(elem);
        }
        public SettingBase(Dictionary<string, object> properties)
        {
            Properties = properties;
        }

        public void Add(string settingName, object settingValue)
        {
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException("settingName");
            }
            if (Properties.ContainsKey(settingName))
            {
                Properties[settingName] = settingValue;
            }
            else if (settingValue != null)
            {
                Properties.Add(settingName, settingValue);
            }
        }
        public bool Contains(string settingName)
        {
            bool contains = false;
            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException("settingName");
            }
            contains = Properties.ContainsKey(settingName);

            return contains;
        }
        public bool Contains(string[] settingNames)
        {
            bool contains = false;
            foreach (string settingName in settingNames)
            {
                if (Contains(settingName))
                {
                    contains = true;
                    break;
                }
            }

            return contains;
        }
        public DateTime GetDateTime(string settingName)
        {
            return GetDateTime(settingName, new DateTime(0));
        }
        public DateTime GetDateTime(string settingName, DateTime defaultValue)
        {
            return DateTime.Parse(GetString(settingName, defaultValue.ToString("MM/dd/yyyy hh:mm:ss tt")));
        }
        public DateTime? GetNullableDateTime(string settingName)
        {
            bool contains = Contains(settingName);
            if (contains)
            {
                return DateTime.Parse(GetString(settingName));
            }
            return null;
        }
        public double GetDouble(string settingName)
        {
            return GetDouble(settingName, 0);
        }
        public double GetDouble(string settingName, double defaultValue)
        {
            return double.Parse(GetString(settingName, defaultValue.ToString()));
        }

        public bool GetBoolean(string settingName)
        {
            return GetBoolean(settingName, false);
        }

        public bool GetBoolean(string settingName, bool defaultValue)
        {
            return bool.Parse(GetString(settingName, defaultValue.ToString()));
        }
        public bool GetBoolean(string[] settingNames)
        {
            return GetBoolean(settingNames, false);
        }
        public bool GetBoolean(string[] settingNames, bool defaultValue)
        {
            return bool.Parse(GetString(settingNames, defaultValue.ToString()));
        }
        public string GetString(string settingName)
        {
            return GetString(settingName, null);
        }
        public string GetString(string settingName, string defaultValue)
        {
            object objectValue = GetObject(settingName, defaultValue);
            string stringValue = null;

            if (objectValue != null)
            {
                stringValue = objectValue.ToString();

                if (stringValue.Length == 0)
                {
                    stringValue = defaultValue;
                }
            }

            return stringValue;
        }
        public string GetString(string[] settingNames)
        {
            return GetString(settingNames, null);
        }
        public string GetString(string[] settingNames, string defaultValue)
        {
            return GetString(settingNames, defaultValue, false);
        }
        public string GetString(string[] settingNames, string defaultValue, bool ignoreFormat)
        {
            object objectValue = GetObject(settingNames, defaultValue);
            string stringValue = null;

            if (objectValue != null)
            {
                stringValue = objectValue.ToString();

                if (stringValue.Length == 0)
                {
                    stringValue = defaultValue;
                }
            }

            return stringValue;
        }
        public int GetInt32(string settingName)
        {
            return GetInt32(settingName, 0);
        }
        public int GetInt32(string settingName, int defaultValue)
        {
            return int.Parse(GetString(settingName, defaultValue.ToString()));
        }
        public object GetObject(string settingName)
        {
            return GetObject(settingName, null);
        }

        public object GetObject(string settingName, object defaultValue)
        {
            object value = null;

            if (Contains(settingName))
            {
                value = Properties[settingName];
            }

            if (value == null || value.ToString().Length == 0)
            {
                value = defaultValue;
            }

            return value;
        }
        public object GetObject(string[] settingNames)
        {
            return GetObject(settingNames, null);
        }
        public object GetObject(string[] settingNames, object defaultValue)
        {
            object value = null;
            foreach (string settingName in settingNames)
            {
                if (Contains(settingName))
                {
                    value = Properties[settingName];

                    if (value != null)
                    {
                        break;
                    }
                }
            }

            if (value == null || value.ToString().Length == 0)
            {
                value = defaultValue;
            }

            return value;
        }
        public void SetValue(string settingName, object settingValue)
        {
            bool removeIfExists = false;

            if (settingValue != null)
            {
                if (settingValue is DateTime)
                {
                    if (settingValue.Equals(DateUtility.ZeroDateTime))
                    {
                        removeIfExists = true;
                    }
                    else
                    {
                        settingValue = ((DateTime)settingValue).ToString("MM/dd/yyyy hh:mm:ss tt");
                    }
                }
                else if (settingValue is TimeSpan)
                {
                    if (settingValue.Equals(TimeSpan.Zero))
                    {
                        removeIfExists = true;
                    }
                    else
                    {
                        settingValue = ((TimeSpan)settingValue).ToString();
                    }
                }
                else if (settingValue is Guid)
                {
                    if (settingValue.Equals(Guid.Empty))
                    {
                        removeIfExists = true;
                    }
                    else
                    {
                        settingValue = ((Guid)settingValue).ToString("N").ToUpper();
                    }
                }
                else if (settingValue is DateRange)
                {
                    if (settingValue.Equals(DateRange.Empty))
                    {
                        removeIfExists = true;
                    }
                    else
                    {
                        settingValue = ((DateRange)settingValue).ToString("MM/dd/yyyy").ToUpper();
                    }
                }
                else if (settingValue is TimeRange)
                {
                    if (settingValue.Equals(TimeRange.Empty))
                    {
                        removeIfExists = true;
                    }
                    else
                    {
                        settingValue = ((TimeRange)settingValue).ToString("hh:mm:ss tt");
                    }
                }
            }
            else
            {
                removeIfExists = true;
            }

            if (Properties.ContainsKey(settingName))
            {
                if (removeIfExists)
                {
                    Remove(settingName);
                }
                else
                {
                    Properties[settingName] = settingValue;
                }
            }
            else if (!removeIfExists)
            {
                Type valueType = null;

                if (settingValue != null)
                {
                    valueType = settingValue.GetType();
                }
                Properties.Add(settingName, settingValue);

            }

        }
        public void SetValue(string[] settingNames, object settingValue)
        {

            foreach (string settingName in settingNames)
            {
                if (Properties.ContainsKey(settingName))
                {
                    Properties[settingName] = settingValue;
                    break;
                }
            }
        }
        public void Remove(string settingName)
        {

            if (string.IsNullOrEmpty(settingName))
            {
                throw new ArgumentNullException("settingName");
            }
            if (!Properties.ContainsKey(settingName))
            {
                throw new ArgumentException("The property named \"" + settingName + "\" does not exist in the settings.");
            }

            try
            {
                Properties.Remove(settingName);
            }
            catch
            {
                // eat any exceptions.
            }
        }

        public void Remove(string[] settingNames)
        {

            foreach (string settingName in settingNames)
            {
                if (Properties.ContainsKey(settingName))
                {
                    Properties.Remove(settingName);
                    break;
                }
            }
        }

        private Dictionary<string, object> ExtractDictionary(XElement elem)
        {
            var dictionary = new Dictionary<string, object>();
            dictionary = (from x in elem.Elements("item") select x).ToDictionary(k => k.Attribute("id").Value,
                k => (object)k.Attribute("value").Value);
            return dictionary;
        }
    }
}
