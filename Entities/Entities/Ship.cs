using System;
using System.Collections.Generic;
using Tools;
using Tools.Attributes;

namespace Entities.Entities
{
    public class Ship : DictionaryEntity<int>
    {
        public string Name { get; set; }

        [IgnoreProperty(EditMode.Add)]
        public int SceneId { get; set; }

        [Display(Enabled = false)]
        public double X { get; set; }

        [Display(Enabled = false)]
        public double Y { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {3}{1}Name: {0}{1}SceneId: {2}{1}", Name, Environment.NewLine,
                SceneId, Id);
        }

        public override IEnumerable<string> AsColumns()
        {
            return new string[] { "Id", "Name", "SceneId", "X", "Y" };
        }

        public override object[] AsDataRow()
        {
            return new object[] { Id, Name, SceneId, X, Y };
        }
    }
}
