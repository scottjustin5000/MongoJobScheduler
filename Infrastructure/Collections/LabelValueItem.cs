using System.Text;
using System.IO;

namespace Common.Lists
{
    // TODO Replace with KeyValuePair<T, K>
    public class LabelValueItem
    {
        public string Label { get; set; }
        public string Value { get; set; }

        public LabelValueItem()
        {
        }

        public LabelValueItem(string value, string label)
        {
            this.Value = value;
            this.Label = label;
        }
    }
}
