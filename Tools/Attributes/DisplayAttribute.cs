using System;

namespace Tools.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayAttribute : Attribute
    {
        public string Label { get; }

        public bool Enabled { get; set; } = true;

        public DisplayAttribute(string label)
        {
            Label = label;
        }

        //public Type ControlType { get; set; }
    }
}
