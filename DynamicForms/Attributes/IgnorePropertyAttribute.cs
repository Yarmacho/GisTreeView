﻿using System;

namespace DynamicForms.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class IgnorePropertyAttribute : Attribute
    {
        public EditMode EditMode { get; }

        public IgnorePropertyAttribute(EditMode editMode)
        {
            EditMode = editMode;
        }
    }
}
