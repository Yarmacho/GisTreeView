﻿using System;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Gas : EntityBase<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int ExperimentId { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}ExperimentId: {2}{1}", Name, Environment.NewLine,
                ExperimentId, Id);
        }
    }
}
