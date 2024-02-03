using System;

namespace DynamicForms.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class DisplayAttribute : Attribute
    {
        public string Label { get; }

        public DisplayAttribute(string label)
        {
            Label = label;
        }

        //public Type ControlType { get; set; }
    }
}
